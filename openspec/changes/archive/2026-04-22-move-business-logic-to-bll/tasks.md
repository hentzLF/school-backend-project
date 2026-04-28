## 1. Prerequisites

- [x] 1.1 Ensure `fix-api-bll-integration` change is applied first (API controllers already wired to BLL services)

## 2. BLL Foundation

- [x] 2.1 Create `BusinessRuleException` class in `AgriMarket.BLL/` with a string message constructor
- [x] 2.2 Create `AgriMarket.BLL/Dtos/Listings/CreateListingDto.cs` with fields: `Title`, `Description`, `ServiceCategoryId`, `PricePerHectare`, `LocationId`
- [x] 2.3 Create `AgriMarket.BLL/Dtos/Listings/UpdateListingDto.cs` with fields: `Id`, `Title`, `Description`, `ServiceCategoryId`, `PricePerHectare`, `IsActive`, `LocationId`
- [x] 2.4 Create `AgriMarket.BLL/Dtos/Listings/ListingDto.cs` matching existing `ServiceListingResponse` shape: `Id`, `Title`, `Description`, `PricePerHectare`, `IsActive`, `UserProfileId`, `ServiceCategoryId`, `LocationId`
- [x] 2.5 Create `AgriMarket.BLL/Dtos/Listings/ListingSummaryDto.cs` with fields: `Id`, `Title`, `CategoryName`, `ProviderName`, `PricePerHectare`
- [x] 2.6 Create `AgriMarket.BLL/Dtos/Listings/CreateAvailabilityDto.cs` with fields: `ListingId`, `StartTime`, `EndTime`
- [x] 2.7 Create `AgriMarket.BLL/Dtos/Listings/AvailabilityDto.cs` with fields: `Id`, `StartTime`, `EndTime`, `IsBooked`, `ServiceListingId`
- [x] 2.8 Create `AgriMarket.BLL/Dtos/Bookings/CreateBookingDto.cs` with fields: `ServiceListingId`, `AvailabilityId`, `AreaInHectares`, `Notes`
- [x] 2.9 Create `AgriMarket.BLL/Dtos/Bookings/BookingDto.cs` matching existing `BookingResponse` shape: `Id`, `Status`, `TotalPrice`, `AreaInHectares`, `CreatedAt`, `Notes`, `ServiceListingId`, `ClientProfileId`, `AvailabilityId`
- [x] 2.10 Create `AgriMarket.BLL/Dtos/Bookings/BookingSummaryDto.cs` with fields: `Id`, `ClientName`, `Status`, `AreaInHectares`, `TotalPrice`, `CreatedAt`
- [x] 2.11 Create `AgriMarket.BLL/Dtos/Reviews/CreateReviewDto.cs` with fields: `BookingId`, `Rating`, `Comment`
- [x] 2.12 Create `AgriMarket.BLL/Dtos/Reviews/ReviewDto.cs` matching existing `ReviewResponse` shape: `Id`, `Rating`, `Comment`, `CreatedAt`, `BookingId`, `ReviewerProfileId`
- [x] 2.13 Create `AgriMarket.BLL/Dtos/Users/UserProfileDto.cs` with fields: `Id`, `FirstName`, `LastName`, `Bio`, `AvatarUrl`, `AppUserId`, `Email` (nullable; only populated for owner/admin contexts)
- [x] 2.14 Add project-local manual mapper modules in `AgriMarket.Web` for ViewModel↔DTO conversions used by Client/Admin controllers
- [x] 2.15 Add project-local manual mapper modules in `AgriMarket.Api` for request/response↔DTO conversions where needed; keep controllers free of repeated inline mapping blocks

## 3. Refactor ListingService

- [x] 3.1 Update `IListingService` — change `CreateAsync` to accept `(Guid userId, CreateListingDto dto)` and return `ListingDto`
- [x] 3.2 Update `IListingService` — change `UpdateAsync` to accept `(Guid userId, UpdateListingDto dto)` and return `ListingDto`
- [x] 3.3 Update `IListingService` — change `GetAllAsync` to return `IEnumerable<ListingSummaryDto>`
- [x] 3.4 Update `IListingService` — change `GetActiveListingsAsync` to return `IEnumerable<ListingSummaryDto>`
- [x] 3.5 Update `IListingService` — change `GetByIdAsync` to return `ListingDto?` (with availability data)
- [x] 3.6 Update `IListingService` — change `GetByProviderAsync` to return `IEnumerable<ListingSummaryDto>`
- [x] 3.7 Update `IListingService` — change `DeleteAsync` to accept `(Guid userId, Guid listingId)` with ownership check and active-bookings guard
- [x] 3.8 Update `IListingService` — change `AddAvailabilityAsync` to accept `(Guid userId, CreateAvailabilityDto dto)` and return `AvailabilityDto`
- [x] 3.9 Update `IListingService` — change `DeleteAvailabilityAsync` to accept `(Guid userId, Guid availabilityId)` with ownership and booked-status checks
- [x] 3.10 Update `IListingService` — change `GetAvailabilityByIdAsync` to return `AvailabilityDto?`
- [x] 3.11 Implement all `ListingService` changes: entity construction, profile resolution, ownership checks, DTO mapping
- [x] 3.12 Update Web `MyListingsController` — use mapper modules for ViewModels↔DTOs, remove entity references
- [x] 3.13 Update Web `Client/ListingsController` — use mapper modules for DTO→ViewModel mapping, remove entity references
- [x] 3.14 Update Web Admin `ListingsController` — use mapper modules for DTO→ViewModel mapping, remove entity references
- [x] 3.15 Update API `ListingsController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/ServiceListings/` references, and keep mapping logic in mapper modules (not actions)
- [x] 3.16 Build and verify zero compilation errors after ListingService refactor

