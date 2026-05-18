using AgriMarket.BLL.Dtos.Locations;

namespace AgriMarket.BLL.Services;

public interface ILocationLookupService
{
    Task<IReadOnlyList<CountyDto>> GetAllCountiesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MunicipalityDto>> GetMunicipalitiesByCountyAsync(Guid countyId, CancellationToken ct = default);
    Task<bool> CountyExistsAsync(Guid countyId, CancellationToken ct = default);
}
