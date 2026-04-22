using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class CategoryService(
    IRepository<ServiceCategory> categories,
    IRepository<ServiceListing> listings,
    IUnitOfWork uow) : ICategoryService
{
    public async Task<IEnumerable<ServiceCategory>> GetAllAsync()
    {
        return await categories.Query().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<ServiceCategory?> GetByIdAsync(Guid id)
    {
        return await categories.GetByIdAsync(id);
    }

    public async Task CreateAsync(ServiceCategory category)
    {
        categories.Add(category);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceCategory category)
    {
        categories.Update(category);
        await uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await categories.GetByIdAsync(id);
        if (category != null)
        {
            categories.Remove(category);
            await uow.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<Guid, int>> GetListingCountsAsync()
    {
        return await listings.Query()
            .GroupBy(l => l.ServiceCategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CategoryId, g => g.Count);
    }

    public async Task<int> GetListingCountAsync(Guid categoryId)
    {
        return await listings.CountAsync(l => l.ServiceCategoryId == categoryId);
    }
}
