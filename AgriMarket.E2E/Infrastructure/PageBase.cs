using Microsoft.Playwright;

namespace AgriMarket.E2E.Infrastructure;

public abstract class PageBase
{
    protected IPage Page { get; }
    protected string BaseUrl { get; }

    protected PageBase(IPage page, string baseUrl)
    {
        Page = page;
        BaseUrl = baseUrl;
    }

    protected async Task NavigateToAsync(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");
    }

    protected async Task<string> GetCurrentPathAsync()
    {
        var url = Page.Url;
        var uri = new Uri(url);
        return uri.AbsolutePath;
    }

    protected async Task<bool> HasValidationErrorsAsync()
    {
        var errorElements = await Page.QuerySelectorAllAsync(".text-danger:not(:empty)");
        return errorElements.Count > 0;
    }

    protected async Task<string> GetValidationSummaryTextAsync()
    {
        var summary = await Page.QuerySelectorAsync("[data-valmsg-summary='true'], .validation-summary-errors");
        return summary is not null ? await summary.InnerTextAsync() : string.Empty;
    }

    protected async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync();
    }

    protected async Task<bool> IsElementVisibleAsync(string selector)
    {
        var element = await Page.QuerySelectorAsync(selector);
        return element is not null && await element.IsVisibleAsync();
    }
}
