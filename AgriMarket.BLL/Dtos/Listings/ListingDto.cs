using System.Text.Json.Serialization;

namespace AgriMarket.BLL.Dtos.Listings;

public sealed class ListingDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public decimal PricePerHectare { get; init; }
    public bool IsActive { get; init; }
    public Guid UserProfileId { get; init; }
    public Guid ServiceCategoryId { get; init; }
    public Guid? LocationId { get; init; }

    [JsonIgnore]
    public string CategoryName { get; init; } = "Unknown";

    [JsonIgnore]
    public string ProviderName { get; init; } = "Unknown";

    [JsonIgnore]
    public Guid? ProviderUserId { get; init; }

    [JsonIgnore]
    public IReadOnlyList<AvailabilityDto> Availabilities { get; init; } = [];
}
