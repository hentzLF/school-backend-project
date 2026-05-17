using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Categories;

public sealed class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = default!;

    [MaxLength(500)]
    public string? Description { get; init; }
}
