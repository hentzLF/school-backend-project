using AgriMarket.BLL.Dtos.Auth;

namespace AgriMarket.BLL.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<TokenResponse> SelectProfileAsync(SelectProfileRequest request);
    Task<TokenResponse> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
