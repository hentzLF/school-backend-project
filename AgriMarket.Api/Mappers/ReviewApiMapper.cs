using AgriMarket.BLL.Dtos.Reviews;

namespace AgriMarket.Api.Mappers;

public static class ReviewApiMapper
{
    public static UpdateReviewDto WithRouteId(this UpdateReviewDto dto, Guid id)
    {
        dto.Id = id;
        return dto;
    }
}
