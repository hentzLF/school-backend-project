namespace AgriMarket.Domain.Entities;

public class OAuthAccount
{
    public Guid Id { get; set; }

    public string Provider { get; set; } = default!;

    public string ProviderAccountId { get; set; } = default!;

    //FK
    public Guid AppUserId { get; set; }

    // Navigation
    public AppUser? AppUser { get; set; }
}