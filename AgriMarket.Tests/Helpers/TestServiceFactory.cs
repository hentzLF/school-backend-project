using AgriMarket.BLL.Services;
using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

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
}
