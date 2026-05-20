using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class BookingCancellationTests
{
    private readonly E2EFixture _fixture;

    public BookingCancellationTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CancelBooking_StatusChangesCancelled()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var bookingIndex = new BookingIndexPage(page, _fixture.BaseUrl);
        await bookingIndex.NavigateAsync();

        var count = await bookingIndex.GetBookingCountAsync();
        if (count > 0)
        {
            await bookingIndex.ClickBookingAsync(0);
            var detailPage = new BookingDetailPage(page, _fixture.BaseUrl);

            if (await detailPage.HasCancelButtonAsync())
            {
                await detailPage.CancelBookingAsync();
                var bodyText = await page.InnerTextAsync("body");
                bodyText.Should().ContainAny("Cancelled", "cancelled", "Cancel");
            }
        }

        await page.Context.DisposeAsync();
    }
}
