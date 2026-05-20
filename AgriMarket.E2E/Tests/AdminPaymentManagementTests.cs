using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminPaymentManagementTests
{
    private readonly E2EFixture _fixture;

    public AdminPaymentManagementTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AdminPayments_PageLoads()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var paymentsPage = new AdminPaymentsPage(page, _fixture.BaseUrl);
        await paymentsPage.NavigateAsync();

        page.Url.Should().Contain("/Admin/Payments");
        await page.Context.DisposeAsync();
    }
}
