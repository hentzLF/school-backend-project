using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Listings;

public sealed class CreateAvailabilityDto
{
    [Required]
    public Guid ListingId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}
