using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class PaymentHistoryTests
{
    private readonly E2EFixture _fixture;

    public PaymentHistoryTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task PaymentHistory_PageLoads()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var historyPage = new PaymentHistoryPage(page, _fixture.BaseUrl);
        await historyPage.NavigateAsync();

        page.Url.Should().Contain("/Client/Payments");
        await page.Context.DisposeAsync();
    }
}
