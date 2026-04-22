using AgriMarket.BLL.Dtos.Listings;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Mappers;

public static class ListingMapper
{
    public static ListingSummaryDto ToListingSummaryDto(this ServiceListing listing) =>
        new()
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

    public static ListingDto ToListingDto(this ServiceListing listing) =>
        new()
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
                .Select(a => a.ToAvailabilityDto())
                .ToList()
        };

    public static AvailabilityDto ToAvailabilityDto(this Availability availability) =>
        new()
        {
            Id = availability.Id,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            IsBooked = availability.IsBooked,
            ServiceListingId = availability.ServiceListingId
        };
}
