using AgriMarket.DAL;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class ListingsController : Controller
{
    private readonly AppDbContext _db;

    public ListingsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(bool? active)
    {
        var query = _db.ServiceListings
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .AsQueryable();

        if (active.HasValue)
            query = query.Where(l => l.IsActive == active.Value);

        var listings = await query.OrderBy(l => l.Title).ToListAsync();

        var vm = new ListingListViewModel
        {
            TotalCount = listings.Count,
            FilterActive = active,
            Listings = listings.Select(l => new ListingListItemViewModel
            {
                Id = l.Id,
                Title = l.Title,
                ProviderName = l.UserProfile != null
                    ? $"{l.UserProfile.FirstName} {l.UserProfile.LastName}"
                    : "Unknown",
                CategoryName = l.ServiceCategory?.Name ?? "Unknown",
                PricePerHectare = l.PricePerHectare,
                IsActive = l.IsActive
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await _db.ServiceListings
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .Include(l => l.Equipments)
            .Include(l => l.Availabilities)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (listing == null) return NotFound();

        var bookingsCount = await _db.Bookings.CountAsync(b => b.ServiceListingId == id);

        var vm = new ListingDetailViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            ProviderName = listing.UserProfile != null
                ? $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}"
                : "Unknown",
            ProviderProfileId = listing.UserProfileId,
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            CategoryId = listing.ServiceCategoryId,
            BookingsCount = bookingsCount,
            Equipments = listing.Equipments?.Select(e => new ListingEquipmentViewModel
            {
                Name = e.Name,
                Model = e.Model,
                ManufactureYear = e.ManufactureYear
            }) ?? [],
            Availabilities = listing.Availabilities == null ? [] : listing.Availabilities.Select(a => new ListingAvailabilityViewModel
            {
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                IsBooked = a.IsBooked
            }).OrderBy(a => a.StartTime)
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var listing = await _db.ServiceListings.FindAsync(id);
        if (listing == null) return NotFound();

        var vm = new ListingEditViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            ServiceCategoryId = listing.ServiceCategoryId,
            Categories = await GetCategorySelectList(listing.ServiceCategoryId)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ListingEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await GetCategorySelectList(vm.ServiceCategoryId);
            return View(vm);
        }

        var listing = await _db.ServiceListings.FindAsync(vm.Id);
        if (listing == null) return NotFound();

        listing.Title = vm.Title;
        listing.Description = vm.Description;
        listing.PricePerHectare = vm.PricePerHectare;
        listing.IsActive = vm.IsActive;
        listing.ServiceCategoryId = vm.ServiceCategoryId;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var listing = await _db.ServiceListings
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (listing == null) return NotFound();

        var vm = new ListingListItemViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            ProviderName = listing.UserProfile != null
                ? $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}"
                : "Unknown",
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var listing = await _db.ServiceListings.FindAsync(id);
        if (listing == null) return NotFound();

        _db.ServiceListings.Remove(listing);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var listing = await _db.ServiceListings.FindAsync(id);
        if (listing == null) return NotFound();

        listing.IsActive = !listing.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<IEnumerable<SelectListItem>> GetCategorySelectList(Guid selectedId)
    {
        var categories = await _db.ServiceCategories.OrderBy(c => c.Name).ToListAsync();
        return categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = c.Id == selectedId
        });
    }
}
