using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.BLL.Dtos.Users;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class UserService(
    IAppUserRepository appUsers,
    IUserProfileRepository userProfiles,
    IRepository<UserRole> userRoles,
    IUnitOfWork uow,
    IRepository<MessageRead> messageReads,
    IRepository<Message> messages,
    IRepository<ConversationParticipant> conversationParticipants,
    IRepository<Review> reviewRepo,
    IRepository<Booking> bookingRepo,
    IRepository<ServiceListing> serviceListingRepo,
    IReviewService reviewService,
    ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
    {
        var profiles = await userProfiles.ListWithDetailsAsync();
        return await BuildUserProfileDtosAsync(profiles, includeEmail: true);
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false)
    {
        var profile = await userProfiles.GetByIdWithDetailsAsync(id);

        if (profile is null)
            return null;

        var canSeeEmail = isAdmin || (callerUserId.HasValue && callerUserId.Value == profile.AppUserId);
        return await BuildUserProfileDtoAsync(profile, canSeeEmail ? profile.AppUser?.Email : null);
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

        var profileList = await userProfiles.FindAsync(p => p.AppUserId == id);
        var profileIds = profileList.Select(p => p.Id).ToList();

        await uow.BeginTransactionAsync();
        try
        {
            if (profileIds.Count > 0)
            {
                await messageReads.ExecuteDeleteAsync(mr => profileIds.Contains(mr.UserProfileId));

                await messages.ExecuteDeleteAsync(m => profileIds.Contains(m.SenderProfileId));

                await conversationParticipants.ExecuteDeleteAsync(cp => profileIds.Contains(cp.UserProfileId));

                await reviewRepo.ExecuteDeleteAsync(r => profileIds.Contains(r.ReviewerProfileId)
                                                      || profileIds.Contains(r.ReviewedProfileId));

                await bookingRepo.ExecuteDeleteAsync(b => profileIds.Contains(b.ClientProfileId));

                var listingList = await serviceListingRepo.FindAsync(sl => profileIds.Contains(sl.UserProfileId));
                var listingIds = listingList.Select(sl => sl.Id).ToList();

                if (listingIds.Count > 0)
                {
                    await bookingRepo.ExecuteDeleteAsync(b => listingIds.Contains(b.ServiceListingId));
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
        var profile = await userProfiles.GetByAppUserIdWithDetailsAsync(appUserId);
        return profile is null ? null : await BuildUserProfileDtoAsync(profile, profile.AppUser?.Email);
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
        return await appUsers.GetByEmailWithProfilesAsync(email);
    }

    public async Task<AppUser> CreateUserWithProfileAsync(AppUser user, UserProfile profile, RoleType role)
    {
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            AppUserId = user.Id,
            Role = role
        };

        appUsers.Add(user);
        userProfiles.Add(profile);
        userRoles.Add(userRole);
        await uow.SaveChangesAsync();
        return user;
    }

    public async Task<(IEnumerable<UserProfileDto> Items, int TotalCount)> GetAllProfilesAsync(int page, int pageSize)
    {
        var (profiles, totalCount) = await userProfiles.ListPagedWithDetailsAsync(page, pageSize);
        var dtos = await BuildUserProfileDtosAsync(profiles, includeEmail: false);
        return (dtos, totalCount);
    }

    public async Task<UserProfileDto?> GetProfileByIdAsync(Guid id, Guid? callerUserId = null, bool isAdmin = false)
    {
        var profile = await userProfiles.GetByIdWithDetailsAsync(id);

        if (profile is null)
            return null;

        var canSeeEmail = isAdmin || (callerUserId.HasValue && callerUserId.Value == profile.AppUserId);
        return await BuildUserProfileDtoAsync(profile, canSeeEmail ? profile.AppUser?.Email : null);
    }

    private async Task<IEnumerable<UserProfileDto>> BuildUserProfileDtosAsync(IEnumerable<UserProfile> profiles, bool includeEmail)
    {
        var result = new List<UserProfileDto>();
        foreach (var profile in profiles)
        {
            var email = includeEmail ? profile.AppUser?.Email : null;
            result.Add(await BuildUserProfileDtoAsync(profile, email));
        }
        return result;
    }

    private async Task<UserProfileDto> BuildUserProfileDtoAsync(UserProfile profile, string? email)
    {
        var stats = await reviewService.GetRatingStatsForProfileAsync(profile.Id);
        return ToUserProfileDto(profile, email, stats);
    }

    private static UserProfileDto ToUserProfileDto(UserProfile profile, string? email, RatingStatsDto? stats = null)
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
            Roles = profile.AppUser?.Roles?.Select(r => r.Role).ToList() ?? [],
            AverageRating = stats?.AverageRating ?? 0,
            ReviewCount = stats?.ReviewCount ?? 0
        };
    }
}
