using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminDashboardPage : PageBase
{
    public AdminDashboardPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Dashboard");

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");

    public async Task<bool> HasStatCardsAsync()
    {
        var cards = await Page.QuerySelectorAllAsync(".card");
        return cards.Count > 0;
    }

    public async Task<bool> IsOnDashboardAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("/Admin", StringComparison.OrdinalIgnoreCase);
    }
}
