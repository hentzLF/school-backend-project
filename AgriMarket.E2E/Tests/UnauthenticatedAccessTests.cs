using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class UnauthenticatedAccessTests
{
    private readonly E2EFixture _fixture;

    public UnauthenticatedAccessTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task ProtectedClientPage_RedirectsToLogin()
    {
        var page = await _fixture.CreatePageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/Client/Bookings");

        page.Url.Should().Contain("/Client/Account/Login");
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task ProtectedAdminPage_RedirectsToAdminLogin()
    {
        var page = await _fixture.CreatePageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/Admin/Dashboard");

        page.Url.Should().Contain("/Admin/Account/Login");
        await page.Context.DisposeAsync();
    }
}
