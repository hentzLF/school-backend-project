using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class BookingsControllerTests
{
    [Fact]
    public async Task Details_WithDifferentOwner_RedirectsToAccessDenied()
    {
        using var db = TestDbContextFactory.Create(nameof(Details_WithDifferentOwner_RedirectsToAccessDenied));
        var (ownerUser, ownerProfile) = TestDbContextFactory.SeedClientUser(db, "owner@test.com", "pw", RoleType.Farmer);
        var (requestorUser, _) = TestDbContextFactory.SeedClientUser(db, "requestor@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, ownerProfile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, ownerProfile.Id, listing.Id, availability.Id);

        var controller = new BookingsController(db);
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(requestorUser.Id);

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

        var controller = new BookingsController(db);
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(user.Id);

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

        var controller = new BookingsController(db);
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(user.Id);

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

        var controller = new BookingsController(db);
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(user.Id);

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

        var controller = new BookingsController(db);
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(otherUser.Id);

        var result = await controller.ConfirmCompletion(booking.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AccessDenied", redirect.ActionName);
    }
}
