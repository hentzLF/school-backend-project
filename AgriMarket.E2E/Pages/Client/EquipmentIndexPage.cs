using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class EquipmentIndexPage : PageBase
{
    public EquipmentIndexPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Equipment");

    public async Task<int> GetEquipmentCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task<IReadOnlyList<string>> GetEquipmentNamesAsync()
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
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickEditAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='Edit']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    public async Task ClickDeleteAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='Delete']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    public async Task<bool> ContainsEquipmentAsync(string name)
    {
        var names = await GetEquipmentNamesAsync();
        return names.Any(n => n.Contains(name, StringComparison.OrdinalIgnoreCase));
    }
}
