using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL.Seeding;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.AppUsers.AnyAsync(u => u.Email == "admin@agrimarket.ee"))
            return;

        var adminUser = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@agrimarket.ee",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            CreatedAt = DateTime.UtcNow
        };

        var adminProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            FirstName = "Admin",
            LastName = "AgriMarket",
            AppUserId = adminUser.Id
        };

        var adminRole = new ProfileRole
        {
            Id = Guid.NewGuid(),
            UserProfileId = adminProfile.Id,
            Role = RoleType.Admin
        };

        context.AppUsers.Add(adminUser);
        context.UserProfiles.Add(adminProfile);
        context.ProfileRoles.Add(adminRole);

        await context.SaveChangesAsync();
    }
}
