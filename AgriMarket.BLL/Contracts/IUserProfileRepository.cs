using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<List<UserProfile>> ListWithDetailsAsync(CancellationToken ct = default);
    Task<UserProfile?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<UserProfile?> GetByAppUserIdWithDetailsAsync(Guid appUserId, CancellationToken ct = default);
    Task<(List<UserProfile> Items, int TotalCount)> ListPagedWithDetailsAsync(int page, int pageSize, CancellationToken ct = default);
}
