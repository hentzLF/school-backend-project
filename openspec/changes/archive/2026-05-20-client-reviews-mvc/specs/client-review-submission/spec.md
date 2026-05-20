## ADDED Requirements

### Requirement: Client can leave a review for a completed booking
The system SHALL allow an authenticated client to create a review for a booking in `ClientConfirmed` status, provided no review already exists for that booking. The controller SHALL map the `CreateReviewViewModel` to a `CreateReviewDto` via the Web mapper and call `IReviewService.CreateAsync(userId, dto)`. The controller SHALL NOT construct a `Review` entity or validate booking state — all business rules SHALL be delegated to the BLL service.

#### Scenario: Create review successfully
- **WHEN** an authenticated client submits a valid review (rating 1-5, optional comment) for a booking in `ClientConfirmed` status that has no existing review
- **THEN** the controller maps the ViewModel to `CreateReviewDto`, the BLL service creates the review, and the system redirects to `/Client/Bookings/Details/{bookingId}` showing the newly created review

#### Scenario: Booking not in completed status
- **WHEN** an authenticated client attempts to create a review for a booking not in `ClientConfirmed` status
- **THEN** the BLL service throws `BusinessRuleException` and the controller redirects back with an error message

#### Scenario: Duplicate review rejected
- **WHEN** an authenticated client attempts to create a review for a booking that already has a review
- **THEN** the BLL service throws `BusinessRuleException` and the controller redirects back with an error message

#### Scenario: Invalid review input rejected
- **WHEN** an authenticated client submits a review with invalid data (missing rating or rating outside 1-5)
- **THEN** the system redisplays the review form with validation errors and does not create a review

#### Scenario: Non-owner attempts to review
- **WHEN** an authenticated client attempts to create a review for a booking they do not own
- **THEN** the BLL service throws `BusinessRuleException` and the controller redirects back with an error message

### Requirement: Client can edit their own review
The system SHALL allow an authenticated client to edit a review they previously created. The controller SHALL map the `EditReviewViewModel` to an `UpdateReviewDto` via the Web mapper and call `IReviewService.UpdateAsync(userId, reviewId, dto)`. Only `Rating` and `Comment` fields SHALL be editable.

#### Scenario: Edit review successfully
- **WHEN** an authenticated client submits valid updated review data for a review they created
- **THEN** the controller maps the ViewModel to `UpdateReviewDto`, the BLL service updates the review, and the system redirects to `/Client/Bookings/Details/{bookingId}`

#### Scenario: Non-owner attempts to edit
- **WHEN** an authenticated client attempts to edit a review created by a different user
- **THEN** the BLL service throws `UnauthorizedBusinessException` and the controller redirects back with an error message

#### Scenario: Edit non-existent review
- **WHEN** an authenticated client attempts to edit a review that does not exist
- **THEN** the system returns a 404 not-found page

#### Scenario: Invalid edit input rejected
- **WHEN** an authenticated client submits invalid updated review data (rating outside 1-5)
- **THEN** the system redisplays the edit form with validation errors and does not update the review

### Requirement: Client can delete their own review
The system SHALL allow an authenticated client to delete a review they previously created. The controller SHALL call `IReviewService.DeleteAsync(userId, reviewId)`. A confirmation page SHALL be shown before deletion.

#### Scenario: Delete review successfully
- **WHEN** an authenticated client confirms deletion of a review they created
- **THEN** the BLL service deletes the review and the system redirects to `/Client/Bookings/Details/{bookingId}`

#### Scenario: Non-owner attempts to delete
- **WHEN** an authenticated client attempts to delete a review created by a different user
- **THEN** the BLL service throws `UnauthorizedBusinessException` and the controller redirects back with an error message

#### Scenario: Delete non-existent review
- **WHEN** an authenticated client attempts to delete a review that does not exist
- **THEN** the system returns a 404 not-found page
