using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ListingIndexPage : PageBase
{
    public ListingIndexPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Listings");

    public async Task<int> GetListingCountAsync()
    {
        var cards = await Page.QuerySelectorAllAsync(".card");
        return cards.Count;
    }

    public async Task<IReadOnlyList<string>> GetListingTitlesAsync()
    {
        var titles = await Page.QuerySelectorAllAsync(".card .card-title, .card h5");
        var result = new List<string>();
        foreach (var t in titles)
            result.Add(await t.InnerTextAsync());
        return result;
    }

    public async Task ClickListingAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a.btn-outline-success, a[href*='/Client/Listings/Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task ClickListingByTitleAsync(string title)
    {
        var link = await Page.QuerySelectorAsync($"a[href*='/Client/Listings/Details']:has-text('{title}')");
        if (link is null)
        {
            var card = await Page.QuerySelectorAsync($".card:has-text('{title}') a.btn-outline-success");
            card?.ClickAsync();
        }
        else
        {
            await link.ClickAsync();
        }
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
