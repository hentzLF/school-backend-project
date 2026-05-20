using AgriMarket.BLL.Contracts;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL.Seeding;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        await SeedServiceCategoriesAsync(context);
        await SeedUsersAsync(context, passwordHasher);
    }

    private static async Task SeedServiceCategoriesAsync(AppDbContext context)
    {
        if (await context.ServiceCategories.AnyAsync())
            return;

        context.ServiceCategories.AddRange(
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"), Name = "Hay Baling", Description = "Round and square baling services" },
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0002-0000-0000-000000000002"), Name = "Combine Harvesting", Description = "Grain and cereal harvesting" },
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0003-0000-0000-000000000003"), Name = "Spraying", Description = "Crop protection and fertilizer spraying" },
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0004-0000-0000-000000000004"), Name = "Soil Preparation", Description = "Ploughing, discing, and cultivating" },
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0005-0000-0000-000000000005"), Name = "Seeding", Description = "Precision and broadcast seeding" },
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0006-0000-0000-000000000006"), Name = "Mowing", Description = "Grass and hay mowing services" },
            new ServiceCategory { Id = Guid.Parse("a1b2c3d4-0007-0000-0000-000000000007"), Name = "Transport", Description = "Agricultural cargo transport" }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        await SeedUserIfMissing(context, passwordHasher,
            "admin@agrimarket.ee", "Admin123!", "Admin", "AgriMarket", RoleType.Admin);
        await SeedUserIfMissing(context, passwordHasher,
            "provider@agrimarket.ee", "Provider123!", "Jaan", "Tamm", RoleType.Provider);
        await SeedUserIfMissing(context, passwordHasher,
            "farmer@agrimarket.ee", "Farmer123!", "Mari", "Mets", RoleType.Farmer);
    }

    private static async Task SeedUserIfMissing(
        AppDbContext context, IPasswordHasher passwordHasher,
        string email, string password, string firstName, string lastName, RoleType role)
    {
        if (await context.AppUsers.AnyAsync(u => u.Email == email))
            return;

        var user = CreateUser(email, password, passwordHasher);
        var profile = CreateProfile(firstName, lastName, user.Id);
        var profileRole = CreateRole(profile.Id, role);

        context.AppUsers.Add(user);
        context.UserProfiles.Add(profile);
        context.ProfileRoles.Add(profileRole);

        await context.SaveChangesAsync();
    }

    private static AppUser CreateUser(string email, string password, IPasswordHasher hasher) => new()
    {
        Id = Guid.NewGuid(),
        Email = email,
        PasswordHash = hasher.Hash(password),
        CreatedAt = DateTime.UtcNow
    };

    private static UserProfile CreateProfile(string firstName, string lastName, Guid appUserId) => new()
    {
        Id = Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        AppUserId = appUserId
    };

    private static ProfileRole CreateRole(Guid profileId, RoleType role) => new()
    {
        Id = Guid.NewGuid(),
        UserProfileId = profileId,
        Role = role
    };
}
