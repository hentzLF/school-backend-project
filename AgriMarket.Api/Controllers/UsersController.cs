using AgriMarket.Api.Mappers;
using AgriMarket.BLL.Dtos.Users;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IUserService userService) : ApiControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _userService.GetAllProfilesAsync(page, pageSize);
        var items = result.Items.Select(UserApiMapper.HideEmail);
        return Ok(new { items, page, pageSize, totalCount = result.TotalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var callerUserId = GetCallerUserId();
        var isAdmin = User.Claims.Any(c =>
            c.Type == ClaimTypes.Role && string.Equals(c.Value, "Admin", StringComparison.OrdinalIgnoreCase));

        var profile = await _userService.GetProfileByIdAsync(id, callerUserId, isAdmin);
        if (profile is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"UserProfile {id} not found.");

        return Ok(profile);
    }

}
