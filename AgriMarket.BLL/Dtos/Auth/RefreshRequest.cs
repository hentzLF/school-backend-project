namespace AgriMarket.BLL.Dtos.Auth;

public sealed class RefreshRequest
{
    public string RefreshToken { get; init; } = default!;
}
