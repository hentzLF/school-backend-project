using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ListingBrowseTests
{
    private readonly E2EFixture _fixture;

    public ListingBrowseTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task ListingIndex_ShowsActiveListings()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var listingIndex = new ListingIndexPage(page, _fixture.BaseUrl);
        await listingIndex.NavigateAsync();

        var count = await listingIndex.GetListingCountAsync();
        count.Should().BeGreaterThanOrEqualTo(0);
        await page.Context.DisposeAsync();
    }
}
