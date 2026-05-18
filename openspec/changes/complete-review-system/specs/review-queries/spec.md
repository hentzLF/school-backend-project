## ADDED Requirements

### Requirement: Get review by booking
The API SHALL expose `GET /api/reviews/booking/{bookingId}` returning the single `ReviewDto` associated with the given booking, or HTTP 404 if no review exists. This endpoint SHALL be publicly accessible (no authentication required).

#### Scenario: Booking has a review
- **WHEN** `GET /api/reviews/booking/{bookingId}` is called with a bookingId that has an associated review
- **THEN** the response returns HTTP 200 with the `ReviewDto`

#### Scenario: Booking has no review
- **WHEN** `GET /api/reviews/booking/{bookingId}` is called with a bookingId that has no review
- **THEN** the response returns HTTP 404 with a ProblemDetails body

#### Scenario: Non-existent booking
- **WHEN** `GET /api/reviews/booking/{bookingId}` is called with a bookingId that does not exist in the database
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: Get reviews by profile
The API SHALL expose `GET /api/reviews/profile/{profileId}` returning a paginated list of `ReviewDto` records where `ReviewedProfileId` matches the given profile. This endpoint SHALL be publicly accessible (no authentication required).

#### Scenario: Profile has reviews
- **WHEN** `GET /api/reviews/profile/{profileId}?page=1&pageSize=10` is called for a profile with reviews
- **THEN** the response returns HTTP 200 with `{ items, page, pageSize, totalCount }` containing only reviews where `ReviewedProfileId` matches

#### Scenario: Profile has no reviews
- **WHEN** `GET /api/reviews/profile/{profileId}` is called for a profile with no reviews
- **THEN** the response returns HTTP 200 with `{ items: [], page: 1, pageSize: 20, totalCount: 0 }`

#### Scenario: Pagination
- **WHEN** `GET /api/reviews/profile/{profileId}?page=2&pageSize=5` is called
- **THEN** the response returns the second page of reviews, ordered by `CreatedAt` descending

#### Scenario: Page size cap
- **WHEN** `GET /api/reviews/profile/{profileId}?pageSize=200` is called
- **THEN** the page size SHALL be capped at 100
