using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ListingToggleActiveTests
{
    private readonly E2EFixture _fixture;

    public ListingToggleActiveTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task MyListings_PageLoadsSuccessfully()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var myListings = new MyListingsIndexPage(page, _fixture.BaseUrl);
        await myListings.NavigateAsync();

        page.Url.Should().Contain("/Client/MyListings");
        await page.Context.DisposeAsync();
    }
}
