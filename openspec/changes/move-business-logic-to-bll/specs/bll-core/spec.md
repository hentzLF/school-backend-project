## MODIFIED Requirements

### Requirement: ListingService encapsulates listing business logic
The BLL SHALL provide `IListingService` with operations that accept and return DTOs. `CreateAsync` SHALL accept a userId and `CreateListingDto`, construct the `ServiceListing` entity internally, set `IsActive` to false, generate the ID, resolve the provider profile, and return a `ListingDto`. `UpdateAsync` SHALL accept a userId and `UpdateListingDto`, verify ownership, apply changes, and return a `ListingDto`. All query methods SHALL return DTOs, not entities.

#### Scenario: Create listing via DTO
- **WHEN** `CreateAsync(userId, CreateListingDto)` is called with valid data
- **THEN** the service constructs the entity, sets `IsActive = false`, persists it, and returns a `ListingDto`

#### Scenario: Create listing resolves provider profile
- **WHEN** `CreateAsync(userId, CreateListingDto)` is called
- **THEN** the service resolves the provider's `UserProfileId` from the given `userId` and assigns it to the entity

#### Scenario: Update listing verifies ownership
- **WHEN** `UpdateAsync(userId, UpdateListingDto)` is called for a listing not owned by that user
- **THEN** the service throws `BusinessRuleException`

#### Scenario: Get all listings returns DTOs
- **WHEN** `GetAllAsync()` is called
- **THEN** the service returns `IEnumerable<ListingSummaryDto>`, not entities

### Requirement: BookingService encapsulates booking business logic
The BLL SHALL provide `IBookingService` with operations that accept and return DTOs. `CreateAsync` SHALL accept a userId and `CreateBookingDto`, resolve the client profile, fetch the listing and availability, calculate `TotalPrice`, mark the availability as booked, construct the `Booking` entity, and return a `BookingDto`. The self-booking guard SHALL be enforced within the service.

#### Scenario: Create booking via DTO
- **WHEN** `CreateAsync(userId, CreateBookingDto)` is called with valid data
- **THEN** the service calculates `TotalPrice` as `areaInHectares * listing.PricePerHectare`, marks the availability as booked, persists the booking, and returns a `BookingDto`

#### Scenario: Create booking rejects provider self-booking
- **WHEN** a provider attempts to book their own listing via `CreateAsync`
- **THEN** the service throws `BusinessRuleException` and no booking record is persisted

#### Scenario: Create booking rejects unavailable slot
- **WHEN** `CreateAsync` is called with an `availabilityId` that is already booked
- **THEN** the service throws `BusinessRuleException`

#### Scenario: Get bookings for client returns DTOs
- **WHEN** `GetByClientAsync(clientProfileId)` is called
- **THEN** only bookings belonging to that client are returned as `IEnumerable<BookingDto>`

### Requirement: ReviewService encapsulates review business logic
The BLL SHALL provide `IReviewService` with operations that accept and return DTOs. `CreateAsync` SHALL accept a userId and `CreateReviewDto`, validate booking state, construct the entity, and return a `ReviewDto`.

#### Scenario: Submit review for completed booking via DTO
- **WHEN** `CreateAsync(userId, CreateReviewDto)` is called for a booking with status Completed
- **THEN** the review is persisted and a `ReviewDto` is returned

#### Scenario: Review rejected for non-completed booking
- **WHEN** `CreateAsync` is called for a booking that is not Completed
- **THEN** the service throws `BusinessRuleException` and no review is persisted

### Requirement: UserService encapsulates user profile business logic
The BLL SHALL provide `IUserService` with query operations that return DTOs. `GetProfileByUserIdAsync` SHALL return a `UserProfileDto` instead of a `UserProfile` entity.

#### Scenario: Get user profile returns DTO
- **WHEN** `GetProfileByUserIdAsync(userId)` is called with a valid user id
- **THEN** the service returns a `UserProfileDto` with profile fields and email from the linked `AppUser`

#### Scenario: User profile email is privacy-safe
- **WHEN** a profile DTO is returned in a context where the caller is not the owner or an admin
- **THEN** `UserProfileDto.Email` is `null`

### Requirement: Web controllers use BLL interfaces, not AppDbContext
All `AgriMarket.Web` controllers (Admin and Client areas) SHALL inject BLL service interfaces. No controller in `AgriMarket.Web` SHALL directly inject or use `AppDbContext`. Controllers SHALL NOT reference `AgriMarket.Domain.Entities` — they SHALL work exclusively with DTOs from BLL and ViewModels from the Web project.

#### Scenario: No entity references in Web controllers
- **WHEN** the Web project is compiled
- **THEN** no controller file contains a `using AgriMarket.Domain.Entities` directive

#### Scenario: No direct DbContext in Web controllers
- **WHEN** the Web project is compiled
- **THEN** no controller file contains a constructor parameter of type `AppDbContext`

### Requirement: Mapping logic is implemented in per-project manual mapper modules
Mapping between Web ViewModels and BLL DTOs, and between API contracts and BLL DTOs, SHALL be implemented in explicit manual mapper classes or extension methods within each presentation project. Controllers SHALL call these mappers and SHALL NOT contain large repeated inline object-construction mapping blocks.

#### Scenario: Web mapping is delegated to mapper modules
- **WHEN** a Web controller maps ViewModel data to DTOs or DTOs to ViewModels
- **THEN** it uses mapper classes/extensions from the Web project

#### Scenario: API mapping is delegated to mapper modules
- **WHEN** an API controller maps request or response shapes to BLL DTOs
- **THEN** it uses mapper classes/extensions from the API project
