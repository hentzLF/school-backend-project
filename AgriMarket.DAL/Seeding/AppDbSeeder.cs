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
        await SeedAdminIfMissing(context, passwordHasher);
        await SeedClientUserIfMissing(context, passwordHasher,
            "provider@agrimarket.ee", "Provider123!", "Jaan", "Tamm");
        await SeedClientUserIfMissing(context, passwordHasher,
            "farmer@agrimarket.ee", "Farmer123!", "Mari", "Mets");
    }

    private static async Task SeedAdminIfMissing(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.AppUsers.AnyAsync(u => u.Email == "admin@agrimarket.ee"))
            return;

        var user = CreateUser("admin@agrimarket.ee", "Admin123!", passwordHasher);
        var profile = CreateProfile("Admin", "AgriMarket", user.Id);

        context.AppUsers.Add(user);
        context.UserProfiles.Add(profile);
        context.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), AppUserId = user.Id, Role = RoleType.Admin });

        await context.SaveChangesAsync();
    }

    private static async Task SeedClientUserIfMissing(
        AppDbContext context, IPasswordHasher passwordHasher,
        string email, string password, string firstName, string lastName)
    {
        if (await context.AppUsers.AnyAsync(u => u.Email == email))
            return;

        var user = CreateUser(email, password, passwordHasher);
        var profile = CreateProfile(firstName, lastName, user.Id);

        context.AppUsers.Add(user);
        context.UserProfiles.Add(profile);
        context.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), AppUserId = user.Id, Role = RoleType.Farmer });
        context.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), AppUserId = user.Id, Role = RoleType.Provider });

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
}
