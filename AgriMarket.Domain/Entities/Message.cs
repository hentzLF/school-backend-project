namespace AgriMarket.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderProfileId { get; set; }

    public string Content { get; set; } = default!;

    public DateTime SentAt { get; set; }

    // Navigation
    public Conversation? Conversation { get; set; }
    public UserProfile? SenderProfile { get; set; }
    public ICollection<MessageRead>? MessageReads { get; set; }
}