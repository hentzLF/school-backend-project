using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class MyListingEditPage : PageBase
{
    public MyListingEditPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int listingId) =>
        await NavigateToAsync($"/Client/MyListings/Edit/{listingId}");

    public async Task SetTitleAsync(string title)
    {
        await Page.FillAsync("input[name='Title']", title);
    }

    public async Task SetPriceAsync(string price)
    {
        await Page.FillAsync("input[name='PricePerHectare']", price);
    }

    public async Task SetDescriptionAsync(string description)
    {
        await Page.FillAsync("textarea[name='Description']", description);
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("input[type='submit'][value='Save'], button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
