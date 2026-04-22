# Spec: Client Booking Management (MVC)

## Purpose
Defines client-facing booking creation, listing, detail, and status management pages in the Client MVC area.

## Requirements

### Requirement: Client can create a booking from listing details
The system SHALL allow an authenticated client to create a booking from a listing details page. The controller SHALL map the `CreateBookingViewModel` to a `CreateBookingDto` via Web mapper modules and call `IBookingService.CreateAsync(userId, dto)`. The controller SHALL NOT construct a `Booking` entity, calculate `TotalPrice`, or update availability status — all of this SHALL be delegated to the BLL service.

#### Scenario: Create booking successfully
- **WHEN** an authenticated client submits valid booking data
- **THEN** the controller maps the ViewModel to `CreateBookingDto`, the BLL service handles entity construction, price calculation, and availability update, and the system redirects to the booking details page

#### Scenario: Self-booking rejected
- **WHEN** an authenticated client submits booking data for a listing they own as a provider
- **THEN** the BLL service throws `BusinessRuleException` and the controller redirects back to the listing details

#### Scenario: Unavailable slot rejected
- **WHEN** an authenticated client submits booking data for an already-booked availability slot
- **THEN** the BLL service throws `BusinessRuleException` and the controller redirects back with an error message

#### Scenario: Invalid booking input rejected
- **WHEN** an authenticated client submits incomplete or invalid booking data
- **THEN** the system redisplays the booking form with validation errors and does not create a booking

### Requirement: Client can view own bookings
The system SHALL provide a booking management page at `/Client/Bookings` that lists only bookings belonging to the authenticated client's profile and shows booking status and key booking metadata.

#### Scenario: View own bookings
- **WHEN** an authenticated client navigates to `/Client/Bookings`
- **THEN** the system displays only that client's bookings

#### Scenario: No bookings for client
- **WHEN** an authenticated client navigates to `/Client/Bookings` and has no bookings
- **THEN** the system displays an empty-state message

### Requirement: Client can view own booking details
The system SHALL provide booking details at `/Client/Bookings/Details/{id}` for bookings owned by the authenticated client profile.

#### Scenario: View own booking details
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for a booking they own
- **THEN** the system shows booking details and available client actions

#### Scenario: Access denied for non-owned booking
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for a booking they do not own
- **THEN** the system denies access and does not reveal booking details

### Requirement: Client can confirm booking completion
The system SHALL allow a client to confirm completion for an owned booking that is in `ProviderCompleted` status, transitioning it to `ClientConfirmed`.

#### Scenario: Confirm completion from ProviderCompleted
- **WHEN** an authenticated client confirms completion for an owned booking in `ProviderCompleted` status
- **THEN** the system updates booking status to `ClientConfirmed`

#### Scenario: Completion confirmation blocked for invalid status
- **WHEN** an authenticated client attempts to confirm completion for an owned booking not in `ProviderCompleted` status
- **THEN** the system rejects the action and preserves the current booking status
