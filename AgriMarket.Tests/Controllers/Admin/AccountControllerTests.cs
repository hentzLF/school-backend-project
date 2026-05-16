using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Admin.Controllers;
using AgriMarket.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AgriMarket.Tests.Controllers.Admin;

public class AccountControllerTests
{
    private static readonly AgriMarket.BLL.Contracts.IPasswordHasher PasswordHasher = new BCryptPasswordHasher();

    private static AgriMarket.BLL.Services.UserService CreateUserService(AppDbContext db) =>
        new(new EfAppUserRepository(db),
            new EfUserProfileRepository(db),
            new EfRepository<ProfileRole>(db),
            new EfUnitOfWork(db),
            new EfRepository<Notification>(db),
            new EfRepository<MessageRead>(db),
            new EfRepository<Message>(db),
            new EfRepository<ConversationParticipant>(db),
            new EfRepository<Review>(db),
            new EfRepository<Booking>(db),
            new EfRepository<ServiceListing>(db),
            NullLogger<AgriMarket.BLL.Services.UserService>.Instance);

    [Fact]
    public async Task Login_WithAdminRole_RedirectsToDashboard()
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithAdminRole_RedirectsToDashboard));
        TestDbContextFactory.SeedClientUser(db, "admin@test.com", "password123", RoleType.Admin);

        var controller = new AccountController(CreateUserService(db), PasswordHasher);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "admin@test.com", Password = "password123" });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);
    }

    [Theory]
    [InlineData(RoleType.Farmer)]
    [InlineData(RoleType.Provider)]
    public async Task Login_WithClientRole_ReturnsViewWithError(RoleType role)
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithClientRole_ReturnsViewWithError) + role);
        TestDbContextFactory.SeedClientUser(db, "client@test.com", "password123", role);

        var controller = new AccountController(CreateUserService(db), PasswordHasher);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "client@test.com", Password = "password123" });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }
}
