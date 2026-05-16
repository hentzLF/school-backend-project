using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    public static (AppUser user, UserProfile profile) SeedClientUser(
        AppDbContext db, string email, string password, RoleType role)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            AppUserId = user.Id
        };
        var profileRole = new ProfileRole
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Role = role
        };
        db.AppUsers.Add(user);
        db.UserProfiles.Add(profile);
        db.ProfileRoles.Add(profileRole);
        db.SaveChanges();
        return (user, profile);
    }

    public static void EnsureServiceCategory(AppDbContext db)
    {
        var categoryId = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001");
        if (!db.ServiceCategories.Any(c => c.Id == categoryId))
        {
            db.ServiceCategories.Add(new ServiceCategory
            {
                Id = categoryId,
                Name = "Test Category"
            });
            db.SaveChanges();
        }
    }

    public static (ServiceListing listing, Availability availability) SeedListing(
        AppDbContext db, Guid providerProfileId)
    {
        EnsureServiceCategory(db);
        var categoryId = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001");
        var listing = new ServiceListing
        {
            Id = Guid.NewGuid(),
            Title = "Test Service",
            PricePerHectare = 50m,
            IsActive = true,
            UserProfileId = providerProfileId,
            ServiceCategoryId = categoryId
        };
        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            ServiceListingId = listing.Id,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(2),
            IsBooked = false
        };
        db.ServiceListings.Add(listing);
        db.Availabilities.Add(availability);
        db.SaveChanges();
        return (listing, availability);
    }

    public static Booking SeedBooking(
        AppDbContext db, Guid clientProfileId, Guid listingId, Guid availabilityId,
        BookingStatus status = BookingStatus.Pending)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ClientProfileId = clientProfileId,
            ServiceListingId = listingId,
            AvailabilityId = availabilityId,
            Status = status,
            TotalPrice = 100m,
            AreaInHectares = 1.0m,
            CreatedAt = DateTime.UtcNow
        };
        db.Bookings.Add(booking);
        db.SaveChanges();
        return booking;
    }
}
