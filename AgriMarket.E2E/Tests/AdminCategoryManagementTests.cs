using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminCategoryManagementTests
{
    private readonly E2EFixture _fixture;

    public AdminCategoryManagementTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CategoryList_ShowsAllCategories()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var categoriesPage = new AdminCategoriesPage(page, _fixture.BaseUrl);
        await categoriesPage.NavigateAsync();

        var count = await categoriesPage.GetCategoryCountAsync();
        count.Should().BeGreaterThanOrEqualTo(SeedData.CategoryCount);
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task CreateCategory_ValidName_AppearsInList()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var categoriesPage = new AdminCategoriesPage(page, _fixture.BaseUrl);
        await categoriesPage.NavigateAsync();
        await categoriesPage.ClickCreateAsync();

        var name = $"E2E Category {Guid.NewGuid():N[..8]}";
        await categoriesPage.FillCreateFormAsync(name, "E2E test category");
        await categoriesPage.SubmitAsync();

        await categoriesPage.NavigateAsync();
        var names = await categoriesPage.GetCategoryNamesAsync();
        names.Should().Contain(n => n.Contains(name));
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task CreateCategory_DuplicateName_ShowsError()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var categoriesPage = new AdminCategoriesPage(page, _fixture.BaseUrl);
        await categoriesPage.NavigateAsync();
        await categoriesPage.ClickCreateAsync();

        await categoriesPage.FillCreateFormAsync("Hay Baling", "Duplicate test");
        await categoriesPage.SubmitAsync();

        (await categoriesPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }
}
