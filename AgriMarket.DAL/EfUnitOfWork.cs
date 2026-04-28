namespace AgriMarket.DAL;

public class EfUnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task BeginTransactionAsync(CancellationToken ct = default)
        => db.Database.BeginTransactionAsync(ct);

    public Task CommitTransactionAsync(CancellationToken ct = default)
        => db.Database.CommitTransactionAsync(ct);

    public Task RollbackTransactionAsync(CancellationToken ct = default)
        => db.Database.RollbackTransactionAsync(ct);
}
