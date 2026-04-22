using AgriMarket.Domain.Enums;

namespace AgriMarket.Web.Areas.Client.ViewModels.Bookings;

public class BookingDetailsViewModel
{
    public Guid Id { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal AreaInHectares { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
    public string ListingTitle { get; set; } = default!;
    public Guid ListingId { get; set; }
    public DateTime AvailabilityStart { get; set; }
    public DateTime AvailabilityEnd { get; set; }
    public bool CanConfirmCompletion => Status == BookingStatus.ProviderCompleted;
}
