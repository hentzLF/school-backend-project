using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class CheckoutPage : PageBase
{
    public CheckoutPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int bookingId) =>
        await NavigateToAsync($"/Client/Bookings/Checkout/{bookingId}");

    public async Task SelectPaymentMethodAsync(string methodId = "methodCard")
    {
        await Page.CheckAsync($"#{methodId}");
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();
}
