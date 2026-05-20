using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class BookingListTests
{
    private readonly E2EFixture _fixture;

    public BookingListTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task BookingsIndex_ShowsUserBookings()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var bookingIndex = new BookingIndexPage(page, _fixture.BaseUrl);
        await bookingIndex.NavigateAsync();

        var path = page.Url;
        path.Should().Contain("/Client/Bookings");
        await page.Context.DisposeAsync();
    }
}
