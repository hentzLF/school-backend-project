namespace AgriMarket.BLL.Dtos.Auth;

public sealed class LoginResult
{
    public TokenResponse? Tokens { get; init; }
    public ProfileSelectionResponse? ProfileSelection { get; init; }
    public bool RequiresProfileSelection => ProfileSelection is not null;
}
