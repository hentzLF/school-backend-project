## ADDED Requirements

### Requirement: Update review
The API SHALL expose `PUT /api/reviews/{id}` accepting an `UpdateReviewDto` and returning the updated `ReviewDto`. Only the original reviewer SHALL be allowed to update their review. Only `Rating` and `Comment` fields SHALL be updatable.

#### Scenario: Owner updates their review
- **WHEN** `PUT /api/reviews/{id}` is called by the authenticated user who created the review, with valid `Rating` and `Comment`
- **THEN** the response returns HTTP 200 with the updated `ReviewDto` reflecting the new values

#### Scenario: Non-owner attempts to update
- **WHEN** `PUT /api/reviews/{id}` is called by a user who is not the original reviewer
- **THEN** the response returns HTTP 403 with a ProblemDetails body and the review is not modified

#### Scenario: Update non-existent review
- **WHEN** `PUT /api/reviews/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

#### Scenario: Unauthenticated update
- **WHEN** `PUT /api/reviews/{id}` is called without a valid authentication token
- **THEN** the response returns HTTP 401

### Requirement: UpdateReviewDto shape
`UpdateReviewDto` SHALL include: `Id` (Guid, required), `Rating` (integer 1-5, required), `Comment` (string, optional). The `Id` field SHALL be overridden by the route parameter via `ReviewApiMapper.WithRouteId`.

#### Scenario: Rating out of range on update
- **WHEN** `PUT /api/reviews/{id}` is called with `Rating` outside 1-5
- **THEN** the response returns HTTP 400 with a validation error

### Requirement: Delete review
The API SHALL expose `DELETE /api/reviews/{id}` allowing the original reviewer to delete their review. The endpoint SHALL return HTTP 204 on success.

#### Scenario: Owner deletes their review
- **WHEN** `DELETE /api/reviews/{id}` is called by the authenticated user who created the review
- **THEN** the response returns HTTP 204 and the review is removed from the database

#### Scenario: Non-owner attempts to delete
- **WHEN** `DELETE /api/reviews/{id}` is called by a user who is not the original reviewer
- **THEN** the response returns HTTP 403 with a ProblemDetails body and the review is not removed

#### Scenario: Delete non-existent review
- **WHEN** `DELETE /api/reviews/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

#### Scenario: Unauthenticated delete
- **WHEN** `DELETE /api/reviews/{id}` is called without a valid authentication token
- **THEN** the response returns HTTP 401
