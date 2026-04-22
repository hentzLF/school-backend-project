using AgriMarket.Api.Dtos.Users;
using AgriMarket.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _userService.GetAllProfilesAsync(page, pageSize);
        var items = result.Items.Select(up => new UserProfileResponse
        {
            Id = up.Id,
            FirstName = up.FirstName,
            LastName = up.LastName,
            Bio = up.Bio,
            AvatarUrl = up.AvatarUrl,
            AppUserId = up.AppUserId
        });

        return Ok(new { items, page, pageSize, totalCount = result.TotalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var callerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var profile = await _userService.GetProfileByIdAsync(id);

        if (profile is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"UserProfile {id} not found.");

        var isOwner = callerUserId != null && profile.AppUserId.ToString() == callerUserId;

        return Ok(new UserProfileResponse
        {
            Id = profile.Id,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            AppUserId = profile.AppUserId,
            Email = isOwner ? profile.AppUser?.Email : null
        });
    }
}
