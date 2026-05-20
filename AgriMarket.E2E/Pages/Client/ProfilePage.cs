using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ProfilePage : PageBase
{
    public ProfilePage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Profile");

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");

    public async Task ClickEditAsync()
    {
        await Page.ClickAsync("a[href*='Profile/Edit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
