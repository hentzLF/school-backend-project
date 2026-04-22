namespace AgriMarket.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string Content { get; set; } = default!;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }

    //FK
    public Guid UserProfileId { get; set; }

    // Navigation
    public UserProfile? UserProfile { get; set; }
}