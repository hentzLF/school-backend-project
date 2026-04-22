using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.BLL.Dtos.Users;

namespace AgriMarket.BLL.Services;

public interface IUserService
{
    Task<IEnumerable<UserProfileDto>> GetAllUsersAsync();
    Task<UserProfileDto?> GetUserByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false);
    Task UpdateUserAsync(Guid appUserId, string email, DateTime? lockoutEnd);
    Task DeleteUserAsync(Guid id);
    Task LockUserAsync(Guid id);
    Task UnlockUserAsync(Guid id);
    Task<UserProfileDto?> GetProfileByUserIdAsync(Guid appUserId);
    Task UpdateProfileAsync(UserProfileDto profile);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser> CreateUserWithProfileAsync(AppUser user, UserProfile profile, RoleType role);
    Task<(IEnumerable<UserProfileDto> Items, int TotalCount)> GetAllProfilesAsync(int page, int pageSize);
    Task<UserProfileDto?> GetProfileByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false);
}
