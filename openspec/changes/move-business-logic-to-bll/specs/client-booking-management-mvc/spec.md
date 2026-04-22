## MODIFIED Requirements

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
