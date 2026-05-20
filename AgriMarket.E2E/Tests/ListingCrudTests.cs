using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ListingCrudTests
{
    private readonly E2EFixture _fixture;

    public ListingCrudTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CreateListing_ValidData_AppearsInMyListings()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var createPage = new MyListingCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();

        var title = $"E2E Test Listing {Guid.NewGuid().ToString("N")[..8]}";
        await createPage.FillFormAsync(title, "Test description for E2E", "50.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(page, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        (await myListings.ContainsListingAsync(title)).Should().BeTrue();
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task CreateListing_EmptyTitle_ShowsError()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var createPage = new MyListingCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();

        await createPage.FillFormAsync("", "Description", "50.00");
        await createPage.SubmitAsync();

        page.Url.Should().Contain("Create");
        await page.Context.DisposeAsync();
    }
}
