using AgriMarket.BLL.Dtos.Users;
using AgriMarket.Domain.Entities;

namespace AgriMarket.BLL.Mappers;

public static class UserMapper
{
    public static UserProfileDto ToUserProfileDto(this UserProfile profile, string? email) =>
        new()
        {
            Id = profile.Id,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            AppUserId = profile.AppUserId,
            Email = email
        };
}
