using AgriMarket.DAL;
using AgriMarket.Web.Areas.Client.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Policy = "ClientOnly")]
public class ProfileController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var profile = await GetProfileWithRoleAsync();
        if (profile == null) return Unauthorized();

        var role = profile.Roles?.FirstOrDefault()?.Role ?? default;

        var vm = new ProfileViewModel
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            Role = role
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var profile = await GetProfileWithRoleAsync();
        if (profile == null) return Unauthorized();

        var vm = new EditProfileViewModel
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var profile = await GetProfileAsync();
        if (profile == null) return Unauthorized();

        profile.FirstName = model.FirstName;
        profile.LastName = model.LastName;
        profile.Bio = model.Bio;
        profile.AvatarUrl = model.AvatarUrl;

        await db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Domain.Entities.UserProfile?> GetProfileWithRoleAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return null;
        return await db.UserProfiles
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.AppUserId == Guid.Parse(userId));
    }

    private async Task<Domain.Entities.UserProfile?> GetProfileAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return null;
        return await db.UserProfiles
            .FirstOrDefaultAsync(p => p.AppUserId == Guid.Parse(userId));
    }
}
