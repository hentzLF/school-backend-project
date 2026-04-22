using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Dtos.Bookings;

public sealed class BookingSummaryDto
{
    public Guid Id { get; init; }
    public string ClientName { get; init; } = default!;
    public BookingStatus Status { get; init; }
    public double AreaInHectares { get; init; }
    public decimal TotalPrice { get; init; }
    public DateTime CreatedAt { get; init; }
}
