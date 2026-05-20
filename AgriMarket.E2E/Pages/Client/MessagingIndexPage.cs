using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class MessagingIndexPage : PageBase
{
    public MessagingIndexPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Messaging");

    public async Task<int> GetConversationCountAsync()
    {
        var items = await Page.QuerySelectorAllAsync(".conversation-item, .list-group-item");
        return items.Count;
    }

    public async Task ClickConversationAsync(int index)
    {
        var items = await Page.QuerySelectorAllAsync(".conversation-item, a[href*='/Client/Messaging/Details']");
        if (index < items.Count)
        {
            await items[index].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    public async Task<bool> HasUnreadIndicatorAsync()
    {
        return await IsElementVisibleAsync(".badge, .unread, [class*='unread']");
    }
}
