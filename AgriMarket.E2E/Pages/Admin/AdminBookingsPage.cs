using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminBookingsPage : PageBase
{
    public AdminBookingsPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Bookings");

    public async Task<int> GetBookingCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task FilterByStatusAsync(string status)
    {
        await Page.ClickAsync($"a[href*='status={status}']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickDetailsAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='/Admin/Bookings/Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");
}
