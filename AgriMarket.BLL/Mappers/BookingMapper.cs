using AgriMarket.BLL.Dtos.Bookings;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Mappers;

public static class BookingMapper
{
    public static BookingDto ToBookingDto(this Booking booking) =>
        new()
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ServiceListingId = booking.ServiceListingId,
            ClientProfileId = booking.ClientProfileId,
            AvailabilityId = booking.AvailabilityId,
            ClientName = booking.ClientProfile is null
                ? "Unknown"
                : $"{booking.ClientProfile.FirstName} {booking.ClientProfile.LastName}",
            ListingTitle = booking.ServiceListing?.Title ?? "Unknown",
            ProviderProfileId = booking.ServiceListing?.UserProfileId ?? Guid.Empty,
            AvailabilityStart = booking.Availability?.StartTime ?? default,
            AvailabilityEnd = booking.Availability?.EndTime ?? default
        };

    public static BookingSummaryDto ToBookingSummaryDto(this Booking booking) =>
        new()
        {
            Id = booking.Id,
            ClientName = booking.ClientProfile is null
                ? "Unknown"
                : $"{booking.ClientProfile.FirstName} {booking.ClientProfile.LastName}",
            Status = booking.Status,
            AreaInHectares = booking.AreaInHectares,
            TotalPrice = booking.TotalPrice,
            CreatedAt = booking.CreatedAt
        };
}
