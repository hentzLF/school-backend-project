namespace AgriMarket.Api.Dtos.ServiceListings;

public class ServiceListingResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal PricePerHectare { get; set; }
    public bool IsActive { get; set; }
    public Guid UserProfileId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public Guid? LocationId { get; set; }
}
