using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ServiceBookingJourneyTest
{
    private readonly E2EFixture _fixture;

    public ServiceBookingJourneyTest(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task FullJourney_CreateListing_Book_StatusFlow()
    {
        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var equipCreate = new EquipmentCreatePage(providerPage, _fixture.BaseUrl);
        await equipCreate.NavigateAsync();
        var equipName = $"Journey Tractor {Guid.NewGuid():N[..6]}";
        await equipCreate.FillFormAsync(equipName, "JD", "6R", "2024", "200");
        await equipCreate.SubmitAsync();

        var createPage = new MyListingCreatePage(providerPage, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"Journey Listing {Guid.NewGuid():N[..8]}";
        await createPage.FillFormAsync(title, "Full journey test listing", "60.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(providerPage, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        (await myListings.ContainsListingAsync(title)).Should().BeTrue();

        await myListings.ClickDetailsAsync(0);

        await providerPage.ClickAsync("a[href*='Availabilities']");
        await providerPage.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        var availPage = new AvailabilitiesPage(providerPage, _fixture.BaseUrl);
        await availPage.AddAvailabilityAsync(DateTime.Now.AddDays(40), DateTime.Now.AddDays(40).AddHours(10));
        await providerPage.Context.DisposeAsync();

        var farmerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var listingIndex = new ListingIndexPage(farmerPage, _fixture.BaseUrl);
        await listingIndex.NavigateAsync();

        var bodyText = await farmerPage.InnerTextAsync("body");
        bodyText.Should().Contain(title);

        await farmerPage.Context.DisposeAsync();
    }
}
