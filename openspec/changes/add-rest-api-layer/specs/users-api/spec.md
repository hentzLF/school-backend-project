## ADDED Requirements

### Requirement: List user profiles
The API SHALL expose `GET /api/users` returning a paginated list of `UserProfile` records.

#### Scenario: Default pagination
- **WHEN** `GET /api/users` is called with no query params
- **THEN** the response returns HTTP 200 with `{ items, page: 1, pageSize: 20, totalCount }`

### Requirement: Get single user profile
The API SHALL expose `GET /api/users/{id}` returning a single `UserProfileResponse`.

#### Scenario: Existing user profile
- **WHEN** `GET /api/users/{id}` is called with a valid existing ID
- **THEN** the response returns HTTP 200 with a `UserProfileResponse` body

#### Scenario: Non-existent user profile
- **WHEN** `GET /api/users/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: UserProfileResponse DTO shape
`UserProfileResponse` SHALL include: `id`, `firstName`, `lastName`, `bio`, `avatarUrl`, `appUserId`, `email` (from linked `AppUser`).

#### Scenario: Email is included in response
- **WHEN** `GET /api/users/{id}` is called for a profile with a linked `AppUser`
- **THEN** the response JSON includes an `email` field with the user's email address

#### Scenario: Response does not include navigation collections
- **WHEN** a user profile endpoint returns a `UserProfileResponse`
- **THEN** the JSON does not contain nested `serviceListings`, `clientBookings`, or `reviews` arrays
