## MODIFIED Requirements

### Requirement: List user profiles
The API SHALL expose `GET /api/users` returning a paginated list of `UserProfileDto` records (from BLL). The controller SHALL delegate to `IUserService` and return the DTOs directly with privacy-safe `email` handling.

#### Scenario: Default pagination
- **WHEN** `GET /api/users` is called with no query params
- **THEN** the response returns HTTP 200 with `UserProfileDto` items

#### Scenario: Email is not disclosed in list endpoint
- **WHEN** `GET /api/users` returns profile items
- **THEN** each `UserProfileDto.email` is `null`

### Requirement: Get single user profile
The API SHALL expose `GET /api/users/{id}` returning a single `UserProfileDto` (from BLL). The `email` field SHALL be populated only when the caller is the profile owner or an admin; otherwise it SHALL be `null`.

#### Scenario: Existing user profile
- **WHEN** `GET /api/users/{id}` is called with a valid existing ID
- **THEN** the response returns HTTP 200 with a `UserProfileDto` body

#### Scenario: Email hidden for non-owner
- **WHEN** `GET /api/users/{id}` is called by a caller who is not the profile owner and not an admin
- **THEN** the response returns HTTP 200 and `UserProfileDto.email` is `null`

#### Scenario: Non-existent user profile
- **WHEN** `GET /api/users/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body
