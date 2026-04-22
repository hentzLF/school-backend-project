using AgriMarket.BLL.Dtos.Bookings;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(BookingStatus? status = null);
    Task<BookingDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookingDto>> GetByClientAsync(Guid clientProfileId);
    Task<IEnumerable<BookingDto>> GetByProviderAsync(Guid providerProfileId);
    Task<BookingDto> CreateAsync(Guid userId, CreateBookingDto dto);
    Task<BookingDto> UpdateStatusAsync(Guid id, BookingStatus status, Guid? callerProfileId = null);
    Task DeleteAsync(Guid id);
    Task<int> GetCountByListingAsync(Guid listingId);
    Task<bool> HasActiveBookingsAsync(Guid listingId);
    Task<IEnumerable<BookingSummaryDto>> GetByListingAsync(Guid listingId);
    Task<(IEnumerable<BookingDto> Items, int TotalCount)> GetAllForProfileAsync(Guid profileId, int page, int pageSize);
}
