using AgriMarket.Api.Dtos.Users;
using AgriMarket.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var query = _db.UserProfiles.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(up => new UserProfileResponse
            {
                Id = up.Id,
                FirstName = up.FirstName,
                LastName = up.LastName,
                Bio = up.Bio,
                AvatarUrl = up.AvatarUrl,
                AppUserId = up.AppUserId
            })
            .ToListAsync();

        return Ok(new { items, page, pageSize, totalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var profile = await _db.UserProfiles.AsNoTracking()
            .Include(up => up.AppUser)
            .Where(up => up.Id == id)
            .Select(up => new UserProfileResponse
            {
                Id = up.Id,
                FirstName = up.FirstName,
                LastName = up.LastName,
                Bio = up.Bio,
                AvatarUrl = up.AvatarUrl,
                AppUserId = up.AppUserId,
                Email = up.AppUser != null ? up.AppUser.Email : null
            })
            .FirstOrDefaultAsync();

        if (profile is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"UserProfile {id} not found.");

        return Ok(profile);
    }
}
