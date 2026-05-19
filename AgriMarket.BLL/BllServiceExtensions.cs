using AgriMarket.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AgriMarket.BLL;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IListingService, ListingService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IClientPaymentService, ClientPaymentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IProviderDashboardService, ProviderDashboardService>();
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<ILocationLookupService, LocationLookupService>();

        return services;
    }
}
