using AgriMarket.Api.Dtos.Auth;

namespace AgriMarket.Api.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<TokenResponse> SelectProfileAsync(SelectProfileRequest request);
    Task<TokenResponse> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
