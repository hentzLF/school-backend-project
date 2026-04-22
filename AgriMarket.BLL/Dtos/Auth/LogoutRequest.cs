namespace AgriMarket.BLL.Dtos.Auth;

public sealed class LogoutRequest
{
    public string RefreshToken { get; init; } = default!;
}
