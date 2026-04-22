## MODIFIED Requirements

### Requirement: List user profiles
The API SHALL expose `GET /api/users` returning a paginated list of `UserProfileDto` records (from BLL). The controller SHALL delegate to `IUserService` and return the DTOs directly.

#### Scenario: Default pagination
- **WHEN** `GET /api/users` is called with no query params
- **THEN** the response returns HTTP 200 with `UserProfileDto` items

### Requirement: Get single user profile
The API SHALL expose `GET /api/users/{id}` returning a single `UserProfileDto` (from BLL).

#### Scenario: Existing user profile
- **WHEN** `GET /api/users/{id}` is called with a valid existing ID
- **THEN** the response returns HTTP 200 with a `UserProfileDto` body

#### Scenario: Non-existent user profile
- **WHEN** `GET /api/users/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body
