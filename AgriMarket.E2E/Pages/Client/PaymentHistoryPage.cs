using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class PaymentHistoryPage : PageBase
{
    public PaymentHistoryPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Payments");

    public async Task<int> GetPaymentCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");
}
