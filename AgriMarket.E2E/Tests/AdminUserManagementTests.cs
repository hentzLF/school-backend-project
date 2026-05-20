using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminUserManagementTests
{
    private readonly E2EFixture _fixture;

    public AdminUserManagementTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task UserList_ShowsAllUsers()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var usersPage = new AdminUsersPage(page, _fixture.BaseUrl);
        await usersPage.NavigateAsync();

        var count = await usersPage.GetUserCountAsync();
        count.Should().BeGreaterThanOrEqualTo(3);
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task UserDetails_ShowsProfileInfo()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var usersPage = new AdminUsersPage(page, _fixture.BaseUrl);
        await usersPage.NavigateAsync();
        await usersPage.ClickDetailsAsync(0);

        var pageText = await usersPage.GetPageTextAsync();
        pageText.Should().NotBeEmpty();
        await page.Context.DisposeAsync();
    }
}
