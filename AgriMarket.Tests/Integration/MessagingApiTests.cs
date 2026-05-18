using AgriMarket.BLL;
using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Messaging;
using AgriMarket.BLL.Services;
using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using Moq;
using Xunit;

namespace AgriMarket.Tests.Integration;

public class MessagingApiTests
{
    private static (MessagingService service, AppDbContext db) CreateServiceWithDb(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        var service = new MessagingService(
            new EfConversationRepository(db),
            new EfRepository<UserProfile>(db),
            new EfRepository<Conversation>(db),
            new EfRepository<Message>(db),
            new EfRepository<MessageRead>(db),
            new EfUnitOfWork(db),
            new Mock<IMessageNotifier>().Object);
        return (service, db);
    }

    private static (UserProfile caller, UserProfile other) SeedTwoProfiles(AppDbContext db)
    {
        var (_, callerProfile) = TestDbContextFactory.SeedClientUser(db, "caller@test.com", "pw", RoleType.Farmer);
        var (_, otherProfile) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pw", RoleType.Farmer);
        return (callerProfile, otherProfile);
    }

    [Fact]
    public async Task FullConversationLifecycle_CreateSendListGetMarkReadUnread()
    {
        var (service, db) = CreateServiceWithDb(nameof(FullConversationLifecycle_CreateSendListGetMarkReadUnread));
        using var _ = db;
        var (caller, other) = SeedTwoProfiles(db);

        var (conversation, _) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });
        Assert.NotEqual(Guid.Empty, conversation.Id);
        Assert.Equal(2, conversation.Participants.Count());

        var msg1 = await service.SendMessageAsync(caller.Id, conversation.Id,
            new SendMessageDto { Content = "Hello!" });
        Assert.Equal("Hello!", msg1.Content);
        Assert.Equal(caller.Id, msg1.SenderProfileId);

        var msg2 = await service.SendMessageAsync(other.Id, conversation.Id,
            new SendMessageDto { Content = "Hi back!" });
        Assert.Equal("Hi back!", msg2.Content);

        var list = await service.GetConversationsAsync(caller.Id, 1, 20);
        Assert.Equal(1, list.TotalCount);
        var summary = list.Items.First();
        Assert.Equal("Hi back!", summary.LastMessage!.Content);

        var detail = await service.GetConversationAsync(caller.Id, conversation.Id, 1, 50);
        Assert.Equal(2, detail.Messages.TotalCount);
        Assert.Equal(conversation.Id, detail.Id);

        var unreadBefore = await service.GetUnreadCountAsync(caller.Id);
        Assert.Equal(1, unreadBefore.UnreadCount);

        await service.MarkAsReadAsync(caller.Id, msg2.Id);

        var unreadAfter = await service.GetUnreadCountAsync(caller.Id);
        Assert.Equal(0, unreadAfter.UnreadCount);
    }

    [Fact]
    public async Task Authorization_NonParticipantCannotAccessConversation()
    {
        var (service, db) = CreateServiceWithDb(nameof(Authorization_NonParticipantCannotAccessConversation));
        using var _ = db;
        var (caller, other) = SeedTwoProfiles(db);
        var (_, outsider) = TestDbContextFactory.SeedClientUser(db, "outsider@test.com", "pw", RoleType.Farmer);

        var (conversation, _) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetConversationAsync(outsider.Id, conversation.Id, 1, 50));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.SendMessageAsync(outsider.Id, conversation.Id,
                new SendMessageDto { Content = "Intruder!" }));

        var msg = await service.SendMessageAsync(caller.Id, conversation.Id,
            new SendMessageDto { Content = "Private" });
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.MarkAsReadAsync(outsider.Id, msg.Id));
    }

    [Fact]
    public async Task Validation_EmptyContentInvalidParticipantCountNonExistentIds()
    {
        var (service, db) = CreateServiceWithDb(nameof(Validation_EmptyContentInvalidParticipantCountNonExistentIds));
        using var _ = db;
        var (caller, other) = SeedTwoProfiles(db);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateConversationAsync(caller.Id,
                new CreateConversationDto { ParticipantProfileIds = [caller.Id] }));

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CreateConversationAsync(caller.Id,
                new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id, Guid.NewGuid()] }));

        var nonExistentId = Guid.NewGuid();
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateConversationAsync(caller.Id,
                new CreateConversationDto { ParticipantProfileIds = [caller.Id, nonExistentId] }));

        var (conversation, _) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.SendMessageAsync(caller.Id, conversation.Id,
                new SendMessageDto { Content = "   " }));
    }

    [Fact]
    public async Task DuplicateConversationPrevention_ReturnsExistingConversation()
    {
        var (service, db) = CreateServiceWithDb(nameof(DuplicateConversationPrevention_ReturnsExistingConversation));
        using var _ = db;
        var (caller, other) = SeedTwoProfiles(db);

        var (first, firstIsNew) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });

        var (second, secondIsNew) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });

        Assert.True(firstIsNew);
        Assert.False(secondIsNew);
        Assert.Equal(first.Id, second.Id);

        var count = db.Conversations.Count();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task BookingLinkedConversation_AlwaysCreatesNew()
    {
        var (service, db) = CreateServiceWithDb(nameof(BookingLinkedConversation_AlwaysCreatesNew));
        using var _ = db;
        var (caller, other) = SeedTwoProfiles(db);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, caller.Id);
        var booking = TestDbContextFactory.SeedBooking(db, other.Id, listing.Id, availability.Id);

        var (first, _) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });

        var (bookingConvo, _) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto
            {
                ParticipantProfileIds = [caller.Id, other.Id],
                BookingId = booking.Id
            });

        Assert.NotEqual(first.Id, bookingConvo.Id);
        Assert.Equal(booking.Id, bookingConvo.BookingId);
        Assert.Equal(2, db.Conversations.Count());
    }

    [Fact]
    public async Task Pagination_ConversationsListAndMessages()
    {
        var (service, db) = CreateServiceWithDb(nameof(Pagination_ConversationsListAndMessages));
        using var _ = db;
        var (caller, other) = SeedTwoProfiles(db);

        var (conversation, _) = await service.CreateConversationAsync(caller.Id,
            new CreateConversationDto { ParticipantProfileIds = [caller.Id, other.Id] });

        for (var i = 0; i < 5; i++)
        {
            await service.SendMessageAsync(caller.Id, conversation.Id,
                new SendMessageDto { Content = $"Message {i}" });
        }

        var page1 = await service.GetConversationAsync(caller.Id, conversation.Id, 1, 3);
        Assert.Equal(3, page1.Messages.Items.Count());
        Assert.Equal(5, page1.Messages.TotalCount);
        Assert.Equal(1, page1.Messages.Page);

        var page2 = await service.GetConversationAsync(caller.Id, conversation.Id, 2, 3);
        Assert.Equal(2, page2.Messages.Items.Count());
        Assert.Equal(5, page2.Messages.TotalCount);

        var convList = await service.GetConversationsAsync(caller.Id, 1, 20);
        Assert.Equal(1, convList.TotalCount);
    }
}
