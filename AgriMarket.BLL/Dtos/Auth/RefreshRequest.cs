using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Auth;

public sealed class RefreshRequest
{
    [Required]
    public string RefreshToken { get; init; } = default!;
}
