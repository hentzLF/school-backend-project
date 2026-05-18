namespace AgriMarket.BLL.Dtos.Locations;

public sealed record LocationDto(
    Guid Id,
    Guid MunicipalityId,
    string MunicipalityName,
    Guid CountyId,
    string CountyName,
    string? Address,
    double? Latitude,
    double? Longitude);
