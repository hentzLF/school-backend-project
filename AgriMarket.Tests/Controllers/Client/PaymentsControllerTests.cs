using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class PaymentsControllerTests
{
    private static AgriMarket.BLL.Services.UserService CreateUserService(AppDbContext db) =>
        new(new EfAppUserRepository(db),
            new EfUserProfileRepository(db),
            new EfRepository<ProfileRole>(db),
            new EfUnitOfWork(db),
            new EfRepository<MessageRead>(db),
            new EfRepository<Message>(db),
            new EfRepository<ConversationParticipant>(db),
            new EfRepository<Review>(db),
            new EfRepository<Booking>(db),
            new EfRepository<ServiceListing>(db),
            TestServiceFactory.CreateReviewService(db),
            NullLogger<AgriMarket.BLL.Services.UserService>.Instance);

    private static PaymentsController CreateController(AppDbContext db, Guid userId)
    {
        var controller = new PaymentsController(
            TestServiceFactory.CreatePaymentService(db),
            TestServiceFactory.CreateClientPaymentService(db),
            CreateUserService(db));
        controller.ControllerContext = ControllerContextFactory.WithAuthenticatedUser(userId);
        return controller;
    }

    private static Payment SeedPayment(AppDbContext db, Guid bookingId)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Amount = 100m,
            PlatformFee = 5m,
            Method = PaymentMethod.Card,
            Status = PaymentStatus.Held,
            CreatedAt = DateTime.UtcNow
        };
        db.Payments.Add(payment);
        db.SaveChanges();
        return payment;
    }

    [Fact]
    public async Task Receipt_WithValidPayment_ReturnsView()
    {
        using var db = TestDbContextFactory.Create(nameof(Receipt_WithValidPayment_ReturnsView));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.Confirmed);
        var payment = SeedPayment(db, booking.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.Receipt(payment.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<ReceiptViewModel>(viewResult.Model);
        Assert.Equal(payment.Id, vm.PaymentId);
        Assert.Equal(booking.Id, vm.BookingId);
    }

    [Fact]
    public async Task Receipt_WithNonExistentPayment_ReturnsNotFound()
    {
        using var db = TestDbContextFactory.Create(nameof(Receipt_WithNonExistentPayment_ReturnsNotFound));
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);

        var controller = CreateController(db, user.Id);

        var result = await controller.Receipt(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Index_WithPayments_ReturnsViewWithHistory()
    {
        using var db = TestDbContextFactory.Create(nameof(Index_WithPayments_ReturnsViewWithHistory));
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);
        var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);
        var booking = TestDbContextFactory.SeedBooking(db, profile.Id, listing.Id, availability.Id, BookingStatus.Confirmed);
        SeedPayment(db, booking.Id);

        var controller = CreateController(db, user.Id);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<PaymentHistoryViewModel>(viewResult.Model);
        Assert.Single(vm.Payments);
    }

    [Fact]
    public async Task Index_WithNoPayments_ReturnsEmptyView()
    {
        using var db = TestDbContextFactory.Create(nameof(Index_WithNoPayments_ReturnsEmptyView));
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "client@test.com", "pw", RoleType.Farmer);

        var controller = CreateController(db, user.Id);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<PaymentHistoryViewModel>(viewResult.Model);
        Assert.Empty(vm.Payments);
    }
}
