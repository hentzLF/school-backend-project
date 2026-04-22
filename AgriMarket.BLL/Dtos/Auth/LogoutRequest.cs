using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Auth;

public sealed class LogoutRequest
{
    [Required]
    public string RefreshToken { get; init; } = default!;
}
