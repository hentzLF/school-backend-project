using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class EquipmentCrudTests
{
    private readonly E2EFixture _fixture;

    public EquipmentCrudTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CreateEquipment_ValidData_AppearsInList()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var createPage = new EquipmentCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();

        var name = $"E2E Equipment {Guid.NewGuid():N[..8]}";
        await createPage.FillFormAsync(name, "TestMake", "TestModel", "2022", "100");
        await createPage.SubmitAsync();

        var indexPage = new EquipmentIndexPage(page, _fixture.BaseUrl);
        (await indexPage.ContainsEquipmentAsync(name)).Should().BeTrue();
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task CreateEquipment_EmptyName_ShowsError()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        var createPage = new EquipmentCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();

        await createPage.FillFormAsync("", "Make", "Model", "2022", "100");
        await createPage.SubmitAsync();

        (await createPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }
}
