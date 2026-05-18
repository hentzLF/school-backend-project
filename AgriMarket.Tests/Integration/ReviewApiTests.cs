using AgriMarket.BLL;
using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.BLL.Services;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using Xunit;

namespace AgriMarket.Tests.Integration;

public class ReviewApiTests
{
    private static (ReviewService service, AppDbContext db) CreateServiceWithDb(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        return (TestServiceFactory.CreateReviewService(db), db);
    }

    private static (AppUser user, UserProfile profile) SeedProvider(AppDbContext db)
        => TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Farmer);

    private static (AppUser user, UserProfile profile) SeedClient(AppDbContext db)
        => TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);

    private static Booking SeedCompletedBooking(
        AppDbContext db, Guid clientProfileId, Guid providerProfileId,
        BookingStatus status = BookingStatus.ClientConfirmed)
    {
        var (listing, availability) = TestDbContextFactory.SeedListing(db, providerProfileId);
        return TestDbContextFactory.SeedBooking(db, clientProfileId, listing.Id, availability.Id, status);
    }

    [Fact]
    public async Task CreateAndGetById_FullLifecycle()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAndGetById_FullLifecycle));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);

        var created = await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking.Id, Rating = 4, Comment = "Great work" });

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(4, created.Rating);
        Assert.Equal("Great work", created.Comment);
        Assert.Equal(booking.Id, created.BookingId);
        Assert.Equal(clientProfile.Id, created.ReviewerProfileId);

        var fetched = await service.GetByIdAsync(created.Id);

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(4, fetched.Rating);
    }

    [Fact]
    public async Task CreateAsync_ProviderCompletedBooking_Succeeds()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_ProviderCompletedBooking_Succeeds));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id, BookingStatus.ProviderCompleted);

        var result = await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking.Id, Rating = 5 });

        Assert.Equal(5, result.Rating);
    }

    [Fact]
    public async Task CreateAsync_PendingBooking_ThrowsBusinessRuleException()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_PendingBooking_ThrowsBusinessRuleException));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id, BookingStatus.Pending);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateAsync(clientUser.Id,
                new CreateReviewDto { BookingId = booking.Id, Rating = 3 }));
    }

    [Fact]
    public async Task CreateAsync_UnrelatedUser_ThrowsBusinessRuleException()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_UnrelatedUser_ThrowsBusinessRuleException));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (_, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        var (outsiderUser, _) = TestDbContextFactory.SeedClientUser(db, "outsider@test.com", "pw", RoleType.Farmer);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateAsync(outsiderUser.Id,
                new CreateReviewDto { BookingId = booking.Id, Rating = 3 }));

        Assert.Equal("Only the client can review a booking.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_NonExistentBooking_ThrowsKeyNotFoundException()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_NonExistentBooking_ThrowsKeyNotFoundException));
        using var _ = db;
        var (clientUser, _) = SeedClient(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateAsync(clientUser.Id,
                new CreateReviewDto { BookingId = Guid.NewGuid(), Rating = 3 }));
    }

    [Fact]
    public async Task CreateAsync_NoUserProfile_ThrowsBusinessRuleException()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_NoUserProfile_ThrowsBusinessRuleException));
        using var _ = db;
        var fakeUserId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateAsync(fakeUserId,
                new CreateReviewDto { BookingId = Guid.NewGuid(), Rating = 3 }));

        Assert.Equal("User profile not found.", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_MultipleReviews_ReturnsPaginatedAndOrdered()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetAllAsync_MultipleReviews_ReturnsPaginatedAndOrdered));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);

        var booking1 = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking1.Id, Rating = 3, Comment = "First" });

        var booking2 = SeedSecondBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking2.Id, Rating = 5, Comment = "Second" });

        var (items, totalCount) = await service.GetAllAsync(1, 10);
        var list = items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Equal(2, list.Count);
        Assert.Equal("Second", list[0].Comment);
        Assert.Equal("First", list[1].Comment);
    }

    [Fact]
    public async Task GetAllAsync_Pagination_RespectsPageAndPageSize()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetAllAsync_Pagination_RespectsPageAndPageSize));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);

        var booking1 = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking1.Id, Rating = 3 });

        var booking2 = SeedSecondBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking2.Id, Rating = 5 });

        var (page1Items, totalCount) = await service.GetAllAsync(1, 1);
        var page1 = page1Items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Single(page1);

        var (page2Items, _) = await service.GetAllAsync(2, 1);
        var page2 = page2Items.ToList();

        Assert.Single(page2);
        Assert.NotEqual(page1[0].Id, page2[0].Id);
        Assert.Equal(5, page1[0].Rating);
        Assert.Equal(3, page2[0].Rating);
    }

    [Fact]
    public async Task GetByBookingAsync_ReturnsOnlyMatchingBooking()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetByBookingAsync_ReturnsOnlyMatchingBooking));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);

        var booking1 = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking1.Id, Rating = 4 });

        var booking2 = SeedSecondBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking2.Id, Rating = 2 });

        var result = await service.GetByBookingAsync(booking1.Id);

        Assert.NotNull(result);
        Assert.Equal(booking1.Id, result!.BookingId);
        Assert.Equal(4, result.Rating);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentReview_ReturnsNull()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetByIdAsync_NonExistentReview_ReturnsNull));
        using var _ = db;

        var result = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_NoReviews_ReturnsEmptyWithZeroCount()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetAllAsync_NoReviews_ReturnsEmptyWithZeroCount));
        using var _ = db;

        var (items, totalCount) = await service.GetAllAsync(1, 10);

        Assert.Equal(0, totalCount);
        Assert.Empty(items);
    }

    [Fact]
    public async Task UpdateAsync_OwnerUpdatesReview_Succeeds()
    {
        var (service, db) = CreateServiceWithDb(nameof(UpdateAsync_OwnerUpdatesReview_Succeeds));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);

        var created = await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking.Id, Rating = 3, Comment = "OK" });

        var updated = await service.UpdateAsync(clientUser.Id,
            new UpdateReviewDto { Id = created.Id, Rating = 5, Comment = "Actually great" });

        Assert.Equal(5, updated.Rating);
        Assert.Equal("Actually great", updated.Comment);
        Assert.Equal(created.Id, updated.Id);
    }

    [Fact]
    public async Task DeleteAsync_OwnerDeletesReview_Succeeds()
    {
        var (service, db) = CreateServiceWithDb(nameof(DeleteAsync_OwnerDeletesReview_Succeeds));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);

        var created = await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking.Id, Rating = 4 });

        await service.DeleteAsync(clientUser.Id, created.Id);

        var fetched = await service.GetByIdAsync(created.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task CreateAsync_DuplicateReview_ThrowsBusinessRuleException()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_DuplicateReview_ThrowsBusinessRuleException));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);

        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking.Id, Rating = 4 });

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateAsync(clientUser.Id,
                new CreateReviewDto { BookingId = booking.Id, Rating = 5 }));

        Assert.Equal("A review already exists for this booking.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ProviderAttempts_ThrowsBusinessRuleException()
    {
        var (service, db) = CreateServiceWithDb(nameof(CreateAsync_ProviderAttempts_ThrowsBusinessRuleException));
        using var _ = db;
        var (providerUser, providerProfile) = SeedProvider(db);
        var (_, clientProfile) = SeedClient(db);
        var booking = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateAsync(providerUser.Id,
                new CreateReviewDto { BookingId = booking.Id, Rating = 5 }));

        Assert.Equal("Only the client can review a booking.", ex.Message);
    }

    [Fact]
    public async Task GetByProfileAsync_ReturnsOnlyReviewsForProfile()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetByProfileAsync_ReturnsOnlyReviewsForProfile));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);

        var booking1 = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking1.Id, Rating = 4, Comment = "Good" });

        var booking2 = SeedSecondBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking2.Id, Rating = 5, Comment = "Great" });

        var (items, totalCount) = await service.GetByProfileAsync(providerProfile.Id, 1, 10);
        var list = items.ToList();

        Assert.Equal(2, totalCount);
        Assert.Equal(2, list.Count);
        Assert.All(list, r => Assert.Equal(providerProfile.Id, r.ReviewedProfileId));
    }

    [Fact]
    public async Task GetByProfileAsync_NoReviews_ReturnsEmpty()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetByProfileAsync_NoReviews_ReturnsEmpty));
        using var _ = db;

        var (items, totalCount) = await service.GetByProfileAsync(Guid.NewGuid(), 1, 10);

        Assert.Equal(0, totalCount);
        Assert.Empty(items);
    }

    [Fact]
    public async Task GetRatingStatsForProfileAsync_WithReviews_ReturnsCorrectStats()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetRatingStatsForProfileAsync_WithReviews_ReturnsCorrectStats));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);

        var booking1 = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking1.Id, Rating = 3 });

        var booking2 = SeedSecondBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking2.Id, Rating = 5 });

        var stats = await service.GetRatingStatsForProfileAsync(providerProfile.Id);

        Assert.Equal(4.0, stats.AverageRating);
        Assert.Equal(2, stats.ReviewCount);
    }

    [Fact]
    public async Task GetRatingStatsForProfileAsync_NoReviews_ReturnsZero()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetRatingStatsForProfileAsync_NoReviews_ReturnsZero));
        using var _ = db;

        var stats = await service.GetRatingStatsForProfileAsync(Guid.NewGuid());

        Assert.Equal(0, stats.AverageRating);
        Assert.Equal(0, stats.ReviewCount);
    }

    [Fact]
    public async Task GetRatingStatsForListingAsync_WithReviews_ReturnsCorrectStats()
    {
        var (service, db) = CreateServiceWithDb(nameof(GetRatingStatsForListingAsync_WithReviews_ReturnsCorrectStats));
        using var _ = db;
        var (_, providerProfile) = SeedProvider(db);
        var (clientUser, clientProfile) = SeedClient(db);

        var booking1 = SeedCompletedBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking1.Id, Rating = 4 });

        var booking2 = SeedSecondBooking(db, clientProfile.Id, providerProfile.Id);
        await service.CreateAsync(clientUser.Id,
            new CreateReviewDto { BookingId = booking2.Id, Rating = 5 });

        var listingId = db.ServiceListings.First(l => l.UserProfileId == providerProfile.Id).Id;
        var stats = await service.GetRatingStatsForListingAsync(listingId);

        Assert.Equal(4.5, stats.AverageRating);
        Assert.Equal(2, stats.ReviewCount);
    }

    private static Booking SeedSecondBooking(
        AppDbContext db, Guid clientProfileId, Guid providerProfileId)
    {
        var listing = db.ServiceListings.First(l => l.UserProfileId == providerProfileId)
            ?? throw new InvalidOperationException("Provider listing must be seeded first.");
        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            ServiceListingId = listing.Id,
            StartTime = DateTime.UtcNow.AddDays(5),
            EndTime = DateTime.UtcNow.AddDays(6),
            IsBooked = false
        };
        db.Availabilities.Add(availability);
        db.SaveChanges();

        return TestDbContextFactory.SeedBooking(
            db, clientProfileId, listing.Id, availability.Id, BookingStatus.ClientConfirmed);
    }
}
