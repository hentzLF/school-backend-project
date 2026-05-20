using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminCategoriesPage : PageBase
{
    public AdminCategoriesPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Categories");

    public async Task<int> GetCategoryCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr");
        return rows.Count;
    }

    public async Task<IReadOnlyList<string>> GetCategoryNamesAsync()
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

    public async Task FillCreateFormAsync(string name, string description)
    {
        await Page.FillAsync("input[name='Name']", name);
        await Page.FillAsync("textarea[name='Description']", description);
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("form[action*='Categories'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickEditAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='Edit']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task FillEditFormAsync(string name)
    {
        await Page.FillAsync("input[name='Name']", name);
    }

    public async Task ClickDeleteAsync(int index)
    {
        var links = await Page.QuerySelectorAllAsync("a[href*='Delete']");
        if (index < links.Count)
        {
            await links[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task ConfirmDeleteAsync()
    {
        await Page.ClickAsync("form[action*='Delete'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();

    public async Task<string> GetPageTextAsync() => await Page.InnerTextAsync("body");
}
