using AgriMarket.BLL.Dtos.Users;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
    {
        var profiles = await _db.UserProfiles
            .AsNoTracking()
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return profiles.Select(p => ToUserProfileDto(p, null));
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false)
    {
        var profile = await _db.UserProfiles
            .AsNoTracking()
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return null;

        var canSeeEmail = isAdmin || (callerUserId.HasValue && callerUserId.Value == profile.AppUserId);
        return ToUserProfileDto(profile, canSeeEmail ? profile.AppUser?.Email : null);
    }

    public async Task UpdateUserAsync(Guid appUserId, string email, DateTime? lockoutEnd)
    {
        var existing = await _db.AppUsers.FindAsync(appUserId);
        if (existing != null)
        {
            existing.Email = email;
            existing.LockoutEnd = lockoutEnd;
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user != null)
        {
            _db.AppUsers.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    public async Task LockUserAsync(Guid id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user != null)
        {
            user.LockoutEnd = DateTime.UtcNow.AddYears(100);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UnlockUserAsync(Guid id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user != null)
        {
            user.LockoutEnd = null;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<UserProfileDto?> GetProfileByUserIdAsync(Guid appUserId)
    {
        var profile = await _db.UserProfiles
            .AsNoTracking()
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.AppUserId == appUserId);

        return profile is null ? null : ToUserProfileDto(profile, profile.AppUser?.Email);
    }

    public async Task UpdateProfileAsync(UserProfileDto profile)
    {
        var existing = await _db.UserProfiles.FindAsync(profile.Id);
        if (existing is null)
            throw new KeyNotFoundException($"UserProfile {profile.Id} not found.");

        existing.FirstName = profile.FirstName;
        existing.LastName = profile.LastName;
        existing.Bio = profile.Bio;
        existing.AvatarUrl = profile.AvatarUrl;
        await _db.SaveChangesAsync();
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<AppUser> CreateUserWithProfileAsync(AppUser user, UserProfile profile, RoleType role)
    {
        var profileRole = new ProfileRole
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Role = role
        };

        _db.AppUsers.Add(user);
        _db.UserProfiles.Add(profile);
        _db.ProfileRoles.Add(profileRole);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<(IEnumerable<UserProfileDto> Items, int TotalCount)> GetAllProfilesAsync(int page, int pageSize)
    {
        var query = _db.UserProfiles.AsNoTracking();
        var totalCount = await query.CountAsync();
        var profiles = await query
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (profiles.Select(p => ToUserProfileDto(p, null)), totalCount);
    }

    public async Task<UserProfileDto?> GetProfileByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false)
    {
        var profile = await _db.UserProfiles.AsNoTracking()
            .Include(up => up.AppUser)
            .FirstOrDefaultAsync(up => up.Id == id);

        if (profile is null)
            return null;

        var canSeeEmail = isAdmin || (callerUserId.HasValue && callerUserId.Value == profile.AppUserId);
        return ToUserProfileDto(profile, canSeeEmail ? profile.AppUser?.Email : null);
    }

    private static UserProfileDto ToUserProfileDto(UserProfile profile, string? email)
    {
        return new UserProfileDto
        {
            Id = profile.Id,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            AppUserId = profile.AppUserId,
            Email = email
        };
    }
}
