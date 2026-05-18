using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Dtos.Messaging;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Contracts;

public interface IConversationRepository
{
    Task<Conversation?> FindBetweenParticipantsAsync(Guid profileId1, Guid profileId2, CancellationToken ct = default);
    Task<(List<ConversationSummaryDto> Items, int TotalCount)> ListWithSummariesAsync(Guid profileId, int page, int pageSize, CancellationToken ct = default);
    Task<Conversation?> GetWithParticipantsAsync(Guid conversationId, CancellationToken ct = default);
    Task<(List<MessageDto> Items, int TotalCount)> GetMessagesAsync(Guid conversationId, Guid callerProfileId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountUnreadAsync(Guid profileId, CancellationToken ct = default);
    Task<bool> IsParticipantAsync(Guid conversationId, Guid profileId, CancellationToken ct = default);
}
