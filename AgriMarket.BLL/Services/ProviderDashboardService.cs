using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public class ProviderDashboardService(
    IRepository<Booking> bookings,
    IRepository<Payment> payments,
    IRepository<ServiceListing> serviceListings,
    IQueryMaterializer mat) : IProviderDashboardService
{
    public async Task<ProviderDashboardDto> GetStatsAsync(Guid providerProfileId)
    {
        var listingIds = await mat.ToListAsync(
            serviceListings.Query()
                .Where(l => l.UserProfileId == providerProfileId)
                .Select(l => l.Id));

        var providerBookings = bookings.Query()
            .Where(b => listingIds.Contains(b.ServiceListingId));

        var activeStatuses = new[]
        {
            BookingStatus.Pending, BookingStatus.AwaitingPayment,
            BookingStatus.Confirmed, BookingStatus.InProgress
        };

        var activeBookings = await mat.CountAsync(providerBookings.Where(b => activeStatuses.Contains(b.Status)));
        var completedBookings = await mat.CountAsync(providerBookings.Where(b => b.Status == BookingStatus.ClientConfirmed));
        var cancelledBookings = await mat.CountAsync(providerBookings.Where(b => b.Status == BookingStatus.Cancelled));

        var providerPayments = payments.Query()
            .Where(p => listingIds.Contains(p.Booking!.ServiceListingId));

        var totalEarnings = await mat.SumAsync(
            providerPayments.Where(p => p.Status == PaymentStatus.Released),
            p => (decimal?)p.Amount);

        var moneyHeld = await mat.SumAsync(
            providerPayments.Where(p => p.Status == PaymentStatus.Held),
            p => (decimal?)p.Amount);

        var activeListings = await serviceListings.CountAsync(l => l.UserProfileId == providerProfileId && l.IsActive);
        var totalListings = await serviceListings.CountAsync(l => l.UserProfileId == providerProfileId);

        return new ProviderDashboardDto
        {
            TotalEarnings = totalEarnings,
            MoneyHeld = moneyHeld,
            ActiveBookings = activeBookings,
            CompletedBookings = completedBookings,
            CancelledBookings = cancelledBookings,
            ActiveListings = activeListings,
            TotalListings = totalListings
        };
    }
}
