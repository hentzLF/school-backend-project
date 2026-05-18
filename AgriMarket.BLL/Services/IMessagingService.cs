using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Dtos.Messaging;

namespace AgriMarket.BLL.Services;

public interface IMessagingService
{
    Task<ConversationDto> CreateConversationAsync(Guid callerProfileId, CreateConversationDto dto);
    Task<MessageDto> SendMessageAsync(Guid callerProfileId, Guid conversationId, SendMessageDto dto);
    Task<PaginatedResponse<ConversationSummaryDto>> GetConversationsAsync(Guid callerProfileId, int page, int pageSize);
    Task<ConversationDto> GetConversationAsync(Guid callerProfileId, Guid conversationId, int page, int pageSize);
    Task MarkAsReadAsync(Guid callerProfileId, Guid messageId);
    Task<UnreadCountDto> GetUnreadCountAsync(Guid callerProfileId);
}
