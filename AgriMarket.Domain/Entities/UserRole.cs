using AgriMarket.Domain.Enums;

namespace AgriMarket.Domain.Entities;

public class UserRole
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }

    public RoleType Role { get; set; }

    // Navigation
    public AppUser? AppUser { get; set; }
}
