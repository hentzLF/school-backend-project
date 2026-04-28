## Purpose

Defines the REST API endpoints, request/response DTO shapes, and validation rules for managing `Booking` resources in the AgriMarket API.

---

## Requirements

### Requirement: List bookings
The API SHALL expose `GET /api/bookings` returning a paginated list of `Booking` records.

#### Scenario: Default pagination
- **WHEN** `GET /api/bookings` is called with no query params
- **THEN** the response returns HTTP 200 with `{ items, page: 1, pageSize: 20, totalCount }`

### Requirement: Get single booking
The API SHALL expose `GET /api/bookings/{id}` returning a single `BookingResponse`.

#### Scenario: Existing booking
- **WHEN** `GET /api/bookings/{id}` is called with a valid existing ID
- **THEN** the response returns HTTP 200 with a `BookingResponse` body

#### Scenario: Non-existent booking
- **WHEN** `GET /api/bookings/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

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

### Requirement: BookingResponse DTO shape
`BookingResponse` SHALL include: `id`, `status`, `totalPrice`, `areaInHectares`, `createdAt`, `notes`, `serviceListingId`, `clientProfileId`, `availabilityId`.

#### Scenario: Response does not include navigation objects
- **WHEN** a booking endpoint returns a `BookingResponse`
- **THEN** the JSON does not contain nested `serviceListing`, `clientProfile`, or `availability` objects

### Requirement: CreateBookingRequest DTO shape
`CreateBookingRequest` SHALL require: `serviceListingId` (Guid), `clientProfileId` (Guid), `availabilityId` (Guid), `areaInHectares` (positive double). `notes` is optional.

#### Scenario: areaInHectares must be positive
- **WHEN** `POST /api/bookings` is called with `areaInHectares: 0`
- **THEN** the response returns HTTP 400 without writing to the database
