using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;

    public PaymentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync(PaymentStatus? status)
    {
        var query = _db.Payments.AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _db.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b!.ClientProfile)
            .Include(p => p.Booking)
                .ThenInclude(b => b!.ServiceListing)
                    .ThenInclude(l => l!.UserProfile)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task ResolveDisputeAsync(Guid paymentId, string resolution)
    {
        var payment = await _db.Payments.FindAsync(paymentId);
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
            await _db.SaveChangesAsync();
        }
    }
}