## 4. Refactor BookingService

- [x] 4.1 Update `IBookingService` — change `CreateAsync` to accept `(Guid userId, CreateBookingDto dto)` and return `BookingDto`; move price calculation, availability update, and self-booking guard into the service
- [x] 4.2 Update `IBookingService` — change `GetAllAsync` to return `IEnumerable<BookingDto>`
- [x] 4.3 Update `IBookingService` — change `GetByIdAsync` to return `BookingDto?`
- [x] 4.4 Update `IBookingService` — change `GetByClientAsync` to return `IEnumerable<BookingDto>`
- [x] 4.5 Update `IBookingService` — change `GetByProviderAsync` to return `IEnumerable<BookingDto>`
- [x] 4.6 Update `IBookingService` — change `GetByListingAsync` to return `IEnumerable<BookingSummaryDto>`
- [x] 4.7 Update `IBookingService` — change `UpdateStatusAsync` to return `BookingDto`
- [x] 4.8 Implement all `BookingService` changes: entity construction, price calculation, availability marking, self-booking guard, DTO mapping
- [x] 4.9 Update Web `Client/ListingsController.Book` — map ViewModel to `CreateBookingDto` via mapper module, remove entity construction and price calculation
- [x] 4.10 Update Web `Client/BookingsController` — map DTOs to ViewModels via mapper module, remove entity references
- [x] 4.11 Update Web Admin `BookingsController` — map DTOs to ViewModels via mapper module, remove entity references
- [x] 4.12 Update API `BookingsController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/Bookings/` references, and keep mapping logic in mapper modules
- [x] 4.13 Build and verify zero compilation errors after BookingService refactor

## 5. Refactor ReviewService

- [x] 5.1 Update `IReviewService` — change `CreateAsync` to accept `(Guid userId, CreateReviewDto dto)` and return `ReviewDto`
- [x] 5.2 Update `IReviewService` — change `GetByBookingAsync` to return `IEnumerable<ReviewDto>`
- [x] 5.3 Implement `ReviewService` changes: entity construction, booking state validation, DTO mapping
- [x] 5.4 Update API `ReviewsController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/Reviews/` references, and keep mapping logic in mapper modules
- [x] 5.5 Build and verify zero compilation errors after ReviewService refactor

## 6. Refactor UserService

- [x] 6.1 Update `IUserService` — change `GetProfileByUserIdAsync` to return `UserProfileDto?`
- [x] 6.2 Update `IUserService` — change `GetAllUsersAsync` to return `IEnumerable<UserProfileDto>`
- [x] 6.3 Update `IUserService` — change `GetUserByIdAsync` to return `UserProfileDto?` with caller-aware email exposure rules (email only for owner/admin contexts)
- [x] 6.4 Implement `UserService` changes: DTO mapping with privacy-safe email population from linked AppUser
- [x] 6.5 Update Web controllers that call `GetProfileByUserIdAsync` — use `UserProfileDto` instead of `UserProfile` entity
- [x] 6.6 Update Web Admin `UsersController` — map DTOs to ViewModels
- [x] 6.7 Update API `UsersController` — use BLL DTOs, remove `AgriMarket.Api/Dtos/Users/` references, and enforce email visibility rules
- [x] 6.8 Build and verify zero compilation errors after UserService refactor

## 7. Cleanup

- [x] 7.1 Delete `AgriMarket.Api/Dtos/` folder entirely
- [x] 7.2 Remove all `using AgriMarket.Domain.Entities` from Web and API controller files
- [x] 7.3 Remove all `using AgriMarket.Domain.Enums` from controllers where no longer needed (enums used by DTOs should be referenced via DTO types or kept if still needed for status checks)

## 8. Verification

- [x] 8.1 Full solution build — zero compilation errors across all projects
- [x] 8.2 Run `AgriMarket.Tests` — all existing tests pass (update test code as needed for new service signatures)
- [x] 8.3 Verify no controller file contains `new ServiceListing`, `new Booking`, or `new Availability`
- [x] 8.4 Verify no controller file contains `using AgriMarket.Domain.Entities`
- [x] 8.5 Verify no `.cs` files exist under `AgriMarket.Api/Dtos/`
- [x] 8.6 Manual smoke test: create a listing via Web, book it, submit a review — full flow works
