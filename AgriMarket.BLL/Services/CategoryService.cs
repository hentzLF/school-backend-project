using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync()
    {
        return await _db.ServiceCategories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<ServiceCategory?> GetByIdAsync(Guid id)
    {
        return await _db.ServiceCategories.FindAsync(id);
    }

    public async Task CreateAsync(ServiceCategory category)
    {
        _db.ServiceCategories.Add(category);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceCategory category)
    {
        _db.ServiceCategories.Update(category);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _db.ServiceCategories.FindAsync(id);
        if (category != null)
        {
            _db.ServiceCategories.Remove(category);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<Guid, int>> GetListingCountsAsync()
    {
        return await _db.ServiceListings
            .GroupBy(l => l.ServiceCategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CategoryId, g => g.Count);
    }

    public async Task<int> GetListingCountAsync(Guid categoryId)
    {
        return await _db.ServiceListings.CountAsync(l => l.ServiceCategoryId == categoryId);
    }
}
