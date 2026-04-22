using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Bookings;

public sealed class CreateBookingDto
{
    [Required]
    public Guid ServiceListingId { get; init; }

    [Required]
    public Guid AvailabilityId { get; init; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "AreaInHectares must be positive.")]
    public double AreaInHectares { get; init; }

    public string? Notes { get; init; }
}
