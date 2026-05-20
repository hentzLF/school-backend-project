using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class BookingIndexPage : PageBase
{
    public BookingIndexPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Bookings");

    public async Task<int> GetBookingCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr, .list-group-item, .card");
        return rows.Count;
    }

    public async Task ClickBookingAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='/Client/Bookings/Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }
}
