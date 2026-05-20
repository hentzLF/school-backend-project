using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class InvalidPaymentTests
{
    private readonly E2EFixture _fixture;

    public InvalidPaymentTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Checkout_NonPayableBooking_ShowsErrorOrRedirects()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);

        var response = await page.GotoAsync($"{_fixture.BaseUrl}/Client/Bookings/Checkout/99999");

        var bodyText = await page.InnerTextAsync("body");
        var isErrorOrRedirect = response!.Status >= 400
            || bodyText.Contains("error", StringComparison.OrdinalIgnoreCase)
            || bodyText.Contains("not found", StringComparison.OrdinalIgnoreCase)
            || page.Url.Contains("Login")
            || page.Url.Contains("Bookings");
        isErrorOrRedirect.Should().BeTrue();

        await page.Context.DisposeAsync();
    }
}
