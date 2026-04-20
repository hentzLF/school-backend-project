namespace AgriMarket.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set;}

    public string Email { get; set;} = default!;

    public string PasswordHash { get; set; } = default!;

    public DateTime? LockoutEnd {get; set;}

    // Navigation
    public ICollection<UserProfile>? Profiles {get; set;}
    public ICollection<OAuthAccount>? OAuthAccounts { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
}