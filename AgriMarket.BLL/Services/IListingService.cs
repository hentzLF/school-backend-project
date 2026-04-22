using AgriMarket.BLL.Dtos.Listings;

namespace AgriMarket.BLL.Services;

public interface IListingService
{
    Task<IEnumerable<ListingSummaryDto>> GetAllAsync();
    Task<ListingDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ListingSummaryDto>> GetByProviderAsync(Guid providerProfileId);
    Task<ListingDto> CreateAsync(Guid userId, CreateListingDto dto);
    Task<ListingDto> UpdateAsync(Guid userId, UpdateListingDto dto);
    Task DeleteAsync(Guid userId, Guid listingId);
    Task<IEnumerable<ListingSummaryDto>> GetActiveListingsAsync();
    Task ToggleActiveAsync(Guid userId, Guid listingId);
    Task<AvailabilityDto> AddAvailabilityAsync(Guid userId, CreateAvailabilityDto dto);
    Task DeleteAvailabilityAsync(Guid userId, Guid availabilityId);
    Task<AvailabilityDto?> GetAvailabilityByIdAsync(Guid id);
}
