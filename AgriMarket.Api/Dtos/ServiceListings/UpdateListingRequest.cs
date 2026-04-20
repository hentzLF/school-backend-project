using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Api.Dtos.ServiceListings;

public class UpdateListingRequest
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal PricePerHectare { get; set; }

    public bool IsActive { get; set; }

    [Required]
    public Guid ServiceCategoryId { get; set; }

    public Guid? LocationId { get; set; }
}
