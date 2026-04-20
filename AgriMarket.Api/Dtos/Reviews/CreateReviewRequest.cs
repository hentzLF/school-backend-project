using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Api.Dtos.Reviews;

public class CreateReviewRequest
{
    [Required]
    public Guid BookingId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }
}
