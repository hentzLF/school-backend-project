using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Locations;

public sealed class UpdateLocationDto : IValidatableObject
{
    [Required]
    public Guid MunicipalityId { get; init; }

    public string? Address { get; init; }

    [Range(-90.0, 90.0)]
    public double? Latitude { get; init; }

    [Range(-180.0, 180.0)]
    public double? Longitude { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Latitude.HasValue != Longitude.HasValue)
        {
            yield return new ValidationResult(
                "Both Latitude and Longitude must be provided together.",
                [nameof(Latitude), nameof(Longitude)]);
        }
    }
}
