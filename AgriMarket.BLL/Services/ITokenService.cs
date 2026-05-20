using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user, UserProfile profile, IEnumerable<RoleType> roles);
    string GenerateRefreshToken();
}
