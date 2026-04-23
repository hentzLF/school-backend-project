using AgriMarket.BLL.Dtos;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class ProviderDashboardService(
    IRepository<Booking> bookings,
    IRepository<Payment> payments,
    IRepository<ServiceListing> serviceListings) : IProviderDashboardService
{
    public async Task<ProviderDashboardDto> GetStatsAsync(Guid providerProfileId)
    {
        var listingIds = await serviceListings.Query()
            .Where(l => l.UserProfileId == providerProfileId)
            .Select(l => l.Id)
            .ToListAsync();

        var providerBookings = bookings.Query()
            .Where(b => listingIds.Contains(b.ServiceListingId));

        var activeStatuses = new[]
        {
            BookingStatus.Pending, BookingStatus.AwaitingPayment,
            BookingStatus.Confirmed, BookingStatus.InProgress
        };

        var activeBookings = await providerBookings.CountAsync(b => activeStatuses.Contains(b.Status));
        var completedBookings = await providerBookings.CountAsync(b => b.Status == BookingStatus.ClientConfirmed);
        var cancelledBookings = await providerBookings.CountAsync(b => b.Status == BookingStatus.Cancelled);

        var providerPayments = payments.Query()
            .Where(p => listingIds.Contains(p.Booking!.ServiceListingId));

        var totalEarnings = await providerPayments
            .Where(p => p.Status == PaymentStatus.Released)
            .Select(p => (decimal?)p.Amount).SumAsync() ?? 0m;

        var moneyHeld = await providerPayments
            .Where(p => p.Status == PaymentStatus.Held)
            .Select(p => (decimal?)p.Amount).SumAsync() ?? 0m;

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
