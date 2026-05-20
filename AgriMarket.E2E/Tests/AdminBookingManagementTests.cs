using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminBookingManagementTests
{
    private readonly E2EFixture _fixture;

    public AdminBookingManagementTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AdminBookings_PageLoads()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var bookingsPage = new AdminBookingsPage(page, _fixture.BaseUrl);
        await bookingsPage.NavigateAsync();

        page.Url.Should().Contain("/Admin/Bookings");
        await page.Context.DisposeAsync();
    }
}
