using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ReceiptPage : PageBase
{
    public ReceiptPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");

    public async Task<bool> IsOnReceiptPageAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("Receipt", StringComparison.OrdinalIgnoreCase);
    }
}
