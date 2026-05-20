using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminDashboardTests
{
    private readonly E2EFixture _fixture;

    public AdminDashboardTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Dashboard_DisplaysStatistics()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var dashboard = new AdminDashboardPage(page, _fixture.BaseUrl);
        await dashboard.NavigateAsync();

        (await dashboard.IsOnDashboardAsync()).Should().BeTrue();
        (await dashboard.HasStatCardsAsync()).Should().BeTrue();

        var pageText = await dashboard.GetPageTextAsync();
        pageText.Should().NotBeEmpty();
        await page.Context.DisposeAsync();
    }
}
