namespace AgriMarket.Api.Dtos.Auth;

public sealed class TokenResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
}
