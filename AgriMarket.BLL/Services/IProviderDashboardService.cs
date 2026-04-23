using AgriMarket.BLL.Dtos;

namespace AgriMarket.BLL.Services;

public interface IProviderDashboardService
{
    Task<ProviderDashboardDto> GetStatsAsync(Guid providerProfileId);
}
