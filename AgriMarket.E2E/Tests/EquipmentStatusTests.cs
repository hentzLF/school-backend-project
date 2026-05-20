using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class EquipmentStatusTests
{
    private readonly E2EFixture _fixture;

    public EquipmentStatusTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task EquipmentIndex_PageLoads()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var indexPage = new EquipmentIndexPage(page, _fixture.BaseUrl);
        await indexPage.NavigateAsync();

        page.Url.Should().Contain("/Client/Equipment");
        await page.Context.DisposeAsync();
    }
}
