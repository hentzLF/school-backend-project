using AgriMarket.BLL;
using AgriMarket.BLL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL;

public class EfUnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(ex.Message);
        }
    }

    public Task BeginTransactionAsync(CancellationToken ct = default)
        => db.Database.BeginTransactionAsync(ct);

    public Task CommitTransactionAsync(CancellationToken ct = default)
        => db.Database.CommitTransactionAsync(ct);

    public Task RollbackTransactionAsync(CancellationToken ct = default)
        => db.Database.RollbackTransactionAsync(ct);
}
