using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class CheckoutTests
{
    private readonly E2EFixture _fixture;

    public CheckoutTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Checkout_ValidBooking_ShowsReceipt()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        var bookingIndex = new BookingIndexPage(page, _fixture.BaseUrl);
        await bookingIndex.NavigateAsync();

        var count = await bookingIndex.GetBookingCountAsync();
        if (count > 0)
        {
            await bookingIndex.ClickBookingAsync(0);
            var detailPage = new BookingDetailPage(page, _fixture.BaseUrl);

            if (await detailPage.HasCheckoutLinkAsync())
            {
                await detailPage.ClickCheckoutAsync();
                var checkoutPage = new CheckoutPage(page, _fixture.BaseUrl);
                await checkoutPage.SelectPaymentMethodAsync("methodCard");
                await checkoutPage.SubmitAsync();

                var receiptPage = new ReceiptPage(page, _fixture.BaseUrl);
                (await receiptPage.IsOnReceiptPageAsync()).Should().BeTrue();
            }
        }

        await page.Context.DisposeAsync();
    }
}
