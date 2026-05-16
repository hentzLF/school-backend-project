using System.Linq.Expressions;

namespace AgriMarket.BLL.Contracts;

public interface IQueryMaterializer
{
    Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct = default);
    Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default);
    Task<decimal> SumAsync<T>(IQueryable<T> query, Expression<Func<T, decimal?>> selector, CancellationToken ct = default);
    Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TSource, TKey, TValue>(
        IQueryable<TSource> query,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector,
        CancellationToken ct = default) where TKey : notnull;
}
