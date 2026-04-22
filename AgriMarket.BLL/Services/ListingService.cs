using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class ListingService : IListingService
{
    private readonly AppDbContext _db;

    public ListingService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ServiceListing>> GetAllAsync()
    {
        return await _db.ServiceListings
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .OrderBy(l => l.Title)
            .ToListAsync();
    }

    public async Task<ServiceListing?> GetByIdAsync(Guid id)
    {
        return await _db.ServiceListings
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .Include(l => l.Equipments)
            .Include(l => l.Availabilities)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<ServiceListing>> GetByProviderAsync(Guid providerProfileId)
    {
        return await _db.ServiceListings
            .Include(l => l.ServiceCategory)
            .Where(l => l.UserProfileId == providerProfileId)
            .OrderBy(l => l.Title)
            .ToListAsync();
    }

    public async Task<ServiceListing> CreateAsync(ServiceListing listing)
    {
        _db.ServiceListings.Add(listing);
        await _db.SaveChangesAsync();
        return listing;
    }

    public async Task UpdateAsync(ServiceListing listing)
    {
        _db.ServiceListings.Update(listing);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var listing = await _db.ServiceListings.FindAsync(id);
        if (listing != null)
        {
            _db.ServiceListings.Remove(listing);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ServiceListing>> GetActiveListingsAsync()
    {
        return await _db.ServiceListings
            .Where(l => l.IsActive)
            .Include(l => l.UserProfile)
            .Include(l => l.ServiceCategory)
            .OrderBy(l => l.Title)
            .ToListAsync();
    }

    public async Task ToggleActiveAsync(Guid id)
    {
        var listing = await _db.ServiceListings.FindAsync(id);
        if (listing != null)
        {
            listing.IsActive = !listing.IsActive;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Availability> AddAvailabilityAsync(Availability availability)
    {
        _db.Availabilities.Add(availability);
        await _db.SaveChangesAsync();
        return availability;
    }

    public async Task DeleteAvailabilityAsync(Guid id)
    {
        var availability = await _db.Availabilities.FindAsync(id);
        if (availability != null)
        {
            _db.Availabilities.Remove(availability);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Availability?> GetAvailabilityByIdAsync(Guid id)
    {
        return await _db.Availabilities
            .Include(a => a.ServiceListing)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAvailabilityAsync(Availability availability)
    {
        _db.Availabilities.Update(availability);
        await _db.SaveChangesAsync();
    }
}
