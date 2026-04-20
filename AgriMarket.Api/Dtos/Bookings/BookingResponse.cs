using AgriMarket.Domain.Enums;

namespace AgriMarket.Api.Dtos.Bookings;

public class BookingResponse
{
    public Guid Id { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public double AreaInHectares { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
    public Guid ServiceListingId { get; set; }
    public Guid ClientProfileId { get; set; }
    public Guid AvailabilityId { get; set; }
}
