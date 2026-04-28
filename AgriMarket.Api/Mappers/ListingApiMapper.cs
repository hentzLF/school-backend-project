using AgriMarket.BLL.Dtos.Listings;

namespace AgriMarket.Api.Mappers;

public static class ListingApiMapper
{
    public static UpdateListingDto WithRouteId(this UpdateListingDto dto, Guid id)
    {
        return new UpdateListingDto
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description,
            ServiceCategoryId = dto.ServiceCategoryId,
            PricePerHectare = dto.PricePerHectare,
            IsActive = dto.IsActive,
            LocationId = dto.LocationId
        };
    }
}
