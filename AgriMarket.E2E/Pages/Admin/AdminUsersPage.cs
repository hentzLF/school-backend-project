using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminUsersPage : PageBase
{
    public AdminUsersPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Users");

    public async Task<int> GetUserCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task ClickDetailsAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='/Admin/Users/Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");

    public async Task LockUserAsync()
    {
        await Page.ClickAsync("form[action*='Lock'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task UnlockUserAsync()
    {
        await Page.ClickAsync("form[action*='Unlock'] button[type='submit']");
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

    public async Task ClickEditAsync()
    {
        await Page.ClickAsync("a[href*='Edit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
