## MODIFIED Requirements

### Requirement: Create review
The API SHALL expose `POST /api/reviews` accepting a `CreateReviewRequest` and returning the created `ReviewResponse`. Before persisting the review, the system MUST validate that the referenced booking exists and has status `ClientConfirmed` or `ProviderCompleted`. Requests that reference a booking in any other state MUST be rejected with HTTP 422.

#### Scenario: Valid request
- **WHEN** `POST /api/reviews` is called with valid `bookingId`, `reviewerProfileId`, `rating`, and optional `comment`, and the booking status is `ClientConfirmed` or `ProviderCompleted`
- **THEN** the response returns HTTP 201 with the created resource and `createdAt` set to current UTC time

#### Scenario: Rating out of range
- **WHEN** `POST /api/reviews` is called with `rating` outside 1–5
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Missing required fields
- **WHEN** `POST /api/reviews` is called without `bookingId` or `reviewerProfileId`
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Booking not in eligible state
- **WHEN** `POST /api/reviews` is called with a `bookingId` whose booking status is not `ClientConfirmed` or `ProviderCompleted`
- **THEN** the response returns HTTP 422 with a ProblemDetails body and no review is persisted
