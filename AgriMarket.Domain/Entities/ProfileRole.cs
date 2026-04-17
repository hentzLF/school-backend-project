using AgriMarket.Domain.Enums;

namespace AgriMarket.Domain.Entities;

public class ProfileRole
{
    public Guid Id {get; set;}
    public Guid UserProfileId {get; set;}
    public UserProfile? UserProfile {get; set;}
    public RoleType Role {get; set;}

}