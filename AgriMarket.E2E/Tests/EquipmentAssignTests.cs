using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class EquipmentAssignTests
{
    private readonly E2EFixture _fixture;

    public EquipmentAssignTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task EquipmentAssign_PageLoads()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var indexPage = new EquipmentIndexPage(page, _fixture.BaseUrl);
        await indexPage.NavigateAsync();

        var count = await indexPage.GetEquipmentCountAsync();
        count.Should().BeGreaterThanOrEqualTo(0);
        await page.Context.DisposeAsync();
    }
}
