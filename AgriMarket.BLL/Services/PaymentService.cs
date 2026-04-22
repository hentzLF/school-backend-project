using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class PaymentService(
    IRepository<Payment> payments,
    IUnitOfWork uow,
    ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<IEnumerable<Payment>> GetAllAsync(PaymentStatus? status)
    {
        var query = payments.Query();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await payments.Query()
            .Include(p => p.Booking)
                .ThenInclude(b => b!.ClientProfile)
            .Include(p => p.Booking)
                .ThenInclude(b => b!.ServiceListing)
                    .ThenInclude(l => l!.UserProfile)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task ResolveDisputeAsync(Guid paymentId, string resolution)
    {
        var payment = await payments.GetByIdAsync(paymentId);
        if (payment != null && payment.Status == PaymentStatus.Disputed)
        {
            if (resolution == "Release")
            {
                payment.Status = PaymentStatus.Released;
                payment.ReleasedAt = DateTime.UtcNow;
            }
            else if (resolution == "Refund")
            {
                payment.Status = PaymentStatus.Refunded;
            }
            await uow.SaveChangesAsync();
        }
    }
}
