using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Reviews;

public sealed class UpdateReviewDto
{
    public Guid Id { get; set; }

    [Range(1, 5)]
    public int Rating { get; init; }

    public string? Comment { get; init; }
}
