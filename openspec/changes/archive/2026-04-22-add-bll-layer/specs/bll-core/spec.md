## ADDED Requirements

### Requirement: BLL project exists as a class library
The `AgriMarket.BLL` project SHALL exist as a .NET class library in the solution, referencing `AgriMarket.DAL` and `AgriMarket.Domain`. It SHALL NOT reference `AgriMarket.Api` or `AgriMarket.Web`.

#### Scenario: BLL project compiles independently
- **WHEN** the solution is built
- **THEN** `AgriMarket.BLL` compiles without errors and its output assembly is produced

### Requirement: BLL exposes a DI registration extension
The BLL SHALL provide a static extension method `AddBll(this IServiceCollection services)` that registers all BLL service interfaces with their implementations using scoped lifetime.

#### Scenario: Api registers BLL services
- **WHEN** `AgriMarket.Api` calls `builder.Services.AddBll()` in `Program.cs`
- **THEN** all BLL service interfaces resolve correctly from the DI container at runtime

#### Scenario: Web registers BLL services
- **WHEN** `AgriMarket.Web` calls `builder.Services.AddBll()` in `Program.cs`
- **THEN** all BLL service interfaces resolve correctly from the DI container at runtime

### Requirement: Auth and Token services reside in BLL
`IAuthService` and `ITokenService` with their implementations SHALL live in `AgriMarket.BLL`. `AgriMarket.Api` SHALL reference them via the BLL interfaces only and SHALL NOT contain its own copies.

#### Scenario: Api resolves IAuthService from BLL
- **WHEN** `AuthController` requests `IAuthService` from DI
- **THEN** the BLL implementation is injected with no compilation errors

### Requirement: BookingService encapsulates booking business logic
The BLL SHALL provide `IBookingService` with operations for creating, retrieving, updating status, and deleting bookings. All booking queries SHALL use appropriate EF Core `Include` calls to load related entities.

#### Scenario: Create booking rejects provider self-booking
- **WHEN** a provider attempts to book their own listing
- **THEN** the service returns a failure result and no booking record is persisted

#### Scenario: Get bookings for client returns only that client's bookings
- **WHEN** `GetBookingsForClientAsync(clientProfileId)` is called
- **THEN** only bookings belonging to that client are returned

### Requirement: ListingService encapsulates listing business logic
The BLL SHALL provide `IListingService` with operations for creating, retrieving, updating, and deleting service listings, including availability slot management.

#### Scenario: Get all listings returns published listings
- **WHEN** `GetAllListingsAsync()` is called
- **THEN** all listings are returned with their category and provider profile included

### Requirement: ReviewService encapsulates review business logic
The BLL SHALL provide `IReviewService` with operations for submitting and retrieving reviews linked to completed bookings.

#### Scenario: Submit review for completed booking
- **WHEN** a client submits a review for a booking with status Completed
- **THEN** the review is persisted and linked to the booking

#### Scenario: Review rejected for non-completed booking
- **WHEN** a client attempts to submit a review for a booking that is not Completed
- **THEN** the service returns a failure result and no review is persisted

### Requirement: UserService encapsulates user profile business logic
The BLL SHALL provide `IUserService` with operations for retrieving and updating user profiles (both client and provider profiles).

#### Scenario: Get user profile by id
- **WHEN** `GetUserAsync(userId)` is called with a valid user id
- **THEN** the user with their associated profile is returned

### Requirement: Web controllers use BLL interfaces, not AppDbContext
All `AgriMarket.Web` controllers (Admin and Client areas) SHALL inject BLL service interfaces. No controller in `AgriMarket.Web` SHALL directly inject or use `AppDbContext`.

#### Scenario: Admin BookingsController uses IBookingService
- **WHEN** `Admin/BookingsController` handles a request
- **THEN** it delegates data access to `IBookingService` and maps results to ViewModels itself

#### Scenario: No direct DbContext in Web controllers
- **WHEN** the Web project is compiled
- **THEN** no controller file contains a constructor parameter of type `AppDbContext`
