using AgriMarket.BLL.Services;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();

        var now = DateTime.UtcNow;
        var vm = new UserListViewModel
        {
            TotalCount = users.Count(),
            Users = users.Select(u => new UserListItemViewModel
            {
                Id = u.Id,
                Email = u.Email,
                ProfilesCount = u.Profiles?.Count ?? 0,
                Roles = u.Profiles?
                    .SelectMany(p => p.Roles ?? [])
                    .Select(r => r.Role)
                    .Distinct()
                    .ToList() ?? [],
                CreatedAt = u.CreatedAt,
                IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > now,
                LockoutEnd = u.LockoutEnd
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null) return NotFound();

        var now = DateTime.UtcNow;
        var vm = new UserDetailViewModel
        {
            Id = user.Id,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > now,
            LockoutEnd = user.LockoutEnd,
            Profiles = user.Profiles?.Select(p => new UserProfileDetailViewModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Bio = p.Bio,
                AvatarUrl = p.AvatarUrl,
                Roles = p.Roles?.Select(r => r.Role).ToList() ?? []
            }) ?? [],
            BookingsCount = user.Profiles?.Sum(p => p.ClientBookings?.Count ?? 0) ?? 0,
            ListingsCount = user.Profiles?.Sum(p => p.ServiceListings?.Count ?? 0) ?? 0
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            Email = user.Email,
            LockoutEnd = user.LockoutEnd
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userService.GetUserByIdAsync(vm.Id);
        if (user == null) return NotFound();

        user.Email = vm.Email;
        user.LockoutEnd = vm.LockoutEnd;
        await _userService.UpdateUserAsync(user);

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null) return NotFound();

        var now = DateTime.UtcNow;
        var vm = new UserListItemViewModel
        {
            Id = user.Id,
            Email = user.Email,
            ProfilesCount = user.Profiles?.Count ?? 0,
            Roles = user.Profiles?
                .SelectMany(p => p.Roles ?? [])
                .Select(r => r.Role)
                .Distinct()
                .ToList() ?? [],
            CreatedAt = user.CreatedAt,
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > now,
            LockoutEnd = user.LockoutEnd
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lock(Guid id)
    {
        await _userService.LockUserAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(Guid id)
    {
        await _userService.UnlockUserAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }
}
