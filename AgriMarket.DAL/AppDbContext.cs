using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers {get; set;} = default!;
    public DbSet<UserProfile> UserProfiles {get; set;} = default!;
    public DbSet<ProfileRole> ProfileRoles {get; set;} = default!;
    public DbSet<Location> Locations  {get; set;} = default!;
    public DbSet<ServiceCategory> ServiceCategories {get; set;} = default!;
}
