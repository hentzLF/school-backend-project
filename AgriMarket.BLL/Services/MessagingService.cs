using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Dtos.Messaging;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Services;

public class MessagingService(
    IConversationRepository conversationRepo,
    IRepository<UserProfile> userProfiles,
    IRepository<Conversation> conversations,
    IRepository<Message> messages,
    IRepository<MessageRead> messageReads,
    IUnitOfWork uow,
    IMessageNotifier notifier) : IMessagingService
{
    public async Task<(ConversationDto Conversation, bool IsNew)> CreateConversationAsync(Guid callerProfileId, CreateConversationDto dto)
    {
        ValidateParticipants(callerProfileId, dto.ParticipantProfileIds);
        await EnsureProfilesExistAsync(dto.ParticipantProfileIds);

        var existing = await FindExistingConversationAsync(dto);
        if (existing is not null)
            return (ToConversationDto(existing), false);

        var conversation = BuildConversation(dto);
        conversations.Add(conversation);
        await uow.SaveChangesAsync();

        var saved = await conversationRepo.GetWithParticipantsAsync(conversation.Id);
        return (ToConversationDto(saved!), true);
    }

    public async Task<MessageDto> SendMessageAsync(Guid callerProfileId, Guid conversationId, SendMessageDto dto)
    {
        ValidateMessageContent(dto.Content);
        await EnsureConversationExistsAsync(conversationId);
        await EnsureIsParticipantAsync(conversationId, callerProfileId);

        var senderProfile = await userProfiles.GetByIdAsync(callerProfileId)
            ?? throw new KeyNotFoundException($"Sender profile {callerProfileId} not found.");
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderProfileId = callerProfileId,
            Content = dto.Content.Trim(),
            SentAt = DateTime.UtcNow
        };

        messages.Add(message);
        await uow.SaveChangesAsync();

        var result = ToMessageDto(message, senderProfile);
        await notifier.NotifyMessageSentAsync(conversationId, result);
        return result;
    }

    public async Task<PaginatedResponse<ConversationSummaryDto>> GetConversationsAsync(
        Guid callerProfileId, int page, int pageSize)
    {
        var (items, totalCount) = await conversationRepo.ListWithSummariesAsync(callerProfileId, page, pageSize);

        return new PaginatedResponse<ConversationSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ConversationDto> GetConversationAsync(
        Guid callerProfileId, Guid conversationId, int page, int pageSize)
    {
        var conversation = await conversationRepo.GetWithParticipantsAsync(conversationId)
            ?? throw new KeyNotFoundException($"Conversation {conversationId} not found.");

        await EnsureIsParticipantAsync(conversationId, callerProfileId);

        var (messageItems, totalCount) = await conversationRepo.GetMessagesAsync(
            conversationId, callerProfileId, page, pageSize);

        return ToConversationDto(conversation, messageItems, totalCount, page, pageSize);
    }

    public async Task MarkAsReadAsync(Guid callerProfileId, Guid messageId)
    {
        var message = await messages.GetByIdAsync(messageId)
            ?? throw new KeyNotFoundException($"Message {messageId} not found.");

        await EnsureIsParticipantAsync(message.ConversationId, callerProfileId);

        var alreadyRead = await messageReads.AnyAsync(
            mr => mr.MessageId == messageId && mr.UserProfileId == callerProfileId);

        if (alreadyRead)
            return;

        var readAt = DateTime.UtcNow;
        messageReads.Add(new MessageRead
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            UserProfileId = callerProfileId,
            ReadAt = readAt
        });

        await uow.SaveChangesAsync();
        await notifier.NotifyMessageReadAsync(message.ConversationId, messageId, callerProfileId, readAt);
    }

    public async Task<int> MarkAllAsReadAsync(Guid callerProfileId, Guid conversationId)
    {
        await EnsureConversationExistsAsync(conversationId);
        await EnsureIsParticipantAsync(conversationId, callerProfileId);

        var unreadIds = await conversationRepo.GetUnreadMessageIdsAsync(conversationId, callerProfileId);
        if (unreadIds.Count == 0)
            return 0;

        var readAt = DateTime.UtcNow;
        foreach (var messageId in unreadIds)
        {
            messageReads.Add(new MessageRead
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                UserProfileId = callerProfileId,
                ReadAt = readAt
            });
        }

        await uow.SaveChangesAsync();

        foreach (var messageId in unreadIds)
            await notifier.NotifyMessageReadAsync(conversationId, messageId, callerProfileId, readAt);

        return unreadIds.Count;
    }

    public async Task<UnreadCountDto> GetUnreadCountAsync(Guid callerProfileId)
    {
        var count = await conversationRepo.CountUnreadAsync(callerProfileId);
        return new UnreadCountDto { UnreadCount = count };
    }

    private static void ValidateParticipants(Guid callerProfileId, IList<Guid> participantIds)
    {
        if (participantIds.Count != 2)
            throw new BusinessRuleException("A conversation requires exactly 2 participants.");

        if (!participantIds.Contains(callerProfileId))
            throw new BusinessRuleException("You must be one of the conversation participants.");
    }

    private async Task EnsureProfilesExistAsync(IList<Guid> profileIds)
    {
        foreach (var profileId in profileIds)
        {
            var exists = await userProfiles.AnyAsync(p => p.Id == profileId);
            if (!exists)
                throw new KeyNotFoundException($"Profile {profileId} not found.");
        }
    }

    private async Task<Conversation?> FindExistingConversationAsync(CreateConversationDto dto)
    {
        if (dto.BookingId.HasValue)
            return null;

        return await conversationRepo.FindBetweenParticipantsAsync(
            dto.ParticipantProfileIds[0], dto.ParticipantProfileIds[1]);
    }

    private static Conversation BuildConversation(CreateConversationDto dto)
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BookingId = dto.BookingId,
            Participants = dto.ParticipantProfileIds.Select(pid => new ConversationParticipant
            {
                ConversationId = default,
                UserProfileId = pid,
                JoinedAt = DateTime.UtcNow
            }).ToList()
        };

        foreach (var participant in conversation.Participants)
            participant.ConversationId = conversation.Id;

        return conversation;
    }

    private static void ValidateMessageContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new BusinessRuleException("Message content cannot be empty.");
    }

    private async Task EnsureConversationExistsAsync(Guid conversationId)
    {
        var exists = await conversations.AnyAsync(c => c.Id == conversationId);
        if (!exists)
            throw new KeyNotFoundException($"Conversation {conversationId} not found.");
    }

    private async Task EnsureIsParticipantAsync(Guid conversationId, Guid profileId)
    {
        var isParticipant = await conversationRepo.IsParticipantAsync(conversationId, profileId);
        if (!isParticipant)
            throw new UnauthorizedAccessException("You are not a participant of this conversation.");
    }

    private static ConversationDto ToConversationDto(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            BookingId = conversation.BookingId,
            CreatedAt = conversation.CreatedAt,
            Participants = conversation.Participants?.Select(ToParticipantDto) ?? [],
            Messages = new PaginatedResponse<MessageDto>
            {
                Items = [],
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            }
        };
    }

    private static ConversationDto ToConversationDto(
        Conversation conversation, List<MessageDto> messageItems,
        int totalCount, int page, int pageSize)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            BookingId = conversation.BookingId,
            CreatedAt = conversation.CreatedAt,
            Participants = conversation.Participants?.Select(ToParticipantDto) ?? [],
            Messages = new PaginatedResponse<MessageDto>
            {
                Items = messageItems,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        };
    }

    private static ParticipantDto ToParticipantDto(ConversationParticipant cp)
    {
        return new ParticipantDto
        {
            ProfileId = cp.UserProfileId,
            FullName = cp.UserProfile is not null
                ? $"{cp.UserProfile.FirstName} {cp.UserProfile.LastName}"
                : "Unknown"
        };
    }

    private static MessageDto ToMessageDto(Message message, UserProfile sender)
    {
        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderProfileId = message.SenderProfileId,
            SenderName = $"{sender.FirstName} {sender.LastName}",
            Content = message.Content,
            SentAt = message.SentAt,
            IsRead = false
        };
    }
}
