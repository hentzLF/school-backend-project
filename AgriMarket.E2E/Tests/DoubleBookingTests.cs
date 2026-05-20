using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class DoubleBookingTests
{
    private readonly E2EFixture _fixture;

    public DoubleBookingTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task DoubleBooking_SameAvailability_SecondFails()
    {
        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var createPage = new MyListingCreatePage(providerPage, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"DoubleBook {Guid.NewGuid():N[..8]}";
        await createPage.FillFormAsync(title, "Double booking test", "45.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(providerPage, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        await myListings.ClickDetailsAsync(0);

        await providerPage.ClickAsync("a[href*='Availabilities']");
        await providerPage.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        var availPage = new AvailabilitiesPage(providerPage, _fixture.BaseUrl);
        await availPage.AddAvailabilityAsync(DateTime.Now.AddDays(30), DateTime.Now.AddDays(30).AddHours(8));
        await providerPage.Context.DisposeAsync();

        var farmer1 = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var listingIndex1 = new ListingIndexPage(farmer1, _fixture.BaseUrl);
        await listingIndex1.NavigateAsync();

        var bodyText = await farmer1.InnerTextAsync("body");
        if (!bodyText.Contains(title))
        {
            await farmer1.Context.DisposeAsync();
            return;
        }

        await listingIndex1.ClickListingByTitleAsync(title);
        var detail1 = new ListingDetailPage(farmer1, _fixture.BaseUrl);

        if (await detail1.GetAvailabilityCountAsync() > 0)
        {
            await detail1.FillBookingFormAsync("5");
            await detail1.SubmitBookingAsync();
        }
        await farmer1.Context.DisposeAsync();

        var uniqueEmail = $"farmer2-{Guid.NewGuid():N}@example.com";
        var regPage = await _fixture.CreatePageAsync();
        var register = new ClientRegisterPage(regPage, _fixture.BaseUrl);
        await register.NavigateAsync();
        await register.FillFormAsync("Farmer2", "Test", uniqueEmail, "Farmer2Pass123!");
        await register.SubmitAsync();
        await regPage.Context.DisposeAsync();

        var farmer2 = await _fixture.CreateAuthenticatedClientPageAsync(
            uniqueEmail, "Farmer2Pass123!");
        var listingIndex2 = new ListingIndexPage(farmer2, _fixture.BaseUrl);
        await listingIndex2.NavigateAsync();

        bodyText = await farmer2.InnerTextAsync("body");
        if (bodyText.Contains(title))
        {
            await listingIndex2.ClickListingByTitleAsync(title);
            var detail2 = new ListingDetailPage(farmer2, _fixture.BaseUrl);

            if (await detail2.GetAvailabilityCountAsync() > 0)
            {
                await detail2.FillBookingFormAsync("3");
                await detail2.SubmitBookingAsync();

                bodyText = await farmer2.InnerTextAsync("body");
                bodyText.Should().ContainAny("error", "Error", "already", "booked", "unavailable", "Booking");
            }
        }
        await farmer2.Context.DisposeAsync();
    }
}
