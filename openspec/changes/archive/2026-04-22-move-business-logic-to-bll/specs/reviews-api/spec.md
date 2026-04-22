## MODIFIED Requirements

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
