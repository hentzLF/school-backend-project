using AgriMarket.BLL;
using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Listings;
using AgriMarket.BLL.Dtos.Locations;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace AgriMarket.Tests.Services;

public class ListingServiceLocationTests
{
    private readonly Mock<IListingRepository> _listings = new();
    private readonly Mock<IRepository<UserProfile>> _userProfiles = new();
    private readonly Mock<IRepository<Booking>> _bookings = new();
    private readonly Mock<IRepository<Municipality>> _municipalities = new();
    private readonly Mock<IRepository<Location>> _locations = new();
    private readonly Mock<IAvailabilityRepository> _availabilities = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IReviewService> _reviewService = new();
    private readonly ListingService _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ProfileId = Guid.NewGuid();
    private static readonly Guid MunicipalityId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();

    public ListingServiceLocationTests()
    {
        _sut = new ListingService(
            _listings.Object,
            _userProfiles.Object,
            _bookings.Object,
            _municipalities.Object,
            _locations.Object,
            _availabilities.Object,
            _uow.Object,
            _reviewService.Object,
            NullLogger<ListingService>.Instance);
    }

    private void SetupProfileExists()
    {
        _userProfiles
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserProfile, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { Id = ProfileId, AppUserId = UserId });
    }

    private void SetupMunicipalityExists()
    {
        _municipalities
            .Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Municipality, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private void SetupMunicipalityDoesNotExist()
    {
        _municipalities
            .Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Municipality, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    private void SetupGetByIdReturnsListing(ServiceListing listing)
    {
        _listings
            .Setup(r => r.GetWithFullDetailsAsync(listing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(listing);
    }

    [Fact]
    public async Task CreateAsync_WithLocation_CreatesLocationInline()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityExists();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            Location = new CreateLocationDto
            {
                MunicipalityId = MunicipalityId,
                Address = "Tammsaare tee 56",
                Latitude = 59.437,
                Longitude = 24.7536
            }
        };

        _listings
            .Setup(r => r.GetWithFullDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new ServiceListing
            {
                Id = id,
                Title = dto.Title,
                PricePerHectare = dto.PricePerHectare,
                ServiceCategoryId = CategoryId,
                UserProfileId = ProfileId,
                Location = new Location
                {
                    Id = Guid.NewGuid(),
                    MunicipalityId = MunicipalityId,
                    Address = "Tammsaare tee 56",
                    Latitude = 59.437,
                    Longitude = 24.7536,
                    Municipality = new Municipality
                    {
                        Id = MunicipalityId,
                        Name = "Tallinn",
                        EhakCode = "0784",
                        CountyId = Guid.NewGuid(),
                        County = new County { Name = "Harju maakond" }
                    }
                }
            });

        // Act
        var result = await _sut.CreateAsync(UserId, dto);

        // Assert
        _locations.Verify(r => r.Add(It.Is<Location>(l =>
            l.MunicipalityId == MunicipalityId &&
            l.Address == "Tammsaare tee 56" &&
            l.Latitude == 59.437 &&
            l.Longitude == 24.7536
        )), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithoutLocation_DoesNotCreateLocation()
    {
        // Arrange
        SetupProfileExists();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId
        };

        _listings
            .Setup(r => r.GetWithFullDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new ServiceListing
            {
                Id = id,
                Title = dto.Title,
                PricePerHectare = dto.PricePerHectare,
                ServiceCategoryId = CategoryId,
                UserProfileId = ProfileId
            });

        // Act
        await _sut.CreateAsync(UserId, dto);

        // Assert
        _locations.Verify(r => r.Add(It.IsAny<Location>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_InvalidMunicipalityId_ThrowsBusinessRuleException()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityDoesNotExist();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            Location = new CreateLocationDto { MunicipalityId = Guid.NewGuid() }
        };

        // Act
        var act = () => _sut.CreateAsync(UserId, dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Municipality*does not exist*");
    }

    [Fact]
    public async Task CreateAsync_LatitudeWithoutLongitude_ThrowsBusinessRuleException()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityExists();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            Location = new CreateLocationDto
            {
                MunicipalityId = MunicipalityId,
                Latitude = 59.437
            }
        };

        // Act
        var act = () => _sut.CreateAsync(UserId, dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Latitude and Longitude must be provided together*");
    }

    [Fact]
    public async Task CreateAsync_InvalidLatitude_ThrowsBusinessRuleException()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityExists();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            Location = new CreateLocationDto
            {
                MunicipalityId = MunicipalityId,
                Latitude = 91.0,
                Longitude = 24.0
            }
        };

        // Act
        var act = () => _sut.CreateAsync(UserId, dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Latitude must be between -90 and 90*");
    }

    [Fact]
    public async Task CreateAsync_InvalidLongitude_ThrowsBusinessRuleException()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityExists();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            Location = new CreateLocationDto
            {
                MunicipalityId = MunicipalityId,
                Latitude = 59.0,
                Longitude = -181.0
            }
        };

        // Act
        var act = () => _sut.CreateAsync(UserId, dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Longitude must be between -180 and 180*");
    }

    [Fact]
    public async Task UpdateAsync_WithLocation_UpdatesExistingLocation()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityExists();
        var existingLocationId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var existingListing = new ServiceListing
        {
            Id = listingId,
            Title = "Old Title",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            UserProfileId = ProfileId,
            LocationId = existingLocationId,
            IsActive = true
        };

        var existingLocation = new Location
        {
            Id = existingLocationId,
            MunicipalityId = Guid.NewGuid(),
            Address = "Old address"
        };

        _listings
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ServiceListing, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingListing);

        _locations
            .Setup(r => r.GetByIdAsync(existingLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLocation);

        _listings
            .Setup(r => r.GetWithFullDetailsAsync(listingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingListing);

        var dto = new UpdateListingDto
        {
            Id = listingId,
            Title = "New Title",
            PricePerHectare = 60m,
            ServiceCategoryId = CategoryId,
            IsActive = true,
            Location = new UpdateLocationDto
            {
                MunicipalityId = MunicipalityId,
                Address = "New address"
            }
        };

        // Act
        await _sut.UpdateAsync(UserId, dto);

        // Assert
        _locations.Verify(r => r.Update(It.Is<Location>(l =>
            l.Id == existingLocationId &&
            l.MunicipalityId == MunicipalityId &&
            l.Address == "New address"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NullLocation_RemovesExistingLocation()
    {
        // Arrange
        SetupProfileExists();
        var existingLocationId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var existingListing = new ServiceListing
        {
            Id = listingId,
            Title = "Title",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            UserProfileId = ProfileId,
            LocationId = existingLocationId,
            IsActive = true
        };

        var existingLocation = new Location
        {
            Id = existingLocationId,
            MunicipalityId = Guid.NewGuid()
        };

        _listings
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ServiceListing, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingListing);

        _locations
            .Setup(r => r.GetByIdAsync(existingLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLocation);

        _listings
            .Setup(r => r.GetWithFullDetailsAsync(listingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingListing);

        var dto = new UpdateListingDto
        {
            Id = listingId,
            Title = "Title",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            IsActive = true,
            Location = null
        };

        // Act
        await _sut.UpdateAsync(UserId, dto);

        // Assert
        _locations.Verify(r => r.Remove(It.Is<Location>(l => l.Id == existingLocationId)), Times.Once);
        existingListing.LocationId.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithOptionalCoordinates_CreatesLocationWithoutCoordinates()
    {
        // Arrange
        SetupProfileExists();
        SetupMunicipalityExists();
        var dto = new CreateListingDto
        {
            Title = "Test",
            PricePerHectare = 50m,
            ServiceCategoryId = CategoryId,
            Location = new CreateLocationDto
            {
                MunicipalityId = MunicipalityId
            }
        };

        _listings
            .Setup(r => r.GetWithFullDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new ServiceListing
            {
                Id = id,
                Title = dto.Title,
                PricePerHectare = dto.PricePerHectare,
                ServiceCategoryId = CategoryId,
                UserProfileId = ProfileId
            });

        // Act
        await _sut.CreateAsync(UserId, dto);

        // Assert
        _locations.Verify(r => r.Add(It.Is<Location>(l =>
            l.MunicipalityId == MunicipalityId &&
            l.Latitude == null &&
            l.Longitude == null
        )), Times.Once);
    }
}
