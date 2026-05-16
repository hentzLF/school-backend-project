using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IAppUserRepository : IRepository<AppUser>
{
    Task<AppUser?> GetByEmailWithProfilesAsync(string email, CancellationToken ct = default);
    Task<AppUser?> GetByIdWithProfilesAsync(Guid id, CancellationToken ct = default);
}
