namespace AgriMarket.BLL.Dtos.Auth;

public sealed class SelectProfileRequest
{
    public string SessionToken { get; init; } = default!;
    public Guid ProfileId { get; init; }
}
