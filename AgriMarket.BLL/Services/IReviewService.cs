using AgriMarket.BLL.Dtos.Reviews;

namespace AgriMarket.BLL.Services;

public interface IReviewService
{
    Task<ReviewDto?> GetByBookingAsync(Guid bookingId);
    Task<ReviewDto> CreateAsync(Guid userId, CreateReviewDto dto);
    Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<ReviewDto?> GetByIdAsync(Guid id);
    Task<ReviewDto> UpdateAsync(Guid userId, UpdateReviewDto dto);
    Task DeleteAsync(Guid userId, Guid reviewId);
    Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetByProfileAsync(Guid profileId, int page, int pageSize);
    Task<RatingStatsDto> GetRatingStatsForProfileAsync(Guid profileId);
    Task<RatingStatsDto> GetRatingStatsForListingAsync(Guid listingId);
}
