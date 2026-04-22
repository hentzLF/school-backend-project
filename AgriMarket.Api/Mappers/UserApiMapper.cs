using AgriMarket.BLL.Dtos.Users;

namespace AgriMarket.Api.Mappers;

public static class UserApiMapper
{
    public static UserProfileDto HideEmail(this UserProfileDto dto)
    {
        return new UserProfileDto
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl,
            AppUserId = dto.AppUserId,
            Email = null
        };
    }
}
