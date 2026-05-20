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
    public async Task AssignEquipment_AppearsOnListingDetail()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);

        var equipCreate = new EquipmentCreatePage(page, _fixture.BaseUrl);
        await equipCreate.NavigateAsync();
        var equipName = $"E2E Tractor {Guid.NewGuid():N[..6]}";
        await equipCreate.FillFormAsync(equipName, "John Deere", "6R 150", "2023", "150");
        await equipCreate.SubmitAsync();

        var equipIndex = new EquipmentIndexPage(page, _fixture.BaseUrl);
        (await equipIndex.ContainsEquipmentAsync(equipName)).Should().BeTrue();

        await page.Context.DisposeAsync();
    }
}
