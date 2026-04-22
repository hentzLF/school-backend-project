using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels.Bookings;
using AgriMarket.Web.Areas.Client.ViewModels.Listings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
public class ListingsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var listings = await db.ServiceListings
            .Where(l => l.IsActive)
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .OrderBy(l => l.Title)
            .ToListAsync();

        var vm = new ListingIndexViewModel
        {
            Listings = listings.Select(l => new ListingIndexItemViewModel
            {
                Id = l.Id,
                Title = l.Title,
                CategoryName = l.ServiceCategory?.Name ?? "Unknown",
                ProviderName = l.UserProfile != null
                    ? $"{l.UserProfile.FirstName} {l.UserProfile.LastName}"
                    : "Unknown",
                PricePerHectare = l.PricePerHectare
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await db.ServiceListings
            .Where(l => l.Id == id && l.IsActive)
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .Include(l => l.Availabilities)
            .FirstOrDefaultAsync();

        if (listing == null) return NotFound();

        var vm = new ListingDetailsViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            ProviderName = listing.UserProfile != null
                ? $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}"
                : "Unknown",
            Availabilities = listing.Availabilities?
                .Where(a => !a.IsBooked)
                .OrderBy(a => a.StartTime)
                .Select(a => new AvailabilityOptionViewModel
                {
                    Id = a.Id,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }) ?? []
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ClientOnly")]
    public async Task<IActionResult> Book(CreateBookingViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var clientProfile = await db.UserProfiles
            .FirstOrDefaultAsync(p => p.AppUserId == Guid.Parse(userId));

        if (clientProfile == null) return Unauthorized();

        var listing = await db.ServiceListings
            .FirstOrDefaultAsync(l => l.Id == model.ServiceListingId && l.IsActive);

        if (listing == null) return NotFound();

        var availability = await db.Availabilities
            .FirstOrDefaultAsync(a => a.Id == model.AvailabilityId
                && a.ServiceListingId == model.ServiceListingId
                && !a.IsBooked);

        if (availability == null)
        {
            ModelState.AddModelError(string.Empty, "The selected availability is no longer available.");
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ServiceListingId = model.ServiceListingId,
            ClientProfileId = clientProfile.Id,
            AvailabilityId = model.AvailabilityId,
            AreaInHectares = model.AreaInHectares,
            TotalPrice = (decimal)model.AreaInHectares * listing.PricePerHectare,
            Notes = model.Notes,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        availability.IsBooked = true;

        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        return RedirectToAction("Details", "Bookings", new { area = "Client", id = booking.Id });
    }
}
