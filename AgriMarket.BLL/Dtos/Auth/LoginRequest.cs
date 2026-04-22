namespace AgriMarket.BLL.Dtos.Auth;

public sealed class LoginRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
