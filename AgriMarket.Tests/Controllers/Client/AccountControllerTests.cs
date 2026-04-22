using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class AccountControllerTests
{
    [Theory]
    [InlineData(RoleType.Farmer)]
    [InlineData(RoleType.Provider)]
    public async Task Login_WithClientRole_RedirectsToListings(RoleType role)
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithClientRole_RedirectsToListings) + role);
        TestDbContextFactory.SeedClientUser(db, "farmer@test.com", "password123", role);

        var controller = new AccountController(db);
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

        var controller = new AccountController(db);
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
        TestDbContextFactory.SeedClientUser(db, "user@test.com", "correct", RoleType.Farmer);

        var controller = new AccountController(db);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "user@test.com", Password = "wrong" });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}
