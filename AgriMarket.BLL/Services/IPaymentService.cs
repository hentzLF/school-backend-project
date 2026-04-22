using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public interface IPaymentService
{
    Task<IEnumerable<Payment>> GetAllAsync(PaymentStatus? status);
    Task<Payment?> GetByIdAsync(Guid id);
    Task ResolveDisputeAsync(Guid paymentId, string resolution);
}
