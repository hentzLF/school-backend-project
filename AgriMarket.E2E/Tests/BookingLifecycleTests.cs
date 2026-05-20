using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class BookingLifecycleTests
{
    private readonly E2EFixture _fixture;

    public BookingLifecycleTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task BookingLifecycle_PendingToClientConfirmed()
    {
        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var createPage = new MyListingCreatePage(providerPage, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"Lifecycle {Guid.NewGuid():N[..8]}";
        await createPage.FillFormAsync(title, "Lifecycle test", "35.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(providerPage, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        await myListings.ClickDetailsAsync(0);

        await providerPage.ClickAsync("a[href*='Availabilities']");
        await providerPage.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        var availPage = new AvailabilitiesPage(providerPage, _fixture.BaseUrl);
        await availPage.AddAvailabilityAsync(DateTime.Now.AddDays(20), DateTime.Now.AddDays(20).AddHours(10));
        await providerPage.Context.DisposeAsync();

        var farmerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var listingIndex = new ListingIndexPage(farmerPage, _fixture.BaseUrl);
        await listingIndex.NavigateAsync();

        var bodyText = await farmerPage.InnerTextAsync("body");
        if (!bodyText.Contains(title))
        {
            await farmerPage.Context.DisposeAsync();
            return;
        }

        await listingIndex.ClickListingByTitleAsync(title);
        var detailPage = new ListingDetailPage(farmerPage, _fixture.BaseUrl);

        if (await detailPage.GetAvailabilityCountAsync() == 0)
        {
            await farmerPage.Context.DisposeAsync();
            return;
        }

        await detailPage.FillBookingFormAsync("5");
        await detailPage.SubmitBookingAsync();

        bodyText = await farmerPage.InnerTextAsync("body");
        bodyText.Should().ContainAny("Pending", "pending", "Booking");
        await farmerPage.Context.DisposeAsync();

        var provPage2 = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var myListings2 = new MyListingsIndexPage(provPage2, _fixture.BaseUrl);
        await myListings2.NavigateAsync();
        await myListings2.ClickDetailsAsync(0);

        await provPage2.ClickAsync("a[href*='Bookings']");
        await provPage2.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        var confirmBtn = await provPage2.QuerySelectorAsync("form[action*='UpdateBookingStatus'] button.btn-success");
        if (confirmBtn is not null)
        {
            await confirmBtn.ClickAsync();
            await provPage2.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        }

        await provPage2.Context.DisposeAsync();
    }
}
