using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class BookingDetailPage : PageBase
{
    public BookingDetailPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int bookingId) =>
        await NavigateToAsync($"/Client/Bookings/Details/{bookingId}");

    public async Task<string> GetStatusAsync()
    {
        var badge = await Page.QuerySelectorAsync(".badge, [class*='status']");
        return badge is not null ? (await badge.InnerTextAsync()).Trim() : string.Empty;
    }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");

    public async Task<bool> HasCheckoutLinkAsync() =>
        await IsElementVisibleAsync("a[href*='Checkout']");

    public async Task ClickCheckoutAsync()
    {
        await Page.ClickAsync("a[href*='Checkout']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ConfirmCompletionAsync()
    {
        await Page.ClickAsync("form[action*='ConfirmCompletion'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task CancelBookingAsync()
    {
        await Page.ClickAsync("form[action*='Cancel'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasConfirmCompletionButtonAsync() =>
        await IsElementVisibleAsync("form[action*='ConfirmCompletion'] button[type='submit']");

    public async Task<bool> HasCancelButtonAsync() =>
        await IsElementVisibleAsync("form[action*='Cancel'] button[type='submit']");

    public async Task ClickMessageProviderAsync()
    {
        await Page.ClickAsync("form[action*='Messaging'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasReviewLinkAsync() =>
        await IsElementVisibleAsync("a[href*='Reviews/Create']");

    public async Task ClickCreateReviewAsync()
    {
        await Page.ClickAsync("a[href*='Reviews/Create']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
