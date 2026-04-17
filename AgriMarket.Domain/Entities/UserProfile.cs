namespace AgriMarket.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }
    
    public AppUser? AppUser { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    // Navigation: see profile kuulub ühele AppUser-ile

    // Navigation: sellel profile-il on rollid
    public ICollection<ProfileRole>? Roles { get; set; }
}