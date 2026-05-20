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
    public async Task FullJourney_ProviderCreatesListingAndFarmerBrowses()
    {
        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);

        var createPage = new MyListingCreatePage(providerPage, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"Journey Listing {Guid.NewGuid().ToString("N")[..8]}";
        await createPage.FillFormAsync(title, "Full journey test listing", "60.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(providerPage, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        (await myListings.ContainsListingAsync(title)).Should().BeTrue();
        await providerPage.Context.DisposeAsync();

        var farmerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var listingIndex = new ListingIndexPage(farmerPage, _fixture.BaseUrl);
        await listingIndex.NavigateAsync();

        var bodyText = await farmerPage.InnerTextAsync("body");
        bodyText.Should().Contain(title);
        await farmerPage.Context.DisposeAsync();
    }
}
