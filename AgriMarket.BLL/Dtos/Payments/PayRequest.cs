using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Dtos.Payments;

public sealed record PayRequest(Guid BookingId, PaymentMethod Method);
