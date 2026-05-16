using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken ct = default);
}
