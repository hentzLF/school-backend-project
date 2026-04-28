using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Reviews;

public sealed class CreateReviewDto
{
    [Required]
    public Guid BookingId { get; init; }

    [Range(1, 5)]
    public int Rating { get; init; }

    public string? Comment { get; init; }
}
