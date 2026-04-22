using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Auth;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = default!;

    [Required]
    public string Password { get; init; } = default!;
}
