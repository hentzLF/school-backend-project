using AgriMarket.BLL.Services;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels.Bookings;
using AgriMarket.Web.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Policy = "ClientOnly")]
public class BookingsController(IBookingService bookingService, IUserService userService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var bookings = await bookingService.GetByClientAsync(clientProfile.Id);

        var vm = new BookingIndexViewModel
        {
            Bookings = bookings.Select(b => b.ToClientIndexItem())
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var booking = await bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();

        if (booking.ClientProfileId != clientProfile.Id)
            return RedirectToAction("AccessDenied", "Account");

        return View(booking.ToClientDetailsVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(Guid id)
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var booking = await bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();

        if (booking.ClientProfileId != clientProfile.Id)
            return RedirectToAction("AccessDenied", "Account");

        if (booking.Status != BookingStatus.AwaitingPayment)
            return RedirectToAction(nameof(Details), new { id });

        await bookingService.UpdateStatusAsync(id, BookingStatus.Confirmed, clientProfile.Id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var booking = await bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();

        if (booking.ClientProfileId != clientProfile.Id)
            return RedirectToAction("AccessDenied", "Account");

        await bookingService.UpdateStatusAsync(id, BookingStatus.Cancelled, clientProfile.Id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCompletion(Guid id)
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var booking = await bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();

        if (booking.ClientProfileId != clientProfile.Id)
            return RedirectToAction("AccessDenied", "Account");

        if (booking.Status != BookingStatus.ProviderCompleted)
            return RedirectToAction(nameof(Details), new { id });

        await bookingService.UpdateStatusAsync(id, BookingStatus.ClientConfirmed, clientProfile.Id);
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<AgriMarket.BLL.Dtos.Users.UserProfileDto?> GetClientProfileAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return null;
        return await userService.GetProfileByUserIdAsync(userId);
    }
}
