## 1. Prerequisites

- [ ] 1.1 Ensure `fix-api-bll-integration` change is applied first (API controllers already wired to BLL services)

## 2. BLL Foundation

- [ ] 2.1 Create `BusinessRuleException` class in `AgriMarket.BLL/` with a string message constructor
- [ ] 2.2 Create `AgriMarket.BLL/Dtos/Listings/CreateListingDto.cs` with fields: `Title`, `Description`, `ServiceCategoryId`, `PricePerHectare`
- [ ] 2.3 Create `AgriMarket.BLL/Dtos/Listings/UpdateListingDto.cs` with fields: `Id`, `Title`, `Description`, `ServiceCategoryId`, `PricePerHectare`, `IsActive`
- [ ] 2.4 Create `AgriMarket.BLL/Dtos/Listings/ListingDto.cs` matching existing `ServiceListingResponse` shape: `Id`, `Title`, `Description`, `PricePerHectare`, `IsActive`, `UserProfileId`, `ServiceCategoryId`
- [ ] 2.5 Create `AgriMarket.BLL/Dtos/Listings/ListingSummaryDto.cs` with fields: `Id`, `Title`, `CategoryName`, `ProviderName`, `PricePerHectare`
- [ ] 2.6 Create `AgriMarket.BLL/Dtos/Listings/CreateAvailabilityDto.cs` with fields: `ListingId`, `StartTime`, `EndTime`
- [ ] 2.7 Create `AgriMarket.BLL/Dtos/Listings/AvailabilityDto.cs` with fields: `Id`, `StartTime`, `EndTime`, `IsBooked`, `ServiceListingId`
- [ ] 2.8 Create `AgriMarket.BLL/Dtos/Bookings/CreateBookingDto.cs` with fields: `ServiceListingId`, `AvailabilityId`, `AreaInHectares`, `Notes`
- [ ] 2.9 Create `AgriMarket.BLL/Dtos/Bookings/BookingDto.cs` matching existing `BookingResponse` shape: `Id`, `Status`, `TotalPrice`, `AreaInHectares`, `CreatedAt`, `Notes`, `ServiceListingId`, `ClientProfileId`, `AvailabilityId`
- [ ] 2.10 Create `AgriMarket.BLL/Dtos/Bookings/BookingSummaryDto.cs` with fields: `Id`, `ClientName`, `Status`, `AreaInHectares`, `TotalPrice`, `CreatedAt`
- [ ] 2.11 Create `AgriMarket.BLL/Dtos/Reviews/CreateReviewDto.cs` with fields: `BookingId`, `Rating`, `Comment`
- [ ] 2.12 Create `AgriMarket.BLL/Dtos/Reviews/ReviewDto.cs` matching existing `ReviewResponse` shape: `Id`, `Rating`, `Comment`, `CreatedAt`, `BookingId`, `ReviewerProfileId`
- [ ] 2.13 Create `AgriMarket.BLL/Dtos/Users/UserProfileDto.cs` with fields: `Id`, `FirstName`, `LastName`, `Bio`, `AvatarUrl`, `AppUserId`, `Email`

## 3. Refactor ListingService

- [ ] 3.1 Update `IListingService` — change `CreateAsync` to accept `(Guid userId, CreateListingDto dto)` and return `ListingDto`
- [ ] 3.2 Update `IListingService` — change `UpdateAsync` to accept `(Guid userId, UpdateListingDto dto)` and return `ListingDto`
- [ ] 3.3 Update `IListingService` — change `GetAllAsync` to return `IEnumerable<ListingSummaryDto>`
- [ ] 3.4 Update `IListingService` — change `GetActiveListingsAsync` to return `IEnumerable<ListingSummaryDto>`
- [ ] 3.5 Update `IListingService` — change `GetByIdAsync` to return `ListingDto?` (with availability data)
- [ ] 3.6 Update `IListingService` — change `GetByProviderAsync` to return `IEnumerable<ListingSummaryDto>`
- [ ] 3.7 Update `IListingService` — change `DeleteAsync` to accept `(Guid userId, Guid listingId)` with ownership check and active-bookings guard
- [ ] 3.8 Update `IListingService` — change `AddAvailabilityAsync` to accept `(Guid userId, CreateAvailabilityDto dto)` and return `AvailabilityDto`
- [ ] 3.9 Update `IListingService` — change `DeleteAvailabilityAsync` to accept `(Guid userId, Guid availabilityId)` with ownership and booked-status checks
- [ ] 3.10 Update `IListingService` — change `GetAvailabilityByIdAsync` to return `AvailabilityDto?`
- [ ] 3.11 Implement all `ListingService` changes: entity construction, profile resolution, ownership checks, DTO mapping
- [ ] 3.12 Update Web `MyListingsController` — map ViewModels to/from DTOs, remove entity references
- [ ] 3.13 Update Web `Client/ListingsController` — map DTOs to ViewModels, remove entity references
- [ ] 3.14 Update Web Admin `ListingsController` — map DTOs to ViewModels, remove entity references
- [ ] 3.15 Update API `ListingsController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/ServiceListings/` references
- [ ] 3.16 Build and verify zero compilation errors after ListingService refactor

