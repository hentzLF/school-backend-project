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
    public async Task CreateEquipment_ValidData_RedirectsFromCreatePage()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var createPage = new EquipmentCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();

        var name = $"E2E Equipment {Guid.NewGuid().ToString("N")[..8]}";
        await createPage.FillFormAsync(name, "TestMake", "TestModel", "2022", "100");
        await createPage.SubmitAsync();

        page.Url.Should().Contain("/Client/Equipment");
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task CreateEquipment_EmptyName_StaysOnCreatePage()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var createPage = new EquipmentCreatePage(page, _fixture.BaseUrl);
        await createPage.NavigateAsync();

        await createPage.FillFormAsync("", "Make", "Model", "2022", "100");
        await createPage.SubmitAsync();

        page.Url.Should().Contain("Create");
        await page.Context.DisposeAsync();
    }
}
