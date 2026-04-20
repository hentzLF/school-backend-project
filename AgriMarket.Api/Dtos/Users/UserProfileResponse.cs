namespace AgriMarket.Api.Dtos.Users;

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public Guid AppUserId { get; set; }
    public string? Email { get; set; }
}
