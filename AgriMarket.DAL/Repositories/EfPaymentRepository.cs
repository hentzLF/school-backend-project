using AgriMarket.BLL.Contracts;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL.Repositories;

public class EfPaymentRepository(AppDbContext db) : EfRepository<Payment>(db), IPaymentRepository
{
    public async Task<Payment?> GetWithBookingDetailsAsync(Guid id, CancellationToken ct = default)
        => await db.Set<Payment>()
            .Include(p => p.Booking)
                .ThenInclude(b => b!.ClientProfile)
            .Include(p => p.Booking)
                .ThenInclude(b => b!.ServiceListing)
                    .ThenInclude(l => l!.UserProfile)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
}
