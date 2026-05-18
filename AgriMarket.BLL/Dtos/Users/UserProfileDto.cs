using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Dtos.Users;

public sealed class UserProfileDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string? Bio { get; init; }
    public string? AvatarUrl { get; init; }
    public Guid AppUserId { get; init; }
    public string? Email { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsLocked { get; init; }
    public DateTime? LockoutEnd { get; init; }
    public IEnumerable<RoleType> Roles { get; init; } = [];
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
