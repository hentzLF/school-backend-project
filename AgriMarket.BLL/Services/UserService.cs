using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await _db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(Guid id)
    {
        return await _db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.ClientBookings)
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.ServiceListings)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateUserAsync(AppUser user)
    {
        var existing = await _db.AppUsers.FindAsync(user.Id);
        if (existing != null)
        {
            existing.Email = user.Email;
            existing.LockoutEnd = user.LockoutEnd;
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

    public async Task<UserProfile?> GetProfileByUserIdAsync(Guid appUserId, bool includeRoles = false)
    {
        var query = _db.UserProfiles.AsQueryable();
        if (includeRoles)
            query = query.Include(p => p.Roles);
        return await query.FirstOrDefaultAsync(p => p.AppUserId == appUserId);
    }

    public async Task UpdateProfileAsync(UserProfile profile)
    {
        _db.UserProfiles.Update(profile);
        await _db.SaveChangesAsync();
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<AppUser> CreateUserWithProfileAsync(AppUser user, UserProfile profile, AgriMarket.Domain.Enums.RoleType role)
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

    public async Task<(IEnumerable<UserProfile> Items, int TotalCount)> GetAllProfilesAsync(int page, int pageSize)
    {
        var query = _db.UserProfiles.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (items, totalCount);
    }

    public async Task<UserProfile?> GetProfileByIdAsync(Guid id)
    {
        return await _db.UserProfiles.AsNoTracking()
            .Include(up => up.AppUser)
            .FirstOrDefaultAsync(up => up.Id == id);
    }
}
