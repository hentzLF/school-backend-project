using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class EquipmentAssignmentTests
{
    private readonly E2EFixture _fixture;

    public EquipmentAssignmentTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task EquipmentIndex_PageLoadsForProvider()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var equipIndex = new EquipmentIndexPage(page, _fixture.BaseUrl);
        await equipIndex.NavigateAsync();

        page.Url.Should().Contain("/Client/Equipment");
        await page.Context.DisposeAsync();
    }
}
