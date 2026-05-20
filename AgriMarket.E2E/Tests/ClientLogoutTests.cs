using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ClientLogoutTests
{
    private readonly E2EFixture _fixture;

    public ClientLogoutTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Logout_RedirectsToLogin_AndProtectedPagesRedirect()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        await page.ClickAsync("form[action*='Logout'] button[type='submit']");
        await page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.DOMContentLoaded);

        page.Url.Should().Contain("/Client/Account/Login");

        await page.GotoAsync($"{_fixture.BaseUrl}/Client/Bookings");
        page.Url.Should().Contain("/Client/Account/Login");

        await page.Context.DisposeAsync();
    }
}
