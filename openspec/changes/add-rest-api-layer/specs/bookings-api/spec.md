## ADDED Requirements

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
The API SHALL expose `POST /api/bookings` accepting a `CreateBookingRequest` and returning the created `BookingResponse`.

#### Scenario: Valid request
- **WHEN** `POST /api/bookings` is called with valid `serviceListingId`, `clientProfileId`, `availabilityId`, and `areaInHectares`
- **THEN** the response returns HTTP 201 with the created resource, `status` set to `Pending`, and `createdAt` set to current UTC time

#### Scenario: Missing required fields
- **WHEN** `POST /api/bookings` is called without `serviceListingId`
- **THEN** the response returns HTTP 400 with a ProblemDetails body

### Requirement: Update booking status
The API SHALL expose `PATCH /api/bookings/{id}/status` accepting a `UpdateBookingStatusRequest` with a `status` field.

#### Scenario: Valid status update
- **WHEN** `PATCH /api/bookings/{id}/status` is called with a valid `BookingStatus` value
- **THEN** the response returns HTTP 200 with the updated `BookingResponse`

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
