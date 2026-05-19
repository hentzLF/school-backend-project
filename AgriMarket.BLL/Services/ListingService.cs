using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Listings;
using AgriMarket.BLL.Dtos.Locations;
using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class ListingService(
    IListingRepository serviceListings,
    IRepository<UserProfile> userProfiles,
    IRepository<Booking> bookings,
    IRepository<Municipality> municipalities,
    IRepository<Location> locations,
    IAvailabilityRepository availabilities,
    IUnitOfWork uow,
    IReviewService reviewService,
    ILogger<ListingService> logger) : IListingService
{
    private static readonly BookingStatus[] ActiveBookingStatuses =
    [
        BookingStatus.Pending,
        BookingStatus.Confirmed,
        BookingStatus.InProgress,
        BookingStatus.ProviderCompleted
    ];

    public async Task<IEnumerable<ListingSummaryDto>> GetAllAsync()
    {
        var listings = await serviceListings.ListWithSummaryAsync();
        return await BuildListingSummaryDtosAsync(listings);
    }

    public async Task<ListingDto?> GetByIdAsync(Guid id)
    {
        var listing = await serviceListings.GetWithFullDetailsAsync(id);
        return listing is null ? null : await BuildListingDtoAsync(listing);
    }

    public async Task<IEnumerable<ListingSummaryDto>> GetByProviderAsync(Guid providerProfileId)
    {
        var listings = await serviceListings.ListWithSummaryAsync(l => l.UserProfileId == providerProfileId);
        return await BuildListingSummaryDtosAsync(listings);
    }

    public async Task<ListingDto> CreateAsync(Guid userId, CreateListingDto dto)
    {
        var profile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = new ServiceListing
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ServiceCategoryId = dto.ServiceCategoryId,
            PricePerHectare = dto.PricePerHectare,
            UserProfileId = profile.Id,
            IsActive = false
        };

        if (dto.Location is not null)
        {
            await ValidateMunicipalityExists(dto.Location.MunicipalityId);
            ValidateCoordinates(dto.Location.Latitude, dto.Location.Longitude);

            var location = BuildLocationFromDto(dto.Location.MunicipalityId, dto.Location.Address, dto.Location.Latitude, dto.Location.Longitude);
            locations.Add(location);
            listing.LocationId = location.Id;
        }

        serviceListings.Add(listing);
        await uow.SaveChangesAsync();

        return (await GetByIdAsync(listing.Id))!;
    }

    public async Task<ListingDto> UpdateAsync(Guid userId, UpdateListingDto dto)
    {
        var profile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == dto.Id);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {dto.Id} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        listing.Title = dto.Title;
        listing.Description = dto.Description;
        listing.ServiceCategoryId = dto.ServiceCategoryId;
        listing.PricePerHectare = dto.PricePerHectare;
        listing.IsActive = dto.IsActive;
        await UpdateListingLocationAsync(listing, dto.Location);
        await uow.SaveChangesAsync();

        return (await GetByIdAsync(listing.Id))!;
    }

    public async Task DeleteAsync(Guid userId, Guid listingId)
    {
        var profile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {listingId} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        var hasActiveBookings = await bookings.AnyAsync(b =>
            b.ServiceListingId == listingId &&
            ActiveBookingStatuses.Contains(b.Status));

        if (hasActiveBookings)
            throw new BusinessRuleException("Cannot delete listing with active bookings.");

        serviceListings.Remove(listing);
        await uow.SaveChangesAsync();
    }

    public async Task<ListingDto> AdminUpdateAsync(UpdateListingDto dto)
    {
        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == dto.Id);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {dto.Id} not found.");

        listing.Title = dto.Title;
        listing.Description = dto.Description;
        listing.ServiceCategoryId = dto.ServiceCategoryId;
        listing.PricePerHectare = dto.PricePerHectare;
        listing.IsActive = dto.IsActive;
        await UpdateListingLocationAsync(listing, dto.Location);
        await uow.SaveChangesAsync();

        return (await GetByIdAsync(listing.Id))!;
    }

    public async Task AdminDeleteAsync(Guid listingId)
    {
        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {listingId} not found.");

        var hasActiveBookings = await bookings.AnyAsync(b =>
            b.ServiceListingId == listingId &&
            ActiveBookingStatuses.Contains(b.Status));

        if (hasActiveBookings)
            throw new BusinessRuleException("Cannot delete listing with active bookings.");

        serviceListings.Remove(listing);
        await uow.SaveChangesAsync();
    }

    public async Task AdminToggleActiveAsync(Guid listingId)
    {
        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {listingId} not found.");

        listing.IsActive = !listing.IsActive;
        await uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<ListingSummaryDto>> GetActiveListingsAsync()
    {
        var listings = await serviceListings.ListWithSummaryAsync(l => l.IsActive);
        return await BuildListingSummaryDtosAsync(listings);
    }

    public async Task ToggleActiveAsync(Guid userId, Guid listingId)
    {
        var profile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {listingId} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        listing.IsActive = !listing.IsActive;
        await uow.SaveChangesAsync();
    }

    public async Task<AvailabilityDto> AddAvailabilityAsync(Guid userId, CreateAvailabilityDto dto)
    {
        if (dto.StartTime >= dto.EndTime)
            throw new BusinessRuleException("Start time must be before end time.");

        var profile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await serviceListings.FirstOrDefaultAsync(l => l.Id == dto.ListingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {dto.ListingId} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            ServiceListingId = dto.ListingId,
            StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc),
            IsBooked = false
        };

        availabilities.Add(availability);
        await uow.SaveChangesAsync();
        return ToAvailabilityDto(availability);
    }

    public async Task DeleteAvailabilityAsync(Guid userId, Guid availabilityId)
    {
        var profile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var availability = await availabilities.GetWithListingAsync(availabilityId);
        if (availability is null)
            throw new KeyNotFoundException($"Availability {availabilityId} not found.");

        if (availability.ServiceListing?.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this availability.");

        if (availability.IsBooked)
            throw new BusinessRuleException("Cannot delete a booked availability slot.");

        availabilities.Remove(availability);
        await uow.SaveChangesAsync();
    }

    public async Task<AvailabilityDto?> GetAvailabilityByIdAsync(Guid id)
    {
        var availability = await availabilities.FirstOrDefaultAsync(a => a.Id == id);
        return availability is null ? null : ToAvailabilityDto(availability);
    }

    private async Task<IEnumerable<ListingSummaryDto>> BuildListingSummaryDtosAsync(IEnumerable<ServiceListing> listings)
    {
        var result = new List<ListingSummaryDto>();
        foreach (var listing in listings)
        {
            var stats = await reviewService.GetRatingStatsForListingAsync(listing.Id);
            result.Add(ToListingSummaryDto(listing, stats));
        }
        return result;
    }

    private async Task<ListingDto> BuildListingDtoAsync(ServiceListing listing)
    {
        var stats = await reviewService.GetRatingStatsForListingAsync(listing.Id);
        return ToListingDto(listing, stats);
    }

    private static ListingSummaryDto ToListingSummaryDto(ServiceListing listing, RatingStatsDto? stats = null)
    {
        return new ListingSummaryDto
        {
            Id = listing.Id,
            Title = listing.Title,
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            ProviderName = listing.UserProfile is null
                ? "Unknown"
                : $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}",
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            AverageRating = stats?.AverageRating ?? 0,
            ReviewCount = stats?.ReviewCount ?? 0
        };
    }

    private static ListingDto ToListingDto(ServiceListing listing, RatingStatsDto? stats = null)
    {
        return new ListingDto
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            UserProfileId = listing.UserProfileId,
            ServiceCategoryId = listing.ServiceCategoryId,
            Location = ToLocationDto(listing.Location),
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            ProviderName = listing.UserProfile is null
                ? "Unknown"
                : $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}",
            ProviderUserId = listing.UserProfile?.AppUserId,
            Availabilities = (listing.Availabilities ?? [])
                .OrderBy(a => a.StartTime)
                .Select(ToAvailabilityDto)
                .ToList(),
            Equipments = (listing.ServiceListingEquipments ?? [])
                .Where(sle => sle.Equipment is not null)
                .Select(sle => new Dtos.Listings.EquipmentDto
                {
                    Id = sle.Equipment!.Id,
                    Name = sle.Equipment.Name,
                    Make = sle.Equipment.Make,
                    Model = sle.Equipment.Model,
                    ManufactureYear = sle.Equipment.ManufactureYear,
                    HorsePower = sle.Equipment.HorsePower,
                    Condition = sle.Equipment.Condition,
                    Status = sle.Equipment.Status,
                    Description = sle.Equipment.Description
                })
                .ToList(),
            AverageRating = stats?.AverageRating ?? 0,
            ReviewCount = stats?.ReviewCount ?? 0
        };
    }

    private static LocationDto? ToLocationDto(Location? location)
    {
        if (location?.Municipality is null)
            return null;

        return new LocationDto(
            location.Id,
            location.MunicipalityId,
            location.Municipality.Name,
            location.Municipality.CountyId,
            location.Municipality.County?.Name ?? "Unknown",
            location.Address,
            location.Latitude,
            location.Longitude);
    }

    private async Task UpdateListingLocationAsync(ServiceListing listing, UpdateLocationDto? locationDto)
    {
        if (locationDto is null)
        {
            await RemoveExistingLocationAsync(listing);
            return;
        }

        await ValidateMunicipalityExists(locationDto.MunicipalityId);
        ValidateCoordinates(locationDto.Latitude, locationDto.Longitude);

        if (listing.LocationId.HasValue)
        {
            var existingLocation = await locations.GetByIdAsync(listing.LocationId.Value);
            if (existingLocation is not null)
            {
                existingLocation.MunicipalityId = locationDto.MunicipalityId;
                existingLocation.Address = locationDto.Address;
                existingLocation.Latitude = locationDto.Latitude;
                existingLocation.Longitude = locationDto.Longitude;
                locations.Update(existingLocation);
                return;
            }
        }

        var newLocation = BuildLocationFromDto(locationDto.MunicipalityId, locationDto.Address, locationDto.Latitude, locationDto.Longitude);
        locations.Add(newLocation);
        listing.LocationId = newLocation.Id;
    }

    private async Task RemoveExistingLocationAsync(ServiceListing listing)
    {
        if (!listing.LocationId.HasValue)
            return;

        var existingLocation = await locations.GetByIdAsync(listing.LocationId.Value);
        if (existingLocation is not null)
            locations.Remove(existingLocation);

        listing.LocationId = null;
    }

    private async Task ValidateMunicipalityExists(Guid municipalityId)
    {
        var exists = await municipalities.AnyAsync(m => m.Id == municipalityId);
        if (!exists)
            throw new BusinessRuleException($"Municipality {municipalityId} does not exist.");
    }

    private static void ValidateCoordinates(double? latitude, double? longitude)
    {
        if (latitude.HasValue != longitude.HasValue)
            throw new BusinessRuleException("Both Latitude and Longitude must be provided together.");

        if (latitude.HasValue && (latitude.Value < -90 || latitude.Value > 90))
            throw new BusinessRuleException("Latitude must be between -90 and 90.");

        if (longitude.HasValue && (longitude.Value < -180 || longitude.Value > 180))
            throw new BusinessRuleException("Longitude must be between -180 and 180.");
    }

    private static Location BuildLocationFromDto(Guid municipalityId, string? address, double? latitude, double? longitude)
    {
        return new Location
        {
            Id = Guid.NewGuid(),
            MunicipalityId = municipalityId,
            Address = address,
            Latitude = latitude,
            Longitude = longitude
        };
    }

    private static AvailabilityDto ToAvailabilityDto(Availability availability)
    {
        return new AvailabilityDto
        {
            Id = availability.Id,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            IsBooked = availability.IsBooked,
            ServiceListingId = availability.ServiceListingId
        };
    }
}
