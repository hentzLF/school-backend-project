using AgriMarket.BLL.Dtos.Listings;
using AgriMarket.BLL;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class ListingService : IListingService
{
    private readonly AppDbContext _db;
    private static readonly BookingStatus[] ActiveBookingStatuses =
    [
        BookingStatus.Pending,
        BookingStatus.Confirmed,
        BookingStatus.InProgress,
        BookingStatus.ProviderCompleted
    ];

    public ListingService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ListingSummaryDto>> GetAllAsync()
    {
        var listings = await _db.ServiceListings
            .AsNoTracking()
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .OrderBy(l => l.Title)
            .ToListAsync();

        return listings.Select(ToListingSummaryDto);
    }

    public async Task<ListingDto?> GetByIdAsync(Guid id)
    {
        var listing = await _db.ServiceListings
            .AsNoTracking()
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .Include(l => l.Availabilities)
            .FirstOrDefaultAsync(l => l.Id == id);

        return listing is null ? null : ToListingDto(listing);
    }

    public async Task<IEnumerable<ListingSummaryDto>> GetByProviderAsync(Guid providerProfileId)
    {
        var listings = await _db.ServiceListings
            .AsNoTracking()
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .Where(l => l.UserProfileId == providerProfileId)
            .OrderBy(l => l.Title)
            .ToListAsync();

        return listings.Select(ToListingSummaryDto);
    }

    public async Task<ListingDto> CreateAsync(Guid userId, CreateListingDto dto)
    {
        var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = new ServiceListing
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ServiceCategoryId = dto.ServiceCategoryId,
            PricePerHectare = dto.PricePerHectare,
            LocationId = dto.LocationId,
            UserProfileId = profile.Id,
            IsActive = false
        };

        _db.ServiceListings.Add(listing);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(listing.Id))!;
    }

    public async Task<ListingDto> UpdateAsync(Guid userId, UpdateListingDto dto)
    {
        var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await _db.ServiceListings.FirstOrDefaultAsync(l => l.Id == dto.Id);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {dto.Id} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        listing.Title = dto.Title;
        listing.Description = dto.Description;
        listing.ServiceCategoryId = dto.ServiceCategoryId;
        listing.PricePerHectare = dto.PricePerHectare;
        listing.IsActive = dto.IsActive;
        listing.LocationId = dto.LocationId;
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(listing.Id))!;
    }

    public async Task DeleteAsync(Guid userId, Guid listingId)
    {
        var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await _db.ServiceListings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {listingId} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        var hasActiveBookings = await _db.Bookings.AnyAsync(b =>
            b.ServiceListingId == listingId &&
            ActiveBookingStatuses.Contains(b.Status));

        if (hasActiveBookings)
            throw new BusinessRuleException("Cannot delete listing with active bookings.");

        _db.ServiceListings.Remove(listing);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ListingSummaryDto>> GetActiveListingsAsync()
    {
        var listings = await _db.ServiceListings
            .AsNoTracking()
            .Where(l => l.IsActive)
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .OrderBy(l => l.Title)
            .ToListAsync();

        return listings.Select(ToListingSummaryDto);
    }

    public async Task ToggleActiveAsync(Guid userId, Guid listingId)
    {
        var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await _db.ServiceListings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {listingId} not found.");

        if (listing.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this listing.");

        listing.IsActive = !listing.IsActive;
        await _db.SaveChangesAsync();
    }

    public async Task<AvailabilityDto> AddAvailabilityAsync(Guid userId, CreateAvailabilityDto dto)
    {
        if (dto.StartTime >= dto.EndTime)
            throw new BusinessRuleException("Start time must be before end time.");

        var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await _db.ServiceListings.AsNoTracking().FirstOrDefaultAsync(l => l.Id == dto.ListingId);
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

        _db.Availabilities.Add(availability);
        await _db.SaveChangesAsync();
        return ToAvailabilityDto(availability);
    }

    public async Task DeleteAvailabilityAsync(Guid userId, Guid availabilityId)
    {
        var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (profile is null)
            throw new BusinessRuleException("User profile not found.");

        var availability = await _db.Availabilities
            .Include(a => a.ServiceListing)
            .FirstOrDefaultAsync(a => a.Id == availabilityId);
        if (availability is null)
            throw new KeyNotFoundException($"Availability {availabilityId} not found.");

        if (availability.ServiceListing?.UserProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this availability.");

        if (availability.IsBooked)
            throw new BusinessRuleException("Cannot delete a booked availability slot.");

        _db.Availabilities.Remove(availability);
        await _db.SaveChangesAsync();
    }

    public async Task<AvailabilityDto?> GetAvailabilityByIdAsync(Guid id)
    {
        var availability = await _db.Availabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        return availability is null ? null : ToAvailabilityDto(availability);
    }

    private static ListingSummaryDto ToListingSummaryDto(ServiceListing listing)
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
            IsActive = listing.IsActive
        };
    }

    private static ListingDto ToListingDto(ServiceListing listing)
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
            LocationId = listing.LocationId,
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            ProviderName = listing.UserProfile is null
                ? "Unknown"
                : $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}",
            ProviderUserId = listing.UserProfile?.AppUserId,
            Availabilities = (listing.Availabilities ?? [])
                .OrderBy(a => a.StartTime)
                .Select(ToAvailabilityDto)
                .ToList()
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
