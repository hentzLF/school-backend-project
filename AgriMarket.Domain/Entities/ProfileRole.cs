using AgriMarket.Domain.Enums;

namespace AgriMarket.Domain.Entities;

public class ProfileRole
{
    public Guid Id {get; set;}
    
    // Foreign Key
    public Guid UserProfileId {get; set;}

    // Navigation
    public UserProfile? UserProfile {get; set;}
    public RoleType Role {get; set;}
}