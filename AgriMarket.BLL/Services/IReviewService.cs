using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Services;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetByBookingAsync(Guid bookingId);
    Task<Review> CreateAsync(Review review);
    Task<(IEnumerable<Review> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Review?> GetByIdAsync(Guid id);
}
