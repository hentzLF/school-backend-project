using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Dtos.Auth;

public sealed class RegisterRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public RoleType Role { get; init; }
}
