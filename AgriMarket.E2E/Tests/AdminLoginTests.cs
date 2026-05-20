using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminLoginTests
{
    private readonly E2EFixture _fixture;

    public AdminLoginTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AdminLogin_ValidCredentials_RedirectsToDashboard()
    {
        var page = await _fixture.CreatePageAsync();
        var loginPage = new AdminLoginPage(page, _fixture.BaseUrl);
        await loginPage.NavigateAsync();

        await loginPage.LoginAsync(SeedData.AdminEmail, SeedData.AdminPassword);

        (await loginPage.IsOnDashboardAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task AdminLogin_NonAdminUser_ShowsError()
    {
        var page = await _fixture.CreatePageAsync();
        var loginPage = new AdminLoginPage(page, _fixture.BaseUrl);
        await loginPage.NavigateAsync();

        await loginPage.LoginAsync(SeedData.ClientEmail, SeedData.ClientPassword);

        (await loginPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }
}