## 4. Refactor BookingService

- [ ] 4.1 Update `IBookingService` — change `CreateAsync` to accept `(Guid userId, CreateBookingDto dto)` and return `BookingDto`; move price calculation, availability update, and self-booking guard into the service
- [ ] 4.2 Update `IBookingService` — change `GetAllAsync` to return `IEnumerable<BookingDto>`
- [ ] 4.3 Update `IBookingService` — change `GetByIdAsync` to return `BookingDto?`
- [ ] 4.4 Update `IBookingService` — change `GetByClientAsync` to return `IEnumerable<BookingDto>`
- [ ] 4.5 Update `IBookingService` — change `GetByProviderAsync` to return `IEnumerable<BookingDto>`
- [ ] 4.6 Update `IBookingService` — change `GetByListingAsync` to return `IEnumerable<BookingSummaryDto>`
- [ ] 4.7 Update `IBookingService` — change `UpdateStatusAsync` to return `BookingDto`
- [ ] 4.8 Implement all `BookingService` changes: entity construction, price calculation, availability marking, self-booking guard, DTO mapping
- [ ] 4.9 Update Web `Client/ListingsController.Book` — map ViewModel to `CreateBookingDto`, remove entity construction and price calculation
- [ ] 4.10 Update Web `Client/BookingsController` — map DTOs to ViewModels, remove entity references
- [ ] 4.11 Update Web Admin `BookingsController` — map DTOs to ViewModels, remove entity references
- [ ] 4.12 Update API `BookingsController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/Bookings/` references
- [ ] 4.13 Build and verify zero compilation errors after BookingService refactor

## 5. Refactor ReviewService

- [ ] 5.1 Update `IReviewService` — change `CreateAsync` to accept `(Guid userId, CreateReviewDto dto)` and return `ReviewDto`
- [ ] 5.2 Update `IReviewService` — change `GetByBookingAsync` to return `IEnumerable<ReviewDto>`
- [ ] 5.3 Implement `ReviewService` changes: entity construction, booking state validation, DTO mapping
- [ ] 5.4 Update API `ReviewsController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/Reviews/` references
- [ ] 5.5 Build and verify zero compilation errors after ReviewService refactor

## 6. Refactor UserService

- [ ] 6.1 Update `IUserService` — change `GetProfileByUserIdAsync` to return `UserProfileDto?`
- [ ] 6.2 Update `IUserService` — change `GetAllUsersAsync` to return `IEnumerable<UserProfileDto>`
- [ ] 6.3 Update `IUserService` — change `GetUserByIdAsync` to return `UserProfileDto?`
- [ ] 6.4 Implement `UserService` changes: DTO mapping with email from linked AppUser
- [ ] 6.5 Update Web controllers that call `GetProfileByUserIdAsync` — use `UserProfileDto` instead of `UserProfile` entity
- [ ] 6.6 Update Web Admin `UsersController` — map DTOs to ViewModels
- [ ] 6.7 Update API `UsersController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/Users/` references
- [ ] 6.8 Build and verify zero compilation errors after UserService refactor

## 7. Cleanup

- [ ] 7.1 Delete `AgriMarket.Api/Dtos/` folder entirely
- [ ] 7.2 Remove all `using AgriMarket.Domain.Entities` from Web and API controller files
- [ ] 7.3 Remove all `using AgriMarket.Domain.Enums` from controllers where no longer needed (enums used by DTOs should be referenced via DTO types or kept if still needed for status checks)

## 8. Verification

- [ ] 8.1 Full solution build — zero compilation errors across all projects
- [ ] 8.2 Run `AgriMarket.Tests` — all existing tests pass (update test code as needed for new service signatures)
- [ ] 8.3 Verify no controller file contains `new ServiceListing`, `new Booking`, or `new Availability`
- [ ] 8.4 Verify no controller file contains `using AgriMarket.Domain.Entities`
- [ ] 8.5 Verify no `.cs` files exist under `AgriMarket.Api/Dtos/`
- [ ] 8.6 Manual smoke test: create a listing via Web, book it, submit a review — full flow works
