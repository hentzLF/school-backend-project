using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class AccountControllerTests
{
    private static readonly AgriMarket.BLL.Contracts.IPasswordHasher PasswordHasher = new BCryptPasswordHasher();

    private static AgriMarket.BLL.Services.UserService CreateUserService(AppDbContext db) =>
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
            NullLogger<AgriMarket.BLL.Services.UserService>.Instance);

    [Fact]
    public async Task Login_WithClientRole_RedirectsToListings()
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithClientRole_RedirectsToListings));
        TestDbContextFactory.SeedClientUser(db, "farmer@test.com", "password123", RoleType.Client);

        var controller = new AccountController(CreateUserService(db), PasswordHasher);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "farmer@test.com", Password = "password123" });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Listings", redirect.ControllerName);
    }

    [Fact]
    public async Task Login_WithAdminRole_ReturnsViewWithError()
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithAdminRole_ReturnsViewWithError));
        TestDbContextFactory.SeedClientUser(db, "admin@test.com", "password123", RoleType.Admin);

        var controller = new AccountController(CreateUserService(db), PasswordHasher);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "admin@test.com", Password = "password123" });

        var view = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsViewWithError()
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithWrongPassword_ReturnsViewWithError));
        TestDbContextFactory.SeedClientUser(db, "user@test.com", "correct", RoleType.Client);

        var controller = new AccountController(CreateUserService(db), PasswordHasher);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "user@test.com", Password = "wrong" });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}
