using AgriMarket.BLL.Services;
using AgriMarket.DAL;
using AgriMarket.DAL.Repositories;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels.Equipment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class EquipmentControllerTests
{
    private static UserService CreateUserService(AppDbContext db) =>
        new(new EfAppUserRepository(db),
            new EfUserProfileRepository(db),
            new EfRepository<UserRole>(db),
            new EfUnitOfWork(db),
            new EfRepository<MessageRead>(db),
            new EfRepository<Message>(db),
            new EfRepository<ConversationParticipant>(db),
            new EfRepository<Review>(db),
            new EfRepository<Booking>(db),
            new EfRepository<ServiceListing>(db),
            TestServiceFactory.CreateReviewService(db),
            NullLogger<UserService>.Instance);

    private static ListingService CreateListingService(AppDbContext db) =>
        new(new EfListingRepository(db),
            new EfRepository<UserProfile>(db),
            new EfRepository<Booking>(db),
            new EfRepository<Municipality>(db),
            new EfRepository<Location>(db),
            new EfAvailabilityRepository(db),
            new EfUnitOfWork(db),
            TestServiceFactory.CreateReviewService(db),
            NullLogger<ListingService>.Instance);

    private static EquipmentController CreateController(AppDbContext db, Guid userId)
    {
        var equipmentService = TestServiceFactory.CreateEquipmentService(db);
        var listingService = CreateListingService(db);
        var userService = CreateUserService(db);

        return new EquipmentController(equipmentService, listingService, userService)
        {
            ControllerContext = ControllerContextFactory.WithAuthenticatedUser(userId, "Provider")
        };
    }

    [Fact]
    public async Task Index_ReturnsViewWithEquipmentList()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EquipmentIndexViewModel>(viewResult.Model);
        Assert.Single(model.Equipments);
    }

    [Fact]
    public async Task Index_ReturnsEmptyListForNewProvider()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EquipmentIndexViewModel>(viewResult.Model);
        Assert.Empty(model.Equipments);
    }

    [Fact]
    public async Task Create_Get_ReturnsView()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var controller = CreateController(db, user.Id);

        // Act
        var result = controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<EquipmentCreateViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var controller = CreateController(db, user.Id);
        var model = new EquipmentCreateViewModel
        {
            Name = "Test Tractor",
            Make = "John Deere",
            Condition = EquipmentCondition.Good
        };

        // Act
        var result = await controller.Create(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EquipmentController.Index), redirect.ActionName);
        Assert.Single(db.Equipments);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var controller = CreateController(db, user.Id);
        controller.ModelState.AddModelError("Name", "Required");
        var model = new EquipmentCreateViewModel();

        // Act
        var result = await controller.Create(model);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ReturnsViewWithPopulatedModel()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.Edit(equipment.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EquipmentEditViewModel>(viewResult.Model);
        Assert.Equal(equipment.Id, model.Id);
        Assert.Equal("Test Tractor", model.Name);
    }

    [Fact]
    public async Task Edit_Get_NotOwnedEquipment_ReturnsNotFound()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var (_, otherProfile) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, otherProfile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.Edit(equipment.Id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);
        var model = new EquipmentEditViewModel
        {
            Id = equipment.Id,
            Name = "Updated Tractor",
            Make = "Valtra",
            Condition = EquipmentCondition.Excellent
        };

        // Act
        var result = await controller.Edit(equipment.Id, model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EquipmentController.Index), redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);
        controller.ModelState.AddModelError("Name", "Required");
        var model = new EquipmentEditViewModel { Id = equipment.Id };

        // Act
        var result = await controller.Edit(equipment.Id, model);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ReturnsConfirmationView()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.Delete(equipment.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EquipmentDeleteViewModel>(viewResult.Model);
        Assert.Equal(equipment.Id, model.Id);
    }

    [Fact]
    public async Task Delete_Get_NotOwned_ReturnsNotFound()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var (_, otherProfile) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, otherProfile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.Delete(equipment.Id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesEquipmentAndRedirects()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.DeleteConfirmed(equipment.Id);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EquipmentController.Index), redirect.ActionName);
        Assert.Empty(db.Equipments);
    }

    [Fact]
    public async Task UpdateStatus_ValidStatus_RedirectsToIndex()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.UpdateStatus(equipment.Id, EquipmentStatus.UnderMaintenance);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EquipmentController.Index), redirect.ActionName);

        var updated = await db.Equipments.FindAsync(equipment.Id);
        Assert.Equal(EquipmentStatus.UnderMaintenance, updated!.Status);
    }

    [Fact]
    public async Task UpdateStatus_NotOwned_ReturnsNotFound()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var (_, otherProfile) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pwd", RoleType.Provider);
        var equipment = TestDbContextFactory.SeedEquipment(db, otherProfile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.UpdateStatus(equipment.Id, EquipmentStatus.Retired);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AssignToListing_Get_ReturnsViewWithEquipmentList()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var (listing, _) = TestDbContextFactory.SeedListing(db, profile.Id);
        TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.AssignToListing(listing.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EquipmentAssignViewModel>(viewResult.Model);
        Assert.Equal(listing.Id, model.ListingId);
        Assert.Single(model.Equipment);
    }

    [Fact]
    public async Task AssignToListing_Get_OtherProviderListing_ReturnsNotFound()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, _) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var (_, otherProfile) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pwd", RoleType.Provider);
        var (listing, _) = TestDbContextFactory.SeedListing(db, otherProfile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.AssignToListing(listing.Id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AssignToListing_Post_AssignsEquipmentAndRedirects()
    {
        // Arrange
        using var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var (user, profile) = TestDbContextFactory.SeedClientUser(db, "p@test.com", "pwd", RoleType.Provider);
        var (listing, _) = TestDbContextFactory.SeedListing(db, profile.Id);
        var equipment = TestDbContextFactory.SeedEquipment(db, profile.Id);
        var controller = CreateController(db, user.Id);

        // Act
        var result = await controller.AssignToListing(listing.Id, new List<Guid> { equipment.Id });

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("MyListings", redirect.ControllerName);
        Assert.Single(db.ServiceListingEquipments);
    }
}
