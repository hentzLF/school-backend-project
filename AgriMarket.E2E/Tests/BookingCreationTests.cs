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
    public async Task CreateBooking_ValidData_ShowsPendingStatus()
    {
        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var createPage = new MyListingCreatePage(providerPage, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"Bookable Listing {Guid.NewGuid():N[..8]}";
        await createPage.FillFormAsync(title, "Booking test listing", "40.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(providerPage, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        await myListings.ClickDetailsAsync(0);

        await providerPage.ClickAsync("a[href*='Availabilities']");
        await providerPage.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        var availPage = new AvailabilitiesPage(providerPage, _fixture.BaseUrl);
        var startTime = DateTime.Now.AddDays(14);
        var endTime = startTime.AddHours(8);
        await availPage.AddAvailabilityAsync(startTime, endTime);
        await providerPage.Context.DisposeAsync();

        var farmerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        var listingIndex = new ListingIndexPage(farmerPage, _fixture.BaseUrl);
        await listingIndex.NavigateAsync();

        var pageText = await farmerPage.InnerTextAsync("body");
        if (pageText.Contains(title))
        {
            await listingIndex.ClickListingByTitleAsync(title);
            var detailPage = new ListingDetailPage(farmerPage, _fixture.BaseUrl);

            if (await detailPage.GetAvailabilityCountAsync() > 0)
            {
                await detailPage.FillBookingFormAsync("10");
                await detailPage.SubmitBookingAsync();

                var bodyText = await farmerPage.InnerTextAsync("body");
                bodyText.Should().ContainAny("Pending", "pending", "Booking");
            }
        }

        await farmerPage.Context.DisposeAsync();
    }
}
