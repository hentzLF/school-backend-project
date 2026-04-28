## Purpose

Defines the REST API endpoints, request/response DTO shapes, and validation rules for managing `Review` resources in the AgriMarket API.

---

## Requirements

### Requirement: List reviews
The API SHALL expose `GET /api/reviews` returning a paginated list of `Review` records.

#### Scenario: Default pagination
- **WHEN** `GET /api/reviews` is called with no query params
- **THEN** the response returns HTTP 200 with `{ items, page: 1, pageSize: 20, totalCount }`

### Requirement: Get single review
The API SHALL expose `GET /api/reviews/{id}` returning a single `ReviewResponse`.

#### Scenario: Existing review
- **WHEN** `GET /api/reviews/{id}` is called with a valid existing ID
- **THEN** the response returns HTTP 200 with a `ReviewResponse` body

#### Scenario: Non-existent review
- **WHEN** `GET /api/reviews/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: Create review
The API SHALL expose `POST /api/reviews` accepting a `CreateReviewDto` (from BLL) and returning the created `ReviewDto`. The controller SHALL NOT construct a `Review` entity — it SHALL delegate to `IReviewService.CreateAsync(userId, dto)`. Booking state validation SHALL be enforced by the BLL service.

#### Scenario: Valid request
- **WHEN** `POST /api/reviews` is called with valid data for a completed booking
- **THEN** the controller passes the DTO and authenticated userId to the BLL service, and returns HTTP 201 with the `ReviewDto`

#### Scenario: Review for non-completed booking
- **WHEN** `POST /api/reviews` is called for a booking that is not in Completed status
- **THEN** the BLL service throws `BusinessRuleException` and the controller returns HTTP 422

#### Scenario: Rating out of range
- **WHEN** `POST /api/reviews` is called with `rating` outside 1-5
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Missing required fields
- **WHEN** `POST /api/reviews` is called without `bookingId` or `reviewerProfileId`
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Booking not in eligible state
- **WHEN** `POST /api/reviews` is called with a `bookingId` whose booking status is not `ClientConfirmed` or `ProviderCompleted`
- **THEN** the response returns HTTP 422 with a ProblemDetails body and no review is persisted

### Requirement: ReviewResponse DTO shape
`ReviewResponse` SHALL include: `id`, `rating`, `comment`, `createdAt`, `bookingId`, `reviewerProfileId`.

#### Scenario: Response does not include navigation objects
- **WHEN** a review endpoint returns a `ReviewResponse`
- **THEN** the JSON does not contain nested `booking` or `reviewerProfile` objects

### Requirement: CreateReviewRequest DTO shape
`CreateReviewRequest` SHALL require: `bookingId` (Guid), `reviewerProfileId` (Guid), `rating` (integer 1–5). `comment` is optional.

#### Scenario: Rating is validated as integer between 1 and 5
- **WHEN** `POST /api/reviews` is called with `rating: 6`
- **THEN** the response returns HTTP 400 without writing to the database
