using AgriMarket.BLL.Contracts;
using AgriMarket.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AgriMarket.DAL;

public static class DalServiceExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        services.AddScoped<IQueryMaterializer, EfQueryMaterializer>();
        services.AddScoped<IAppUserRepository, EfAppUserRepository>();
        services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();
        services.AddScoped<IBookingRepository, EfBookingRepository>();
        services.AddScoped<IListingRepository, EfListingRepository>();
        services.AddScoped<IUserProfileRepository, EfUserProfileRepository>();
        services.AddScoped<IAvailabilityRepository, EfAvailabilityRepository>();
        services.AddScoped<IPaymentRepository, EfPaymentRepository>();
        return services;
    }
}
