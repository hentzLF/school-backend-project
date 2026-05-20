using AgriMarket.BLL.Dtos.Auth;

namespace AgriMarket.BLL.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
