namespace AgriMarket.Domain.Entities;

public class ConversationParticipant
{
    public Guid UserProfileId { get; set; }

    public DateTime JoinedAt { get; set; }

    // FK
    public Guid ConversationId { get; set; }

    // Navigation
    public Conversation? Conversation { get; set; }
    public UserProfile? UserProfile { get; set; }
}