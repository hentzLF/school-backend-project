using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllAsync(BookingStatus? status = null);
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByClientAsync(Guid clientProfileId);
    Task<IEnumerable<Booking>> GetByProviderAsync(Guid providerProfileId);
    Task<Booking> CreateAsync(Booking booking);
    Task UpdateStatusAsync(Guid id, BookingStatus status, Guid? callerProfileId = null);
    Task DeleteAsync(Guid id);
    Task<int> GetCountByListingAsync(Guid listingId);
    Task<bool> HasActiveBookingsAsync(Guid listingId);
    Task<IEnumerable<Booking>> GetByListingAsync(Guid listingId);
    Task<(IEnumerable<Booking> Items, int TotalCount)> GetAllForProfileAsync(Guid profileId, int page, int pageSize);
}
