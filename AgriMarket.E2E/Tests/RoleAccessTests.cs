using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class RoleAccessTests
{
    private readonly E2EFixture _fixture;

    public RoleAccessTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task NonAdmin_CannotAccessAdminDashboard()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        await page.GotoAsync($"{_fixture.BaseUrl}/Admin/Dashboard");

        var url = page.Url;
        var bodyText = await page.InnerTextAsync("body");
        var isBlocked = url.Contains("AccessDenied")
            || url.Contains("Login")
            || bodyText.Contains("Access Denied", StringComparison.OrdinalIgnoreCase)
            || bodyText.Contains("denied", StringComparison.OrdinalIgnoreCase);
        isBlocked.Should().BeTrue();

        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task NonAdmin_CannotAccessAdminUsers()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        await page.GotoAsync($"{_fixture.BaseUrl}/Admin/Users");

        var url = page.Url;
        var isBlocked = url.Contains("AccessDenied") || url.Contains("Login");
        isBlocked.Should().BeTrue();

        await page.Context.DisposeAsync();
    }
}
