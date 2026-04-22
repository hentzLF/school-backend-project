using AgriMarket.BLL.Services;
using AgriMarket.Web.Areas.Client.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Policy = "ClientOnly")]
public class ProfileController(IUserService userService) : Controller
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

        await userService.UpdateProfileAsync(profile);

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Domain.Entities.UserProfile?> GetProfileWithRoleAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return null;
        return await userService.GetProfileByUserIdAsync(userId, includeRoles: true);
    }

    private async Task<Domain.Entities.UserProfile?> GetProfileAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return null;
        return await userService.GetProfileByUserIdAsync(userId, includeRoles: false);
    }
}
