using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminPaymentsPage : PageBase
{
    public AdminPaymentsPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Payments");

    public async Task<int> GetPaymentCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task ClickDetailsAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='/Admin/Payments/Details']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    public async Task ReleasePaymentAsync()
    {
        await Page.ClickAsync("button[name='Resolution'][value='Release']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task RefundPaymentAsync()
    {
        await Page.ClickAsync("button[name='Resolution'][value='Refund']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");
}
