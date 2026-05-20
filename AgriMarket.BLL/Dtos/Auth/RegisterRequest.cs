using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Auth;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = default!;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = default!;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = default!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = default!;
}
