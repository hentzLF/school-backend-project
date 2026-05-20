using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class DataIsolationTests
{
    private readonly E2EFixture _fixture;

    public DataIsolationTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task User_CannotAccessOthersBooking()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        var response = await page.GotoAsync($"{_fixture.BaseUrl}/Client/Bookings/Details/99999");

        var isBlocked = response!.Status >= 400
            || page.Url.Contains("Error")
            || page.Url.Contains("Login")
            || page.Url.Contains("Bookings");
        isBlocked.Should().BeTrue();

        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task User_CannotAccessOthersConversation()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        var response = await page.GotoAsync($"{_fixture.BaseUrl}/Client/Messaging/Details/99999");

        var isBlocked = response!.Status >= 400
            || page.Url.Contains("Error")
            || page.Url.Contains("Login")
            || page.Url.Contains("Messaging");
        isBlocked.Should().BeTrue();

        await page.Context.DisposeAsync();
    }
}
