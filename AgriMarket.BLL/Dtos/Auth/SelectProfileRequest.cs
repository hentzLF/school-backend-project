using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Auth;

public sealed class SelectProfileRequest
{
    [Required]
    public string SessionToken { get; init; } = default!;

    [Required]
    public Guid ProfileId { get; init; }
}
