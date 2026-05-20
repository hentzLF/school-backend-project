using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class BookingCreationTests
{
    private readonly E2EFixture _fixture;

    public BookingCreationTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task BookingsPage_LoadsForFarmer()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);

        var bookingIndex = new BookingIndexPage(page, _fixture.BaseUrl);
        await bookingIndex.NavigateAsync();

        page.Url.Should().Contain("/Client/Bookings");
        await page.Context.DisposeAsync();
    }
}
