using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels.MyListings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Web.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Policy = "ProviderOnly")]
    public class MyListingsController : Controller
    {
        private readonly AppDbContext _context;

        public MyListingsController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<UserProfile?> GetProviderProfileAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                return null;
            }

            return await _context.UserProfiles.FirstOrDefaultAsync(u => u.AppUserId == userId);
        }

        public async Task<IActionResult> Index()
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listings = await _context.ServiceListings
                .Include(l => l.ServiceCategory)
                .Where(l => l.UserProfileId == profile.Id)
                .OrderBy(l => l.Title)
                .Select(l => new MyListingIndexItemViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    CategoryName = l.ServiceCategory!.Name,
                    PricePerHectare = l.PricePerHectare,
                    IsActive = l.IsActive
                })
                .ToListAsync();

            var viewModel = new MyListingIndexViewModel
            {
                Listings = listings
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .Include(l => l.ServiceCategory)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            var bookingCount = await _context.Bookings
                .CountAsync(b => b.ServiceListingId == id);

            var viewModel = new MyListingDetailsViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                CategoryName = listing.ServiceCategory!.Name,
                PricePerHectare = listing.PricePerHectare,
                IsActive = listing.IsActive,
                TotalBookingCount = bookingCount
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _context.ServiceCategories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            var viewModel = new MyListingCreateViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MyListingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.ServiceCategories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();
                return View(model);
            }

            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = new ServiceListing
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                ServiceCategoryId = model.ServiceCategoryId,
                PricePerHectare = model.PricePerHectare,
                UserProfileId = profile.Id,
                IsActive = false
            };

            _context.ServiceListings.Add(listing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = listing.Id });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            var categories = await _context.ServiceCategories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            var viewModel = new MyListingEditViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                ServiceCategoryId = listing.ServiceCategoryId,
                PricePerHectare = listing.PricePerHectare,
                IsActive = listing.IsActive,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MyListingEditViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Categories = await _context.ServiceCategories
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();
                return View(model);
            }

            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            listing.Title = model.Title;
            listing.Description = model.Description;
            listing.ServiceCategoryId = model.ServiceCategoryId;
            listing.PricePerHectare = model.PricePerHectare;
            listing.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = listing.Id });
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            var hasActiveBookings = await _context.Bookings
                .AnyAsync(b => b.ServiceListingId == id && !new[] { BookingStatus.Archived, BookingStatus.Cancelled, BookingStatus.ClientConfirmed }.Contains(b.Status));

            ViewBag.HasActiveBookings = hasActiveBookings;

            return View(listing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            var hasActiveBookings = await _context.Bookings
                .AnyAsync(b => b.ServiceListingId == id && !new[] { BookingStatus.Archived, BookingStatus.Cancelled, BookingStatus.ClientConfirmed }.Contains(b.Status));

            if (hasActiveBookings)
            {
                ViewBag.HasActiveBookings = true;
                return View("Delete", listing);
            }

            _context.ServiceListings.Remove(listing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            listing.IsActive = !listing.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = listing.Id });
        }

        public async Task<IActionResult> Availabilities(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .Include(l => l.Availabilities)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            var availabilities = (listing.Availabilities ?? new List<Availability>())
                .OrderBy(a => a.StartTime)
                .Select(a => new AvailabilityItemViewModel
                {
                    Id = a.Id,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsBooked = a.IsBooked
                })
                .ToList();

            var viewModel = new ManageAvailabilitiesViewModel
            {
                ListingId = listing.Id,
                ListingTitle = listing.Title,
                Availabilities = availabilities,
                AddStartTime = DateTime.Today.AddDays(1).AddHours(8),
                AddEndTime = DateTime.Today.AddDays(1).AddHours(17)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAvailability(Guid listingId, ManageAvailabilitiesViewModel model)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == listingId && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            if (model.AddStartTime >= model.AddEndTime)
            {
                ModelState.AddModelError(string.Empty, "Start time must be before end time.");
            }

            if (!ModelState.IsValid)
            {
                // Reload list to re-render
                var availabilities = await _context.Availabilities
                    .Where(a => a.ServiceListingId == listingId)
                    .OrderBy(a => a.StartTime)
                    .Select(a => new AvailabilityItemViewModel
                    {
                        Id = a.Id,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        IsBooked = a.IsBooked
                    })
                    .ToListAsync();
                
                model.ListingId = listing.Id;
                model.ListingTitle = listing.Title;
                model.Availabilities = availabilities;

                return View("Availabilities", model);
            }

            var availability = new Availability
            {
                Id = Guid.NewGuid(),
                ServiceListingId = listingId,
                StartTime = DateTime.SpecifyKind(model.AddStartTime, DateTimeKind.Utc),
                EndTime = DateTime.SpecifyKind(model.AddEndTime, DateTimeKind.Utc),
                IsBooked = false
            };

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Availabilities), new { id = listingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAvailability(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var availability = await _context.Availabilities
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (availability == null || availability.ServiceListing!.UserProfileId != profile.Id)
            {
                return NotFound();
            }

            if (availability.IsBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete a booked availability slot.";
                return RedirectToAction(nameof(Availabilities), new { id = availability.ServiceListingId });
            }

            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Availabilities), new { id = availability.ServiceListingId });
        }

        public async Task<IActionResult> Bookings(Guid id)
        {
            var profile = await GetProviderProfileAsync();
            if (profile == null) return NotFound();

            var listing = await _context.ServiceListings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserProfileId == profile.Id);

            if (listing == null) return NotFound();

            var bookings = await _context.Bookings
                .Include(b => b.ClientProfile)
                .Where(b => b.ServiceListingId == id)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingsForListingItemViewModel
                {
                    Id = b.Id,
                    ClientName = b.ClientProfile!.FirstName + " " + b.ClientProfile.LastName,
                    Status = b.Status.ToString(),
                    AreaInHectares = b.AreaInHectares,
                    TotalPrice = b.TotalPrice,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            var viewModel = new BookingsForListingViewModel
            {
                ListingId = listing.Id,
                ListingTitle = listing.Title,
                Bookings = bookings
            };

            return View(viewModel);
        }
    }
}