using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IAvailabilityRepository : IRepository<Availability>
{
    Task<Availability?> GetWithListingAsync(Guid id, CancellationToken ct = default);
}
