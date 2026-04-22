using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public interface IUserService
{
    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<AppUser?> GetUserByIdAsync(Guid id);
    Task UpdateUserAsync(AppUser user);
    Task DeleteUserAsync(Guid id);
    Task LockUserAsync(Guid id);
    Task UnlockUserAsync(Guid id);
    Task<UserProfile?> GetProfileByUserIdAsync(Guid appUserId, bool includeRoles = false);
    Task UpdateProfileAsync(UserProfile profile);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser> CreateUserWithProfileAsync(AppUser user, UserProfile profile, RoleType role);
}
