using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Messaging;
using Microsoft.AspNetCore.SignalR;

namespace AgriMarket.Api.Hubs;

public class SignalRMessageNotifier(IHubContext<MessageHub> hubContext) : IMessageNotifier
{
    public async Task NotifyMessageSentAsync(Guid conversationId, MessageDto message)
    {
        await hubContext.Clients
            .Group(MessageHub.GroupName(conversationId))
            .SendAsync("ReceiveMessage", message);
    }

    public async Task NotifyMessageReadAsync(
        Guid conversationId, Guid messageId, Guid readByProfileId, DateTime readAt)
    {
        await hubContext.Clients
            .Group(MessageHub.GroupName(conversationId))
            .SendAsync("MessageRead", new { messageId, readByProfileId, readAt });
    }
}
