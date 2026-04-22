using Microsoft.Extensions.DependencyInjection;

namespace AgriMarket.DAL;

public static class DalServiceExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }
}
