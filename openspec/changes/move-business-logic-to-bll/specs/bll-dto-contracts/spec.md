## ADDED Requirements

### Requirement: BLL defines shared input/output DTOs for all service methods
The BLL SHALL define input and output DTO classes under `AgriMarket.BLL/Dtos/` organized by domain area (Listings, Bookings, Reviews, Users, Categories, Dashboard). These DTOs SHALL be the sole data types crossing service boundaries — no domain entity SHALL appear in a BLL service interface signature except within the service implementation.

#### Scenario: Listing DTOs exist
- **WHEN** the BLL project is compiled
- **THEN** `AgriMarket.BLL.Dtos.Listings` namespace contains `CreateListingDto`, `UpdateListingDto`, `ListingDto`, and `ListingSummaryDto`

#### Scenario: Booking DTOs exist
- **WHEN** the BLL project is compiled
- **THEN** `AgriMarket.BLL.Dtos.Bookings` namespace contains `CreateBookingDto`, `BookingDto`, and `BookingSummaryDto`

#### Scenario: Review DTOs exist
- **WHEN** the BLL project is compiled
- **THEN** `AgriMarket.BLL.Dtos.Reviews` namespace contains `CreateReviewDto` and `ReviewDto`

#### Scenario: User DTOs exist
- **WHEN** the BLL project is compiled
- **THEN** `AgriMarket.BLL.Dtos.Users` namespace contains `UserProfileDto`

#### Scenario: Availability DTOs exist
- **WHEN** the BLL project is compiled
- **THEN** `AgriMarket.BLL.Dtos.Listings` namespace contains `CreateAvailabilityDto` and `AvailabilityDto`

### Requirement: BLL defines a BusinessRuleException for expected failures
The BLL SHALL define a `BusinessRuleException` class in `AgriMarket.BLL` that services throw when a business rule is violated (e.g., self-booking, reviewing a non-completed booking, deleting a listing with active bookings). The exception SHALL carry a human-readable message describing the violation.

#### Scenario: BusinessRuleException is throwable
- **WHEN** a service detects a business rule violation
- **THEN** it throws `BusinessRuleException` with a descriptive message and the caller can catch it by type

### Requirement: API project DTOs are consolidated into BLL
All DTOs previously defined in `AgriMarket.Api/Dtos/` SHALL be removed. The `AgriMarket.Api` project SHALL NOT contain a `Dtos/` folder. API controllers SHALL reference DTOs from `AgriMarket.BLL.Dtos` namespaces.

#### Scenario: No DTOs in API project
- **WHEN** the solution is built
- **THEN** no `.cs` files exist under `AgriMarket.Api/Dtos/`

### Requirement: Output DTOs match existing API response shapes
BLL output DTOs (`ListingDto`, `BookingDto`, `ReviewDto`, `UserProfileDto`) SHALL include the same fields as the existing API response DTOs so that API controllers can return them directly without additional mapping.

#### Scenario: ListingDto matches ServiceListingResponse shape
- **WHEN** `ListingDto` is serialized to JSON
- **THEN** the JSON includes `id`, `title`, `description`, `pricePerHectare`, `isActive`, `userProfileId`, `serviceCategoryId`

#### Scenario: BookingDto matches BookingResponse shape
- **WHEN** `BookingDto` is serialized to JSON
- **THEN** the JSON includes `id`, `status`, `totalPrice`, `areaInHectares`, `createdAt`, `notes`, `serviceListingId`, `clientProfileId`, `availabilityId`

#### Scenario: ReviewDto matches ReviewResponse shape
- **WHEN** `ReviewDto` is serialized to JSON
- **THEN** the JSON includes `id`, `rating`, `comment`, `createdAt`, `bookingId`, `reviewerProfileId`

#### Scenario: UserProfileDto matches UserProfileResponse shape
- **WHEN** `UserProfileDto` is serialized to JSON
- **THEN** the JSON includes `id`, `firstName`, `lastName`, `bio`, `avatarUrl`, `appUserId`, `email`
