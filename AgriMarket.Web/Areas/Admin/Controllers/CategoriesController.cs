using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class CategoriesController : Controller
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _db.ServiceCategories.OrderBy(c => c.Name).ToListAsync();
        var listingCounts = await _db.ServiceListings
            .GroupBy(l => l.ServiceCategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CategoryId, g => g.Count);

        var vm = new CategoryListViewModel
        {
            TotalCount = categories.Count,
            Categories = categories.Select(c => new CategoryListItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ListingsCount = listingCounts.GetValueOrDefault(c.Id, 0)
            })
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CategoryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var category = new ServiceCategory
        {
            Id = Guid.NewGuid(),
            Name = vm.Name,
            Description = vm.Description
        };

        _db.ServiceCategories.Add(category);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _db.ServiceCategories.FindAsync(id);
        if (category == null) return NotFound();

        var vm = new CategoryEditViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var category = await _db.ServiceCategories.FindAsync(vm.Id);
        if (category == null) return NotFound();

        category.Name = vm.Name;
        category.Description = vm.Description;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _db.ServiceCategories.FindAsync(id);
        if (category == null) return NotFound();

        var listingsCount = await _db.ServiceListings.CountAsync(l => l.ServiceCategoryId == id);

        var vm = new CategoryListItemViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ListingsCount = listingsCount
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var listingsCount = await _db.ServiceListings.CountAsync(l => l.ServiceCategoryId == id);
        if (listingsCount > 0)
        {
            ModelState.AddModelError(string.Empty, "Cannot delete category with existing listings");
            var category = await _db.ServiceCategories.FindAsync(id);
            return View("Delete", new CategoryListItemViewModel
            {
                Id = id,
                Name = category?.Name ?? string.Empty,
                Description = category?.Description,
                ListingsCount = listingsCount
            });
        }

        var cat = await _db.ServiceCategories.FindAsync(id);
        if (cat == null) return NotFound();

        _db.ServiceCategories.Remove(cat);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
