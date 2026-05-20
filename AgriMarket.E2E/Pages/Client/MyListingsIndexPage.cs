using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class MyListingsIndexPage : PageBase
{
    public MyListingsIndexPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/MyListings");

    public async Task<int> GetListingCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task<IReadOnlyList<string>> GetListingTitlesAsync()
    {
        var cells = await Page.QuerySelectorAllAsync("table tbody tr td:first-child");
        var result = new List<string>();
        foreach (var cell in cells)
            result.Add((await cell.InnerTextAsync()).Trim());
        return result;
    }

    public async Task ClickCreateAsync()
    {
        await Page.ClickAsync("a[href*='Create']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickDetailsAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a.btn-info, a[href*='Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task<bool> ContainsListingAsync(string title)
    {
        var titles = await GetListingTitlesAsync();
        return titles.Any(t => t.Contains(title, StringComparison.OrdinalIgnoreCase));
    }
}
