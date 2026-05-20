using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ConversationDetailPage : PageBase
{
    public ConversationDetailPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int conversationId) =>
        await NavigateToAsync($"/Client/Messaging/Details/{conversationId}");

    public async Task<int> GetMessageCountAsync()
    {
        var messages = await Page.QuerySelectorAllAsync(".chat-bubble");
        return messages.Count;
    }

    public async Task<IReadOnlyList<string>> GetMessageTextsAsync()
    {
        var bubbles = await Page.QuerySelectorAllAsync(".chat-bubble");
        var result = new List<string>();
        foreach (var bubble in bubbles)
            result.Add((await bubble.InnerTextAsync()).Trim());
        return result;
    }

    public async Task SendMessageAsync(string content)
    {
        await Page.FillAsync("input[name='Content']", content);
        await Page.ClickAsync("form[action*='SendMessage'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> ContainsMessageAsync(string text)
    {
        var messages = await GetMessageTextsAsync();
        return messages.Any(m => m.Contains(text, StringComparison.OrdinalIgnoreCase));
    }
}
