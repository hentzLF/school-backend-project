using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Admin.Controllers;
using AgriMarket.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AgriMarket.Tests.Controllers.Admin;

public class AccountControllerTests
{
    [Fact]
    public async Task Login_WithAdminRole_RedirectsToDashboard()
    {
        using var db = TestDbContextFactory.Create(nameof(Login_WithAdminRole_RedirectsToDashboard));
        TestDbContextFactory.SeedClientUser(db, "admin@test.com", "password123", RoleType.Admin);

        var controller = new AccountController(db);
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

        var controller = new AccountController(db);
        controller.ControllerContext = ControllerContextFactory.WithSignInSupport();

        var result = await controller.Login(new LoginViewModel { Email = "client@test.com", Password = "password123" });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }
}
