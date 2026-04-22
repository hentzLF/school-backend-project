## MODIFIED Requirements

### Requirement: Create booking
The API SHALL expose `POST /api/bookings` accepting a `CreateBookingDto` (from BLL) and returning the created `BookingDto`. The controller SHALL NOT construct a `Booking` entity, calculate `TotalPrice`, or mark availability as booked — all of this SHALL be delegated to `IBookingService.CreateAsync(userId, dto)`.

#### Scenario: Valid request
- **WHEN** `POST /api/bookings` is called with valid data
- **THEN** the controller passes the DTO and authenticated userId to the BLL service, and returns HTTP 201 with the `BookingDto`

#### Scenario: Self-booking rejected
- **WHEN** `POST /api/bookings` is called by a provider for their own listing
- **THEN** the BLL service throws `BusinessRuleException` and the controller returns HTTP 400

#### Scenario: Missing required fields
- **WHEN** `POST /api/bookings` is called without `serviceListingId`
- **THEN** the response returns HTTP 400 with a ProblemDetails body

### Requirement: Update booking status
The API SHALL expose `PATCH /api/bookings/{id}/status` accepting an `UpdateBookingStatusRequest` (from BLL). The controller SHALL delegate to `IBookingService.UpdateStatusAsync(id, status)`.

#### Scenario: Valid status update
- **WHEN** `PATCH /api/bookings/{id}/status` is called with a valid `BookingStatus` value
- **THEN** the response returns HTTP 200 with the updated `BookingDto`

#### Scenario: Invalid status value
- **WHEN** `PATCH /api/bookings/{id}/status` is called with an unrecognized status string
- **THEN** the response returns HTTP 400 with a ProblemDetails body
