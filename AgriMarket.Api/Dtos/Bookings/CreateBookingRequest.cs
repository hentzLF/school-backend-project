using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Api.Dtos.Bookings;

public class CreateBookingRequest
{
    [Required]
    public Guid ServiceListingId { get; set; }

    [Required]
    public Guid ClientProfileId { get; set; }

    [Required]
    public Guid AvailabilityId { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "AreaInHectares must be positive.")]
    public double AreaInHectares { get; set; }

    public string? Notes { get; set; }
}
