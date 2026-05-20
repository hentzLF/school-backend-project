using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminListingManagementTests
{
    private readonly E2EFixture _fixture;

    public AdminListingManagementTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AdminListings_PageLoads()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var listingsPage = new AdminListingsPage(page, _fixture.BaseUrl);
        await listingsPage.NavigateAsync();

        page.Url.Should().Contain("/Admin/Listings");
        await page.Context.DisposeAsync();
    }
}
