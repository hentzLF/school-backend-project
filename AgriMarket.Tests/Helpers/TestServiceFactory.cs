using AgriMarket.BLL.Services;
using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using EquipmentEntity = AgriMarket.Domain.Entities.Equipment;

namespace AgriMarket.Tests.Helpers;

public static class TestServiceFactory
{
    public static ReviewService CreateReviewService(AppDbContext db) =>
        new(new EfRepository<Review>(db),
            new EfRepository<UserProfile>(db),
            new EfBookingRepository(db),
            new EfUnitOfWork(db),
            new EfQueryMaterializer(),
            NullLogger<ReviewService>.Instance);

    public static EquipmentService CreateEquipmentService(AppDbContext db) =>
        new(new EfRepository<EquipmentEntity>(db),
            new EfRepository<ServiceListing>(db),
            new EfRepository<ServiceListingEquipment>(db),
            new EfUnitOfWork(db),
            new EfQueryMaterializer(),
            NullLogger<EquipmentService>.Instance);

    public static ClientPaymentService CreateClientPaymentService(AppDbContext db) =>
        new(new EfBookingRepository(db),
            new EfRepository<Payment>(db),
            new EfUnitOfWork(db),
            new EfQueryMaterializer());

    public static PaymentService CreatePaymentService(AppDbContext db) =>
        new(new EfPaymentRepository(db),
            new EfUnitOfWork(db),
            new EfQueryMaterializer(),
            NullLogger<PaymentService>.Instance);
}
