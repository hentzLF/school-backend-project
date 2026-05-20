using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class BookingsControllerTests
{
    private static AgriMarket.BLL.Services.BookingService CreateBookingService(AppDbContext db) =>
        new(new EfBookingRepository(db),
            new EfRepository<UserProfile>(db),
            new EfRepository<ServiceListing>(db),
            new EfRepository<Availability>(db),
            new EfRepository<Payment>(db),
            new EfUnitOfWork(db),
            NullLogger<AgriMarket.BLL.Services.BookingService>.Instance);

    private static AgriMarket.BLL.Services.UserService CreateUserService(AppDbContext db) =>
        new(new EfAppUserRepository(db),
            new EfUserProfileRepository(db),
            new EfRepository<ProfileRole>(db),
            new EfUnitOfWork(db),
            new EfRepository<MessageRead>(db),
            new EfRepository<Message>(db),
            new EfRepository<ConversationParticipant>(db),
            new EfRepository<Review>(db),
            new EfRepository<Booking>(db),
            new EfRepository<ServiceListing>(db),
            TestServiceFactory.CreateReviewService(db),
            NullLogger<AgriMarket.BLL.Services.UserService>.Instance);

    private static BookingsController CreateController(AppDbContext db, Guid userId)
    {
        var controller = new BookingsController(
            CreateBookingService(db),
            CreateUserService(db),
            TestServiceFactory.CreateClientPaymentService(db),
            TestServiceFactory.CreateReviewService(db));
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(userId);
        return controller;
    }

    [Fact]
    public async Task Details_WithDifferentOwner_RedirectsToAccessDenied()
    {
        using var db = TestDbContextFactory.Create(nameof(Details_WithDifferentOwner_RedirectsToAccessDenied));
        var (ownerUser, ownerProfile) = TestDbContextFactory.SeedClientUser(db, "owner@test.com", "pw", RoleType.Farmer);
        var (requestorUser, _) = TestDbContextFactory.SeedClientUser(db, "requestor@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, ownerProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, ownerProfile.Id, listing.Id, availability.Id);

        var controller = CreateController(db, requestorUser.Id);

        var result = await controller.Details(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AccessDenied", redirect.ActionName);
    }

    [Fact]
    public async Task Details_WithCorrectOwner_ReturnsView()
    {
        using var db = TestDbContextFactory.Create(nameof(Details_WithCorrectOwner_ReturnsView));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "owner@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.Details(booking.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task ConfirmCompletion_WhenNotProviderCompleted_RedirectsToDetails()
    {
        using var db = TestDbContextFactory.Create(nameof(ConfirmCompletion_WhenNotProviderCompleted_RedirectsToDetails));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.Confirmed);

        var controller = CreateController(db, user.Id);

        var result = await controller.ConfirmCompletion(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);

        var unchanged = await db.Bookings.FindAsync(booking.Id);
        Assert.Equal(BookingStatus.Confirmed, unchanged!.Status);
    }

    [Fact]
    public async Task ConfirmCompletion_WhenProviderCompleted_TransitionsToClientConfirmed()
    {
        using var db = TestDbContextFactory.Create(nameof(ConfirmCompletion_WhenProviderCompleted_TransitionsToClientConfirmed));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.ProviderCompleted);

        var controller = CreateController(db, user.Id);

        var result = await controller.ConfirmCompletion(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);

        var updated = await db.Bookings.FindAsync(booking.Id);
        Assert.Equal(BookingStatus.ClientConfirmed, updated!.Status);
    }

    [Fact]
    public async Task ConfirmCompletion_WithDifferentOwner_RedirectsToAccessDenied()
    {
        using var db = TestDbContextFactory.Create(nameof(ConfirmCompletion_WithDifferentOwner_RedirectsToAccessDenied));
        var (ownerUser, ownerProfile) = TestDbContextFactory.SeedClientUser(db, "owner@test.com", "pw", RoleType.Farmer);
        var (otherUser, _) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, ownerProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, ownerProfile.Id, listing.Id, availability.Id, BookingStatus.ProviderCompleted);

        var controller = CreateController(db, otherUser.Id);

        var result = await controller.ConfirmCompletion(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AccessDenied", redirect.ActionName);
    }

    [Fact]
    public async Task CheckoutGet_WithAwaitingPayment_ReturnsCheckoutView()
    {
        using var db = TestDbContextFactory.Create(nameof(CheckoutGet_WithAwaitingPayment_ReturnsCheckoutView));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.AwaitingPayment);

        var controller = CreateController(db, user.Id);

        var result = await controller.Checkout(booking.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<CheckoutViewModel>(viewResult.Model);
        Assert.Equal(booking.Id, vm.BookingId);
    }

    [Fact]
    public async Task CheckoutGet_WithNonAwaitingPayment_RedirectsToDetails()
    {
        using var db = TestDbContextFactory.Create(nameof(CheckoutGet_WithNonAwaitingPayment_RedirectsToDetails));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.Confirmed);

        var controller = CreateController(db, user.Id);

        var result = await controller.Checkout(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
    }

    [Fact]
    public async Task CheckoutGet_WithDifferentOwner_RedirectsToAccessDenied()
    {
        using var db = TestDbContextFactory.Create(nameof(CheckoutGet_WithDifferentOwner_RedirectsToAccessDenied));
        var (ownerUser, ownerProfile) = TestDbContextFactory.SeedClientUser(db, "owner@test.com", "pw", RoleType.Farmer);
        var (otherUser, _) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, ownerProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, ownerProfile.Id, listing.Id, availability.Id, BookingStatus.AwaitingPayment);

        var controller = CreateController(db, otherUser.Id);

        var result = await controller.Checkout(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AccessDenied", redirect.ActionName);
    }

    [Fact]
    public async Task CheckoutPost_ValidPayment_RedirectsToReceipt()
    {
        using var db = TestDbContextFactory.Create(nameof(CheckoutPost_ValidPayment_RedirectsToReceipt));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.AwaitingPayment);

        var controller = CreateController(db, user.Id);

        var model = new CheckoutSubmitViewModel
        {
            BookingId = booking.Id,
            Method = PaymentMethod.Card
        };

        var result = await controller.Checkout(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Receipt", redirect.ActionName);
        Assert.Equal("Payments", redirect.ControllerName);

        var updated = await db.Bookings.FindAsync(booking.Id);
        Assert.Equal(BookingStatus.Confirmed, updated!.Status);
    }

    [Fact]
    public async Task CheckoutPost_NonAwaitingPayment_RedirectsWithError()
    {
        using var db = TestDbContextFactory.Create(nameof(CheckoutPost_NonAwaitingPayment_RedirectsWithError));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.Confirmed);

        var controller = CreateController(db, user.Id);

        var model = new CheckoutSubmitViewModel
        {
            BookingId = booking.Id,
            Method = PaymentMethod.Card
        };

        var result = await controller.Checkout(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
    }
}
