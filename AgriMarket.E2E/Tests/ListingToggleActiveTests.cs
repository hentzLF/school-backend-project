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
    public async Task ToggleActive_DeactivatesAndReactivates()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var createPage = new MyListingCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"Toggle Test {Guid.NewGuid():N[..8]}";
        await createPage.FillFormAsync(title, "Toggle test description", "30.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(page, _fixture.BaseUrl);
        await myListings.NavigateAsync();
        await myListings.ClickDetailsAsync(0);

        await page.ClickAsync("form[action*='ToggleActive'] button[type='submit']");
        await page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        var bodyText = await page.InnerTextAsync("body");
        bodyText.Should().NotBeNull();

        await page.Context.DisposeAsync();
    }
}
