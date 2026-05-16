using System.Linq.Expressions;
using AgriMarket.BLL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL.Repositories;

public class EfQueryMaterializer : IQueryMaterializer
{
    public Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken ct)
        => EntityFrameworkQueryableExtensions.ToListAsync(query, ct);

    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct)
        => EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, ct);

    public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct)
        => EntityFrameworkQueryableExtensions.CountAsync(query, ct);

    public async Task<decimal> SumAsync<T>(IQueryable<T> query, Expression<Func<T, decimal?>> selector, CancellationToken ct)
        => await EntityFrameworkQueryableExtensions.SumAsync(query, selector, ct) ?? 0m;

    public async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TSource, TKey, TValue>(
        IQueryable<TSource> query,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector,
        CancellationToken ct) where TKey : notnull
    {
        var list = await EntityFrameworkQueryableExtensions.ToListAsync(query, ct);
        return list.ToDictionary(keySelector, valueSelector);
    }
}
