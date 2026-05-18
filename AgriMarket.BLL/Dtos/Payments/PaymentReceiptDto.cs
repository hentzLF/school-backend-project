namespace AgriMarket.BLL.Dtos.Payments;

public sealed record PaymentReceiptDto(
    Guid PaymentId,
    Guid BookingId,
    decimal Amount,
    decimal PlatformFee,
    decimal TotalCharged,
    string Method,
    string Status,
    DateTime PaidAt);
