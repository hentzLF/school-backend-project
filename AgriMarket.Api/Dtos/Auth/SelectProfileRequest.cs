namespace AgriMarket.Api.Dtos.Auth;

public sealed class SelectProfileRequest
{
    public string SessionToken { get; init; } = default!;
    public Guid ProfileId { get; init; }
}
