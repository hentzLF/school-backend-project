using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Mappers;

public static class ReviewMapper
{
    public static ReviewDto ToReviewDto(this Review review) =>
        new()
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            BookingId = review.BookingId,
            ReviewerProfileId = review.ReviewerProfileId
        };
}
