using AgriMarket.BLL;
using AgriMarket.BLL.Services;
using AgriMarket.Web.Areas.Client.ViewModels.Bookings;
using AgriMarket.Web.Areas.Client.ViewModels.Listings;
using AgriMarket.Web.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
public class ListingsController(IListingService listingService, IBookingService bookingService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var listings = await listingService.GetAllAsync();

        var vm = new ListingIndexViewModel
        {
            Listings = listings.Select(l =>
            {
                var item = l.ToClientIndexItem();
                return item;
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);
        if (listing == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isOwnListing = userId != null &&
                           listing.ProviderUserId.HasValue &&
                           listing.ProviderUserId.Value.ToString() == userId;

        return View(listing.ToClientDetails(isOwnListing));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ClientOnly")]
    public async Task<IActionResult> Book(CreateBookingViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        try
        {
            var booking = await bookingService.CreateAsync(userId, model.ToCreateBookingDto());
            return RedirectToAction("Details", "Bookings", new { area = "Client", id = booking.Id });
        }
        catch (BusinessRuleException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
