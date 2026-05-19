using AgriMarket.BLL;
using AgriMarket.BLL.Dtos.Equipment;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace AgriMarket.Tests.Services;

public class EquipmentServiceTests
{
    private static readonly Guid ProviderId = Guid.NewGuid();
    private static readonly Guid OtherProviderId = Guid.NewGuid();

    private static (DAL.AppDbContext db, EquipmentService sut) CreateSut(string? dbName = null)
    {
        var db = TestDbContextFactory.Create(dbName ?? Guid.NewGuid().ToString());
        TestDbContextFactory.SeedClientUser(db, "provider@test.com", "pass", RoleType.Provider);
        var sut = TestServiceFactory.CreateEquipmentService(db);
        return (db, sut);
    }

    private static CreateEquipmentDto ValidCreateDto() => new()
    {
        Name = "Test Tractor",
        Make = "John Deere",
        Model = "6130M",
        ManufactureYear = 2022,
        HorsePower = 130,
        Condition = EquipmentCondition.Good,
        Description = "A good tractor"
    };

    [Fact]
    public async Task CreateAsync_SetsDefaultStatusToAvailable()
    {
        var (_, sut) = CreateSut();

        var result = await sut.CreateAsync(ProviderId, ValidCreateDto());

        result.Status.Should().Be(EquipmentStatus.Available);
        result.Name.Should().Be("Test Tractor");
        result.Make.Should().Be("John Deere");
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_AssignsUserProfileId()
    {
        var (db, sut) = CreateSut();

        var result = await sut.CreateAsync(ProviderId, ValidCreateDto());

        var entity = await db.Equipments.FindAsync(result.Id);
        entity.Should().NotBeNull();
        entity!.UserProfileId.Should().Be(ProviderId);
    }

    [Fact]
    public async Task GetByProviderAsync_ReturnsOnlyOwnedEquipment()
    {
        var (_, sut) = CreateSut();
        await sut.CreateAsync(ProviderId, ValidCreateDto());
        await sut.CreateAsync(OtherProviderId, new CreateEquipmentDto
        {
            Name = "Other Tractor",
            Make = "Valtra",
            Condition = EquipmentCondition.Good
        });

        var result = await sut.GetByProviderAsync(ProviderId);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Test Tractor");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotOwned()
    {
        var (_, sut) = CreateSut();
        var created = await sut.CreateAsync(OtherProviderId, ValidCreateDto());

        var result = await sut.GetByIdAsync(ProviderId, created.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenOwnedByProvider()
    {
        var (_, sut) = CreateSut();
        var created = await sut.CreateAsync(ProviderId, ValidCreateDto());

        var result = await sut.GetByIdAsync(ProviderId, created.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsKeyNotFoundException_WhenNotFound()
    {
        var (_, sut) = CreateSut();
        var dto = new UpdateEquipmentDto
        {
            Name = "Updated",
            Make = "Valtra",
            Condition = EquipmentCondition.Excellent
        };

        var act = () => sut.UpdateAsync(ProviderId, Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ThrowsBusinessRuleException_WhenNotOwned()
    {
        var (_, sut) = CreateSut();
        var created = await sut.CreateAsync(OtherProviderId, ValidCreateDto());
        var dto = new UpdateEquipmentDto
        {
            Name = "Updated",
            Make = "Valtra",
            Condition = EquipmentCondition.Excellent
        };

        var act = () => sut.UpdateAsync(ProviderId, created.Id, dto);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFieldsAndReturnsDto()
    {
        var (_, sut) = CreateSut();
        var created = await sut.CreateAsync(ProviderId, ValidCreateDto());
        var dto = new UpdateEquipmentDto
        {
            Name = "Updated Tractor",
            Make = "Valtra",
            Model = "T254",
            ManufactureYear = 2024,
            HorsePower = 254,
            Condition = EquipmentCondition.Excellent,
            Description = "Upgraded"
        };

        var result = await sut.UpdateAsync(ProviderId, created.Id, dto);

        result.Name.Should().Be("Updated Tractor");
        result.Make.Should().Be("Valtra");
        result.Model.Should().Be("T254");
        result.HorsePower.Should().Be(254);
        result.Status.Should().Be(EquipmentStatus.Available);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsBusinessRuleException_WhenNotOwned()
    {
        var (_, sut) = CreateSut();
        var created = await sut.CreateAsync(OtherProviderId, ValidCreateDto());

        var act = () => sut.DeleteAsync(ProviderId, created.Id);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task DeleteAsync_RemovesEquipment()
    {
        var (db, sut) = CreateSut();
        var created = await sut.CreateAsync(ProviderId, ValidCreateDto());

        await sut.DeleteAsync(ProviderId, created.Id);

        var entity = await db.Equipments.FindAsync(created.Id);
        entity.Should().BeNull();
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdatesOnlyStatus()
    {
        var (_, sut) = CreateSut();
        var created = await sut.CreateAsync(ProviderId, ValidCreateDto());

        var result = await sut.UpdateStatusAsync(ProviderId, created.Id, EquipmentStatus.UnderMaintenance);

        result.Status.Should().Be(EquipmentStatus.UnderMaintenance);
        result.Name.Should().Be("Test Tractor");
    }

    [Fact]
    public async Task AssignToListingAsync_ThrowsWhenListingNotOwned()
    {
        var (db, sut) = CreateSut();
        var (_, otherProfile) = TestDbContextFactory.SeedClientUser(db, "other@test.com", "pass", RoleType.Provider);
        var (listing, _) = TestDbContextFactory.SeedListing(db, otherProfile.Id);
        var equipment = await sut.CreateAsync(ProviderId, ValidCreateDto());

        var act = () => sut.AssignToListingAsync(ProviderId, listing.Id, [equipment.Id]);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task AssignToListingAsync_ThrowsWhenEquipmentNotOwned()
    {
        var (db, sut) = CreateSut();
        var (listing, _) = TestDbContextFactory.SeedListing(db, ProviderId);
        var otherEquipment = await sut.CreateAsync(OtherProviderId, ValidCreateDto());

        var act = () => sut.AssignToListingAsync(ProviderId, listing.Id, [otherEquipment.Id]);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task AssignToListingAsync_ReplacesExistingAssignments()
    {
        var (db, sut) = CreateSut();
        var (listing, _) = TestDbContextFactory.SeedListing(db, ProviderId);
        var eq1 = await sut.CreateAsync(ProviderId, ValidCreateDto());
        var eq2 = await sut.CreateAsync(ProviderId, new CreateEquipmentDto
        {
            Name = "Sprayer",
            Make = "AMAZONE",
            Condition = EquipmentCondition.Good
        });

        await sut.AssignToListingAsync(ProviderId, listing.Id, [eq1.Id, eq2.Id]);
        var firstAssignment = await sut.GetByListingAsync(listing.Id);
        firstAssignment.Should().HaveCount(2);

        await sut.AssignToListingAsync(ProviderId, listing.Id, [eq1.Id]);
        var secondAssignment = await sut.GetByListingAsync(listing.Id);
        secondAssignment.Should().HaveCount(1);
        secondAssignment[0].Id.Should().Be(eq1.Id);
    }

    [Fact]
    public async Task DeleteAsync_RemovesJoinRowsAndEquipment()
    {
        var (db, sut) = CreateSut();
        var (listing, _) = TestDbContextFactory.SeedListing(db, ProviderId);
        var equipment = await sut.CreateAsync(ProviderId, ValidCreateDto());
        await sut.AssignToListingAsync(ProviderId, listing.Id, [equipment.Id]);

        await sut.DeleteAsync(ProviderId, equipment.Id);

        db.ServiceListingEquipments.Should().BeEmpty();
        db.Equipments.Should().BeEmpty();
    }
}
