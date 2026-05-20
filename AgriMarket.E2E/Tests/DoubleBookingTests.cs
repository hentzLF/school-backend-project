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
    public async Task ListingDetailPage_LoadsForFarmer()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        var listingIndex = new ListingIndexPage(page, _fixture.BaseUrl);
        await listingIndex.NavigateAsync();

        page.Url.Should().Contain("/Client/Listings");
        await page.Context.DisposeAsync();
    }
}
