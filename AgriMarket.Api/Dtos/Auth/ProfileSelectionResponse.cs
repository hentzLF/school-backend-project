namespace AgriMarket.Api.Dtos.Auth;

public sealed class ProfileSelectionResponse
{
    public string SessionToken { get; init; } = default!;
    public List<ProfileSummary> Profiles { get; init; } = default!;
}
