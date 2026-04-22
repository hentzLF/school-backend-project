using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
public class AccountController(AppDbContext db) : Controller
{

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "Your account is locked");
            return View(model);
        }

        var clientProfile = user.Profiles?
            .FirstOrDefault(p => p.Roles != null &&
                p.Roles.Any(r => r.Role == RoleType.Farmer || r.Role == RoleType.Provider));

        if (clientProfile == null)
        {
            ModelState.AddModelError(string.Empty, "You do not have client access");
            return View(model);
        }

        var role = clientProfile.Roles!.First(r => r.Role == RoleType.Farmer || r.Role == RoleType.Provider).Role;
        await SignInAsync(user, clientProfile, role);
        return RedirectToAction("Index", "Listings", new { area = "Client" });
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Role != RoleType.Farmer && model.Role != RoleType.Provider)
        {
            ModelState.AddModelError(nameof(model.Role), "Role must be Farmer or Provider");
            return View(model);
        }

        var emailExists = await db.AppUsers.AnyAsync(u => u.Email == model.Email);
        if (emailExists)
        {
            ModelState.AddModelError(string.Empty, "An account with this email already exists");
            return View(model);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            CreatedAt = DateTime.UtcNow
        };

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            AppUserId = user.Id
        };

        var profileRole = new ProfileRole
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Role = model.Role
        };

        db.AppUsers.Add(user);
        db.UserProfiles.Add(profile);
        db.ProfileRoles.Add(profileRole);
        await db.SaveChangesAsync();

        await SignInAsync(user, profile, model.Role);
        return RedirectToAction("Index", "Listings", new { area = "Client" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(AppUser user, UserProfile profile, RoleType role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{profile.FirstName} {profile.LastName}"),
            new(ClaimTypes.Role, role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
