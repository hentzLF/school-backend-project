using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminDisputeJourneyTest
{
    private readonly E2EFixture _fixture;

    public AdminDisputeJourneyTest(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AdminDispute_CanAccessPaymentManagement()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);

        var paymentsPage = new AdminPaymentsPage(page, _fixture.BaseUrl);
        await paymentsPage.NavigateAsync();

        page.Url.Should().Contain("/Admin/Payments");

        var pageText = await paymentsPage.GetPageTextAsync();
        pageText.Should().NotBeEmpty();

        await page.Context.DisposeAsync();
    }
}
