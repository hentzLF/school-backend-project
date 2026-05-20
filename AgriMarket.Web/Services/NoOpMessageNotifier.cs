using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Messaging;

namespace AgriMarket.Web.Services;

public class NoOpMessageNotifier : IMessageNotifier
{
    public Task NotifyMessageSentAsync(Guid conversationId, MessageDto message) => Task.CompletedTask;
    public Task NotifyMessageReadAsync(Guid conversationId, Guid messageId, Guid readByProfileId, DateTime readAt) => Task.CompletedTask;
}
