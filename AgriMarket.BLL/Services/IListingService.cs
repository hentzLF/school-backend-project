using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Services;

public interface IListingService
{
    Task<IEnumerable<ServiceListing>> GetAllAsync();
    Task<ServiceListing?> GetByIdAsync(Guid id);
    Task<IEnumerable<ServiceListing>> GetByProviderAsync(Guid providerProfileId);
    Task<ServiceListing> CreateAsync(ServiceListing listing);
    Task UpdateAsync(ServiceListing listing);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ServiceListing>> GetActiveListingsAsync();
    Task ToggleActiveAsync(Guid id);
    Task<Availability> AddAvailabilityAsync(Availability availability);
    Task DeleteAvailabilityAsync(Guid id);
    Task<Availability?> GetAvailabilityByIdAsync(Guid id);
    Task UpdateAvailabilityAsync(Availability availability);
}
