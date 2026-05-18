using AgriMarket.BLL.Dtos.Reviews;

namespace AgriMarket.Api.Mappers;

public static class ReviewApiMapper
{
    public static UpdateReviewDto WithRouteId(this UpdateReviewDto dto, Guid id)
    {
        return new UpdateReviewDto
        {
            Id = id,
            Rating = dto.Rating,
            Comment = dto.Comment
        };
    }
}
