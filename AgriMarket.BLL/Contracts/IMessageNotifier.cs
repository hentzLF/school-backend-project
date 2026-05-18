using AgriMarket.BLL.Dtos.Messaging;

namespace AgriMarket.BLL.Contracts;

public interface IMessageNotifier
{
    Task NotifyMessageSentAsync(Guid conversationId, MessageDto message);
    Task NotifyMessageReadAsync(Guid conversationId, Guid messageId, Guid readByProfileId, DateTime readAt);
}
