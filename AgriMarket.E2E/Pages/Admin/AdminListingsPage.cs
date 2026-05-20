using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminListingsPage : PageBase
{
    public AdminListingsPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Listings");

    public async Task<int> GetListingCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task FilterByActiveAsync(bool active)
    {
        var value = active ? "true" : "false";
        await Page.ClickAsync($"a[href*='active={value}']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickDetailsAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='/Admin/Listings/Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task ClickEditAsync()
    {
        await Page.ClickAsync("a[href*='Edit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickDeleteAsync()
    {
        await Page.ClickAsync("a[href*='Delete']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ConfirmDeleteAsync()
    {
        await Page.ClickAsync("form[action*='Delete'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");
}
