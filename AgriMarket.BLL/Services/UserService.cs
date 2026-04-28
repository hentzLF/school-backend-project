using AgriMarket.BLL.Dtos.Users;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class UserService(
    IRepository<AppUser> appUsers,
    IRepository<UserProfile> userProfiles,
    IRepository<ProfileRole> profileRoles,
    IUnitOfWork uow,
    AppDbContext db,
    ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
    {
        var profiles = await userProfiles.Query()
            .AsNoTracking()
            .Include(p => p.AppUser)
            .Include(p => p.Roles)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return profiles.Select(p => ToUserProfileDto(p, p.AppUser?.Email));
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false)
    {
        var profile = await userProfiles.Query()
            .AsNoTracking()
            .Include(p => p.AppUser)
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return null;

        var canSeeEmail = isAdmin || (callerUserId.HasValue && callerUserId.Value == profile.AppUserId);
        return ToUserProfileDto(profile, canSeeEmail ? profile.AppUser?.Email : null);
    }

    public async Task UpdateUserAsync(Guid appUserId, string email, DateTime? lockoutEnd)
    {
        var existing = await appUsers.GetByIdAsync(appUserId)
            ?? throw new KeyNotFoundException($"AppUser {appUserId} not found.");
        existing.Email = email;
        existing.LockoutEnd = lockoutEnd;
        await uow.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await appUsers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"AppUser {id} not found.");

        var profileIds = await userProfiles.Query()
            .Where(p => p.AppUserId == id)
            .Select(p => p.Id)
            .ToListAsync();

        await uow.BeginTransactionAsync();
        try
        {
            if (profileIds.Count > 0)
            {
                await db.Notifications
                    .Where(n => profileIds.Contains(n.UserProfileId))
                    .ExecuteDeleteAsync();

                await db.MessageReads
                    .Where(mr => profileIds.Contains(mr.UserProfileId))
                    .ExecuteDeleteAsync();

                await db.Messages
                    .Where(m => profileIds.Contains(m.SenderProfileId))
                    .ExecuteDeleteAsync();

                await db.ConversationParticipants
                    .Where(cp => profileIds.Contains(cp.UserProfileId))
                    .ExecuteDeleteAsync();

                await db.Reviews
                    .Where(r => profileIds.Contains(r.ReviewerProfileId)
                             || profileIds.Contains(r.ReviewedProfileId))
                    .ExecuteDeleteAsync();

                await db.Bookings
                    .Where(b => profileIds.Contains(b.ClientProfileId))
                    .ExecuteDeleteAsync();

                var listingIds = await db.ServiceListings
                    .Where(sl => profileIds.Contains(sl.UserProfileId))
                    .Select(sl => sl.Id)
                    .ToListAsync();

                if (listingIds.Count > 0)
                {
                    await db.Bookings
                        .Where(b => listingIds.Contains(b.ServiceListingId))
                        .ExecuteDeleteAsync();
                }
            }

            appUsers.Remove(user);
            await uow.SaveChangesAsync();
            await uow.CommitTransactionAsync();
        }
        catch
        {
            await uow.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task LockUserAsync(Guid id)
    {
        var user = await appUsers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"AppUser {id} not found.");
        user.LockoutEnd = DateTime.MaxValue;
        await uow.SaveChangesAsync();
    }

    public async Task UnlockUserAsync(Guid id)
    {
        var user = await appUsers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"AppUser {id} not found.");
        user.LockoutEnd = null;
        await uow.SaveChangesAsync();
    }

    public async Task<UserProfileDto?> GetProfileByUserIdAsync(Guid appUserId)
    {
        var profile = await userProfiles.Query()
            .AsNoTracking()
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.AppUserId == appUserId);

        return profile is null ? null : ToUserProfileDto(profile, profile.AppUser?.Email);
    }

    public async Task UpdateProfileAsync(UserProfileDto profile)
    {
        var existing = await userProfiles.GetByIdAsync(profile.Id);
        if (existing is null)
            throw new KeyNotFoundException($"UserProfile {profile.Id} not found.");

        existing.FirstName = profile.FirstName;
        existing.LastName = profile.LastName;
        existing.Bio = profile.Bio;
        existing.AvatarUrl = profile.AvatarUrl;
        await uow.SaveChangesAsync();
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await appUsers.Query()
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

        appUsers.Add(user);
        userProfiles.Add(profile);
        profileRoles.Add(profileRole);
        await uow.SaveChangesAsync();
        return user;
    }

    public async Task<(IEnumerable<UserProfileDto> Items, int TotalCount)> GetAllProfilesAsync(int page, int pageSize)
    {
        var query = userProfiles.Query().AsNoTracking();
        var totalCount = await query.CountAsync();
        var profiles = await query
            .Include(p => p.Roles)
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (profiles.Select(p => ToUserProfileDto(p, null)), totalCount);
    }

    public async Task<UserProfileDto?> GetProfileByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false)
    {
        var profile = await userProfiles.Query().AsNoTracking()
            .Include(up => up.AppUser)
            .Include(up => up.Roles)
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
            Email = email,
            CreatedAt = profile.AppUser?.CreatedAt ?? default,
            IsLocked = profile.AppUser?.LockoutEnd > DateTime.UtcNow,
            LockoutEnd = profile.AppUser?.LockoutEnd,
            Roles = profile.Roles?.Select(r => r.Role).ToList() ?? []
        };
    }
}
