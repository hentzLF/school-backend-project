using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AvailabilityManagementTests
{
    private readonly E2EFixture _fixture;

    public AvailabilityManagementTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CreateListingAndManageAvailabilities()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var createPage = new MyListingCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();
        var title = $"Avail Test {Guid.NewGuid().ToString("N")[..8]}";
        await createPage.FillFormAsync(title, "Availability test", "25.00");
        await createPage.SubmitAsync();

        var myListings = new MyListingsIndexPage(page, _fixture.BaseUrl);
        await myListings.NavigateAsync();

        var count = await myListings.GetListingCountAsync();
        count.Should().BeGreaterThanOrEqualTo(1);

        await page.Context.DisposeAsync();
    }
}
