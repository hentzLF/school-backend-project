using AgriMarket.BLL.Contracts;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL.Repositories;

public class EfAvailabilityRepository(AppDbContext db) : EfRepository<Availability>(db), IAvailabilityRepository
{
    public async Task<Availability?> GetWithListingAsync(Guid id, CancellationToken ct = default)
        => await db.Set<Availability>()
            .Include(a => a.ServiceListing)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
}
