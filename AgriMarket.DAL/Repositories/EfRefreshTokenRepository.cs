using AgriMarket.BLL.Contracts;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL.Repositories;

public class EfRefreshTokenRepository(AppDbContext db) : EfRepository<RefreshToken>(db), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken ct = default)
        => await db.Set<RefreshToken>()
            .Include(rt => rt.AppUser!)
                .ThenInclude(u => u.Profile)
            .Include(rt => rt.AppUser!)
                .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);
}
