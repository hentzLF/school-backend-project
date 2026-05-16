using System.Linq.Expressions;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IListingRepository : IRepository<ServiceListing>
{
    Task<List<ServiceListing>> ListWithSummaryAsync(Expression<Func<ServiceListing, bool>>? predicate = null, CancellationToken ct = default);
    Task<ServiceListing?> GetWithFullDetailsAsync(Guid id, CancellationToken ct = default);
}
