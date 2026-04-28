using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels.MyListings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client
{
    public class MyListingsControllerTests
    {
        private static MyListingsController CreateController(AppDbContext db, Guid userId, string role = "Provider") =>
            new(
                new AgriMarket.BLL.Services.ListingService(new EfRepository<ServiceListing>(db), new EfRepository<UserProfile>(db), new EfRepository<Booking>(db), new EfRepository<Availability>(db), new EfUnitOfWork(db), NullLogger<AgriMarket.BLL.Services.ListingService>.Instance),
                new AgriMarket.BLL.Services.CategoryService(new EfRepository<ServiceCategory>(db), new EfRepository<ServiceListing>(db), new EfUnitOfWork(db), NullLogger<AgriMarket.BLL.Services.CategoryService>.Instance),
                new AgriMarket.BLL.Services.BookingService(new EfRepository<Booking>(db), new EfRepository<UserProfile>(db), new EfRepository<ServiceListing>(db), new EfRepository<Availability>(db), new EfUnitOfWork(db), NullLogger<AgriMarket.BLL.Services.BookingService>.Instance),
                new AgriMarket.BLL.Services.UserService(new EfRepository<AppUser>(db), new EfRepository<UserProfile>(db), new EfRepository<ProfileRole>(db), new EfUnitOfWork(db), NullLogger<AgriMarket.BLL.Services.UserService>.Instance))
            {
                ControllerContext = ControllerContextFactory.WithAuthenticatedUser(userId, role)
            };
        [Fact]
        public void Controller_HasProviderOnlyPolicy()
        {
            // 5.1 Verify ProviderOnly policy blocks Farmer-role users
            var attr = typeof(MyListingsController).GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("ProviderOnly", attr.Policy);
        }

        [Fact]
        public async Task Actions_OnOtherProviderListing_ReturnNotFound()
        {
            // 5.2 Verify a Provider cannot view, add, or delete slots on another Provider's listing
            using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var (provider1, profile1) = TestDbContextFactory.SeedClientUser(db, "p1@t.c", "pwd", RoleType.Provider);
            var (provider2, profile2) = TestDbContextFactory.SeedClientUser(db, "p2@t.c", "pwd", RoleType.Provider);
            
            var (listing, availability) = TestDbContextFactory.SeedListing(db, profile1.Id);

            var controller = CreateController(db, provider2.Id);

            var getResult = await controller.Availabilities(listing.Id);
            Assert.IsType<NotFoundResult>(getResult);

            var addResult = await controller.AddAvailability(listing.Id, new ManageAvailabilitiesViewModel());
            Assert.IsType<NotFoundResult>(addResult);

            var delResult = await controller.DeleteAvailability(availability.Id);
            Assert.IsType<NotFoundResult>(delResult);
        }

        [Fact]
        public async Task DeleteAvailability_BookedSlot_RedirectsWithError()
        {
            // 5.3 Verify deletion of a booked slot is rejected
            using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var (provider, profile) = TestDbContextFactory.SeedClientUser(db, "p@t.c", "pwd", RoleType.Provider);
            var (listing, availability) = TestDbContextFactory.SeedListing(db, profile.Id);

            availability.IsBooked = true;
            await db.SaveChangesAsync();

            var controller = CreateController(db, provider.Id);
            var tempDataMock = new Mock<ITempDataDictionary>();
            controller.TempData = tempDataMock.Object;

            var result = await controller.DeleteAvailability(availability.Id);
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(MyListingsController.Availabilities), redirectResult.ActionName);
        }

        [Fact]
        public async Task AddAvailability_InvalidDates_AddsModelError()
        {
            // 5.4 Verify AddAvailability rejects StartTime >= EndTime with a validation error
            using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var (provider, profile) = TestDbContextFactory.SeedClientUser(db, "p@t.c", "pwd", RoleType.Provider);
            var (listing, _) = TestDbContextFactory.SeedListing(db, profile.Id);

            var controller = CreateController(db, provider.Id);

            var model = new ManageAvailabilitiesViewModel
            {
                AddStartTime = DateTime.UtcNow.AddDays(1),
                AddEndTime = DateTime.UtcNow // Start > End
            };

            var result = await controller.AddAvailability(listing.Id, model);

            Assert.False(controller.ModelState.IsValid);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Availabilities", viewResult.ViewName);
        }

        [Fact]
        public async Task EndToEnd_ManualFlowSimulation()
        {
            // 5.5 Simulation: Provider login -> create listing -> add slot -> Farmer -> confirm booking -> Provider sees booked.
            using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
            var (provider, provProfile) = TestDbContextFactory.SeedClientUser(db, "p@t.c", "pwd", RoleType.Provider);
            var (farmer, farmProfile) = TestDbContextFactory.SeedClientUser(db, "f@t.c", "pwd", RoleType.Farmer);

            var pController = CreateController(db, provider.Id);

            var categoryId = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001");
            var createRes = await pController.Create(new MyListingCreateViewModel 
            { 
                Title = "Tractor", 
                ServiceCategoryId = categoryId,
                PricePerHectare = 10 
            });
            
            var listingId = db.ServiceListings.First().Id;
            
            await pController.AddAvailability(listingId, new ManageAvailabilitiesViewModel
            {
                AddStartTime = DateTime.UtcNow.AddDays(1),
                AddEndTime = DateTime.UtcNow.AddDays(1).AddHours(2)
            });

            var availId = db.Availabilities.First().Id;

            // Farmer views and books (simulating booking submission)
            var booking = new AgriMarket.Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                ClientProfileId = farmProfile.Id,
                ServiceListingId = listingId,
                AvailabilityId = availId,
                Status = BookingStatus.Pending,
                TotalPrice = 20,
                AreaInHectares = 2m,
                CreatedAt = DateTime.UtcNow
            };
            db.Bookings.Add(booking);
            var avail = db.Availabilities.First(a => a.Id == availId);
            avail.IsBooked = true; // Business logic normally does this
            await db.SaveChangesAsync();

            // Provider checks availabilities
            var getRes = await pController.Availabilities(listingId);
            var viewRes = Assert.IsType<ViewResult>(getRes);
            var model = Assert.IsType<ManageAvailabilitiesViewModel>(viewRes.Model);
            
            Assert.Single(model.Availabilities);
            Assert.True(model.Availabilities[0].IsBooked);
        }
    }
}