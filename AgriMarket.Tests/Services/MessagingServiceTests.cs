using AgriMarket.BLL;
using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Messaging;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace AgriMarket.Tests.Services;

public class MessagingServiceTests
{
    private readonly Mock<IConversationRepository> _conversationRepo = new();
    private readonly Mock<IRepository<UserProfile>> _userProfiles = new();
    private readonly Mock<IRepository<Conversation>> _conversations = new();
    private readonly Mock<IRepository<Message>> _messages = new();
    private readonly Mock<IRepository<MessageRead>> _messageReads = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMessageNotifier> _notifier = new();
    private readonly MessagingService _sut;

    private static readonly Guid CallerId = Guid.NewGuid();
    private static readonly Guid OtherId = Guid.NewGuid();

    public MessagingServiceTests()
    {
        _sut = new MessagingService(
            _conversationRepo.Object,
            _userProfiles.Object,
            _conversations.Object,
            _messages.Object,
            _messageReads.Object,
            _uow.Object,
            _notifier.Object);
    }

    private void SetupProfileExists(Guid profileId)
    {
        _userProfiles
            .Setup(r => r.AnyAsync(It.Is<Expression<Func<UserProfile, bool>>>(
                e => ExpressionMatchesId(e, profileId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private void SetupProfileNotFound(Guid profileId)
    {
        _userProfiles
            .Setup(r => r.AnyAsync(It.Is<Expression<Func<UserProfile, bool>>>(
                e => ExpressionMatchesId(e, profileId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    private static bool ExpressionMatchesId(Expression<Func<UserProfile, bool>> expr, Guid targetId)
    {
        var func = expr.Compile();
        return func(new UserProfile { Id = targetId });
    }

    private void SetupConversationExists(Guid conversationId)
    {
        _conversations
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Conversation, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private void SetupIsParticipant(Guid conversationId, Guid profileId, bool isParticipant = true)
    {
        _conversationRepo
            .Setup(r => r.IsParticipantAsync(conversationId, profileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(isParticipant);
    }

    // ===== CreateConversationAsync =====

    [Fact]
    public async Task CreateConversationAsync_ValidCreation_ReturnsConversationDto()
    {
        var dto = new CreateConversationDto { ParticipantProfileIds = [CallerId, OtherId] };
        SetupProfileExists(CallerId);
        SetupProfileExists(OtherId);
        _conversationRepo
            .Setup(r => r.FindBetweenParticipantsAsync(CallerId, OtherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);
        _conversationRepo
            .Setup(r => r.GetWithParticipantsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new Conversation
            {
                Id = id,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>
                {
                    new() { UserProfileId = CallerId, UserProfile = new UserProfile { Id = CallerId, FirstName = "Alice", LastName = "A" } },
                    new() { UserProfileId = OtherId, UserProfile = new UserProfile { Id = OtherId, FirstName = "Bob", LastName = "B" } }
                }
            });

        var (result, isNew) = await _sut.CreateConversationAsync(CallerId, dto);

        Assert.True(isNew);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(2, result.Participants.Count());
        _conversations.Verify(r => r.Add(It.IsAny<Conversation>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateConversationAsync_CallerNotInList_ThrowsBusinessRuleException()
    {
        var otherOther = Guid.NewGuid();
        var dto = new CreateConversationDto { ParticipantProfileIds = [OtherId, otherOther] };

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateConversationAsync(CallerId, dto));
    }

    [Fact]
    public async Task CreateConversationAsync_WrongParticipantCount_ThrowsBusinessRuleException()
    {
        var dto = new CreateConversationDto { ParticipantProfileIds = [CallerId] };

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateConversationAsync(CallerId, dto));
    }

    [Fact]
    public async Task CreateConversationAsync_TooManyParticipants_ThrowsBusinessRuleException()
    {
        var dto = new CreateConversationDto { ParticipantProfileIds = [CallerId, OtherId, Guid.NewGuid()] };

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateConversationAsync(CallerId, dto));
    }

    [Fact]
    public async Task CreateConversationAsync_NonExistentProfile_ThrowsKeyNotFoundException()
    {
        var dto = new CreateConversationDto { ParticipantProfileIds = [CallerId, OtherId] };
        SetupProfileNotFound(CallerId);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.CreateConversationAsync(CallerId, dto));
    }

    [Fact]
    public async Task CreateConversationAsync_DuplicatePrevention_ReturnsExisting()
    {
        var existingId = Guid.NewGuid();
        var dto = new CreateConversationDto { ParticipantProfileIds = [CallerId, OtherId] };
        SetupProfileExists(CallerId);
        SetupProfileExists(OtherId);
        _conversationRepo
            .Setup(r => r.FindBetweenParticipantsAsync(CallerId, OtherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = existingId,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Participants = new List<ConversationParticipant>
                {
                    new() { UserProfileId = CallerId, UserProfile = new UserProfile { Id = CallerId, FirstName = "Alice", LastName = "A" } },
                    new() { UserProfileId = OtherId, UserProfile = new UserProfile { Id = OtherId, FirstName = "Bob", LastName = "B" } }
                }
            });

        var (result, isNew) = await _sut.CreateConversationAsync(CallerId, dto);

        Assert.False(isNew);
        Assert.Equal(existingId, result.Id);
        _conversations.Verify(r => r.Add(It.IsAny<Conversation>()), Times.Never);
    }

    [Fact]
    public async Task CreateConversationAsync_BookingLinked_AlwaysCreatesNew()
    {
        var bookingId = Guid.NewGuid();
        var dto = new CreateConversationDto
        {
            ParticipantProfileIds = [CallerId, OtherId],
            BookingId = bookingId
        };
        SetupProfileExists(CallerId);
        SetupProfileExists(OtherId);
        _conversationRepo
            .Setup(r => r.GetWithParticipantsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new Conversation
            {
                Id = id,
                BookingId = bookingId,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>
                {
                    new() { UserProfileId = CallerId, UserProfile = new UserProfile { Id = CallerId, FirstName = "Alice", LastName = "A" } },
                    new() { UserProfileId = OtherId, UserProfile = new UserProfile { Id = OtherId, FirstName = "Bob", LastName = "B" } }
                }
            });

        var (result, isNew) = await _sut.CreateConversationAsync(CallerId, dto);

        Assert.True(isNew);
        Assert.Equal(bookingId, result.BookingId);
        _conversations.Verify(r => r.Add(It.IsAny<Conversation>()), Times.Once);
        _conversationRepo.Verify(
            r => r.FindBetweenParticipantsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ===== SendMessageAsync =====

    [Fact]
    public async Task SendMessageAsync_ValidSend_ReturnsMessageDto()
    {
        var conversationId = Guid.NewGuid();
        var dto = new SendMessageDto { Content = "Hello!" };
        SetupConversationExists(conversationId);
        SetupIsParticipant(conversationId, CallerId);
        _userProfiles
            .Setup(r => r.GetByIdAsync(CallerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { Id = CallerId, FirstName = "Alice", LastName = "A" });

        var result = await _sut.SendMessageAsync(CallerId, conversationId, dto);

        Assert.Equal("Hello!", result.Content);
        Assert.Equal(CallerId, result.SenderProfileId);
        Assert.Equal("Alice A", result.SenderName);
        _messages.Verify(r => r.Add(It.IsAny<Message>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_NonParticipant_ThrowsUnauthorizedAccessException()
    {
        var conversationId = Guid.NewGuid();
        var dto = new SendMessageDto { Content = "Hello!" };
        SetupConversationExists(conversationId);
        SetupIsParticipant(conversationId, CallerId, false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.SendMessageAsync(CallerId, conversationId, dto));
    }

    [Fact]
    public async Task SendMessageAsync_EmptyContent_ThrowsBusinessRuleException()
    {
        var conversationId = Guid.NewGuid();
        var dto = new SendMessageDto { Content = "   " };

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.SendMessageAsync(CallerId, conversationId, dto));
    }

    [Fact]
    public async Task SendMessageAsync_NonExistentConversation_ThrowsKeyNotFoundException()
    {
        var conversationId = Guid.NewGuid();
        var dto = new SendMessageDto { Content = "Hello!" };
        _conversations
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Conversation, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.SendMessageAsync(CallerId, conversationId, dto));
    }

    // ===== GetConversationsAsync =====

    [Fact]
    public async Task GetConversationsAsync_WithConversations_ReturnsPaginatedResult()
    {
        var summaries = new List<ConversationSummaryDto>
        {
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
        };
        _conversationRepo
            .Setup(r => r.ListWithSummariesAsync(CallerId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((summaries, 2));

        var result = await _sut.GetConversationsAsync(CallerId, 1, 20);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public async Task GetConversationsAsync_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        _conversationRepo
            .Setup(r => r.ListWithSummariesAsync(CallerId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ConversationSummaryDto>(), 0));

        var result = await _sut.GetConversationsAsync(CallerId, 1, 20);

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    // ===== GetConversationAsync =====

    [Fact]
    public async Task GetConversationAsync_ParticipantAccess_ReturnsConversationWithMessages()
    {
        var conversationId = Guid.NewGuid();
        _conversationRepo
            .Setup(r => r.GetWithParticipantsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>
                {
                    new() { UserProfileId = CallerId, UserProfile = new UserProfile { Id = CallerId, FirstName = "Alice", LastName = "A" } }
                }
            });
        SetupIsParticipant(conversationId, CallerId);
        _conversationRepo
            .Setup(r => r.GetMessagesAsync(conversationId, CallerId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<MessageDto> { new() { Id = Guid.NewGuid(), Content = "Hi" } }, 1));

        var result = await _sut.GetConversationAsync(CallerId, conversationId, 1, 50);

        Assert.Equal(conversationId, result.Id);
        Assert.Single(result.Messages.Items);
    }

    [Fact]
    public async Task GetConversationAsync_NonParticipant_ThrowsUnauthorizedAccessException()
    {
        var conversationId = Guid.NewGuid();
        _conversationRepo
            .Setup(r => r.GetWithParticipantsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                Id = conversationId,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>()
            });
        SetupIsParticipant(conversationId, CallerId, false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.GetConversationAsync(CallerId, conversationId, 1, 50));
    }

    [Fact]
    public async Task GetConversationAsync_NonExistentConversation_ThrowsKeyNotFoundException()
    {
        var conversationId = Guid.NewGuid();
        _conversationRepo
            .Setup(r => r.GetWithParticipantsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.GetConversationAsync(CallerId, conversationId, 1, 50));
    }

    // ===== MarkAsReadAsync =====

    [Fact]
    public async Task MarkAsReadAsync_FirstRead_CreatesMessageReadRecord()
    {
        var messageId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        _messages
            .Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message { Id = messageId, ConversationId = conversationId });
        SetupIsParticipant(conversationId, CallerId);
        _messageReads
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<MessageRead, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.MarkAsReadAsync(CallerId, messageId);

        _messageReads.Verify(r => r.Add(It.Is<MessageRead>(mr =>
            mr.MessageId == messageId && mr.UserProfileId == CallerId)), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_IdempotentReRead_DoesNotCreateDuplicate()
    {
        var messageId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        _messages
            .Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message { Id = messageId, ConversationId = conversationId });
        SetupIsParticipant(conversationId, CallerId);
        _messageReads
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<MessageRead, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.MarkAsReadAsync(CallerId, messageId);

        _messageReads.Verify(r => r.Add(It.IsAny<MessageRead>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkAsReadAsync_NonParticipant_ThrowsUnauthorizedAccessException()
    {
        var messageId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        _messages
            .Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message { Id = messageId, ConversationId = conversationId });
        SetupIsParticipant(conversationId, CallerId, false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.MarkAsReadAsync(CallerId, messageId));
    }

    [Fact]
    public async Task MarkAsReadAsync_NonExistentMessage_ThrowsKeyNotFoundException()
    {
        var messageId = Guid.NewGuid();
        _messages
            .Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.MarkAsReadAsync(CallerId, messageId));
    }

    // ===== GetUnreadCountAsync =====

    [Fact]
    public async Task GetUnreadCountAsync_WithUnread_ReturnsCount()
    {
        _conversationRepo
            .Setup(r => r.CountUnreadAsync(CallerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _sut.GetUnreadCountAsync(CallerId);

        Assert.Equal(5, result.UnreadCount);
    }

    [Fact]
    public async Task GetUnreadCountAsync_AllRead_ReturnsZero()
    {
        _conversationRepo
            .Setup(r => r.CountUnreadAsync(CallerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _sut.GetUnreadCountAsync(CallerId);

        Assert.Equal(0, result.UnreadCount);
    }
}
