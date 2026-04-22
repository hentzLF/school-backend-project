using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user, UserProfile profile, RoleType role);
    string GenerateSessionToken(Guid userId);
    string GenerateRefreshToken();
    Guid? ValidateSessionToken(string token);
}
