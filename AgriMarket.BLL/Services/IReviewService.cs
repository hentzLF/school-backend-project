using AgriMarket.BLL.Dtos.Reviews;

namespace AgriMarket.BLL.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByBookingAsync(Guid bookingId);
    Task<ReviewDto> CreateAsync(Guid userId, CreateReviewDto dto);
    Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<ReviewDto?> GetByIdAsync(Guid id);
}
