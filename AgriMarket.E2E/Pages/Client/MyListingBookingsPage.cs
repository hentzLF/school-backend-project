using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class MyListingBookingsPage : PageBase
{
    public MyListingBookingsPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int listingId) =>
        await NavigateToAsync($"/Client/MyListings/Bookings/{listingId}");

    public async Task<int> GetBookingCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task UpdateBookingStatusAsync(int bookingId, int listingId, string status)
    {
        var form = await Page.QuerySelectorAsync(
            $"form[action*='UpdateBookingStatus'] input[name='status'][value='{status}']");
        if (form is not null)
        {
            var parentForm = await form.EvaluateHandleAsync("el => el.closest('form')");
            await ((IElementHandle)parentForm).QuerySelectorAsync("button[type='submit']")
                .ContinueWith(async t =>
                {
                    var btn = await t;
                    if (btn is not null) await btn.ClickAsync();
                });
        }
        else
        {
            var statusBtn = await Page.QuerySelectorAsync($"button:has-text('{status}'), input[value='{status}']");
            if (statusBtn is not null) await statusBtn.ClickAsync();
        }
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickStatusButtonAsync(string buttonText)
    {
        await Page.ClickAsync($"form[action*='UpdateBookingStatus'] button:has-text('{buttonText}')");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
