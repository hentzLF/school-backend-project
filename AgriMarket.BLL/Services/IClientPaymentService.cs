using AgriMarket.BLL.Dtos.Payments;

namespace AgriMarket.BLL.Services;

public interface IClientPaymentService
{
    Task<PaymentReceiptDto> PayAsync(Guid callerProfileId, PayRequest request);
    Task<List<PaymentHistoryItemDto>> GetHistoryAsync(Guid callerProfileId);
}
