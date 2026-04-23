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
    public bool CanPay => Status == BookingStatus.AwaitingPayment;
    public bool CanCancel => Status is BookingStatus.Pending or BookingStatus.AwaitingPayment or BookingStatus.Confirmed;
    public decimal PlatformFee => TotalPrice * 0.05m;
    public decimal GrandTotal => TotalPrice + PlatformFee;
}
