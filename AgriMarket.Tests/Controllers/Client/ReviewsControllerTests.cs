using AgriMarket.BLL.Services;
using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels.Reviews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class ReviewsControllerTests
{
    private static BookingService CreateBookingService(AppDbContext db) =>
        new(new EfBookingRepository(db),
            new EfRepository<UserProfile>(db),
            new EfRepository<ServiceListing>(db),
            new EfRepository<Availability>(db),
            new EfRepository<Payment>(db),
            new EfUnitOfWork(db),
            NullLogger<BookingService>.Instance);

    private static UserService CreateUserService(AppDbContext db) =>
        new(new EfAppUserRepository(db),
            new EfUserProfileRepository(db),
            new EfRepository<UserRole>(db),
            new EfUnitOfWork(db),
            new EfRepository<MessageRead>(db),
            new EfRepository<Message>(db),
            new EfRepository<ConversationParticipant>(db),
            new EfRepository<Review>(db),
            new EfRepository<Booking>(db),
            new EfRepository<ServiceListing>(db),
            TestServiceFactory.CreateReviewService(db),
            NullLogger<UserService>.Instance);

    private static ReviewsController CreateController(AppDbContext db, Guid userId)
    {
        var controller = new ReviewsController(
            TestServiceFactory.CreateReviewService(db),
            CreateBookingService(db),
            CreateUserService(db));
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(userId);
        return controller;
    }

    [Fact]
    public async Task CreateGet_WithValidBooking_ReturnsView()
    {
        using var db = TestDbContextFactory.Create(nameof(CreateGet_WithValidBooking_ReturnsView));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ClientConfirmed);

        var controller = CreateController(db, user.Id);

        var result = await controller.Create(booking.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<CreateReviewViewModel>(viewResult.Model);
        Assert.Equal(booking.Id, vm.BookingId);
    }

    [Fact]
    public async Task CreatePost_WithValidReview_RedirectsToBookingDetails()
    {
        using var db = TestDbContextFactory.Create(nameof(CreatePost_WithValidReview_RedirectsToBookingDetails));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (otherUser, otherProfile) = TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Provider);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, otherProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ClientConfirmed);

        var controller = CreateController(db, user.Id);

        var model = new CreateReviewViewModel
        {
            BookingId = booking.Id,
            Rating = 4,
            Comment = "Great service!",
            BookingTitle = "Test"
        };

        var result = await controller.Create(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("Bookings", redirect.ControllerName);
    }

    [Fact]
    public async Task CreatePost_WithInvalidBookingState_RedirectsWithError()
    {
        using var db = TestDbContextFactory.Create(nameof(CreatePost_WithInvalidBookingState_RedirectsWithError));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.Pending);

        var controller = CreateController(db, user.Id);

        var model = new CreateReviewViewModel
        {
            BookingId = booking.Id,
            Rating = 4,
            Comment = "Should fail",
            BookingTitle = "Test"
        };

        var result = await controller.Create(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("Bookings", redirect.ControllerName);
    }

    [Fact]
    public async Task EditGet_WithExistingReview_ReturnsView()
    {
        using var db = TestDbContextFactory.Create(nameof(EditGet_WithExistingReview_ReturnsView));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (otherUser, otherProfile) = TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Provider);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, otherProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ClientConfirmed);
        var review = SeedReview(db, booking.Id, profile.Id, otherProfile.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.Edit(review.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<EditReviewViewModel>(viewResult.Model);
        Assert.Equal(review.Id, vm.ReviewId);
    }

    [Fact]
    public async Task EditGet_WithNonExistentReview_ReturnsNotFound()
    {
        using var db = TestDbContextFactory.Create(nameof(EditGet_WithNonExistentReview_ReturnsNotFound));
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);

        var controller = CreateController(db, user.Id);

        var result = await controller.Edit(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteGet_WithExistingReview_ReturnsView()
    {
        using var db = TestDbContextFactory.Create(nameof(DeleteGet_WithExistingReview_ReturnsView));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (otherUser, otherProfile) = TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Provider);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, otherProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ClientConfirmed);
        var review = SeedReview(db, booking.Id, profile.Id, otherProfile.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.Delete(review.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<DeleteReviewViewModel>(viewResult.Model);
        Assert.Equal(review.Id, vm.ReviewId);
    }

    [Fact]
    public async Task DeletePost_WithExistingReview_DeletesAndRedirects()
    {
        using var db = TestDbContextFactory.Create(nameof(DeletePost_WithExistingReview_DeletesAndRedirects));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (otherUser, otherProfile) = TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Provider);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, otherProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ClientConfirmed);
        var review = SeedReview(db, booking.Id, profile.Id, otherProfile.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.DeleteConfirmed(review.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("Bookings", redirect.ControllerName);

        var deleted = await db.Reviews.FindAsync(review.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task ForProvider_WithReviews_ReturnsViewWithList()
    {
        using var db = TestDbContextFactory.Create(nameof(ForProvider_WithReviews_ReturnsViewWithList));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (providerUser, providerProfile) = TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Provider);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, providerProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ClientConfirmed);
        SeedReview(db, booking.Id, profile.Id, providerProfile.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.ForProvider(providerProfile.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<ReviewListViewModel>(viewResult.Model);
        Assert.Single(vm.Reviews);
    }

    [Fact]
    public async Task ForProvider_WithNoReviews_ReturnsEmptyView()
    {
        using var db = TestDbContextFactory.Create(nameof(ForProvider_WithNoReviews_ReturnsEmptyView));
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (providerUser, providerProfile) = TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pw", RoleType.Provider);

        var controller = CreateController(db, user.Id);

        var result = await controller.ForProvider(providerProfile.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<ReviewListViewModel>(viewResult.Model);
        Assert.Empty(vm.Reviews);
    }

    private static Review SeedReview(AppDbContext db, Guid bookingId, Guid reviewerProfileId, Guid reviewedProfileId)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            ReviewerProfileId = reviewerProfileId,
            ReviewedProfileId = reviewedProfileId,
            Rating = 4,
            Comment = "Good service",
            CreatedAt = DateTime.UtcNow
        };
        db.Reviews.Add(review);
        db.SaveChanges();
        return review;
    }
}
