using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetWithBookingDetailsAsync(Guid id, CancellationToken ct = default);
}
