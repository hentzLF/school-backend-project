using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Web.Areas.Client.ViewModels.Bookings;

public class CreateBookingViewModel
{
    public Guid ServiceListingId { get; set; }

    [Required]
    public Guid AvailabilityId { get; set; }

    [Required]
    [Range(0.1, 10000, ErrorMessage = "Area must be between 0.1 and 10000 hectares")]
    public double AreaInHectares { get; set; }

    public string? Notes { get; set; }
}
