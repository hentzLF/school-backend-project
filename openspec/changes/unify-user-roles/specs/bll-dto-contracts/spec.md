## MODIFIED Requirements

### Requirement: UserProfileResponse DTO shape
`UserProfileResponse` SHALL include: `id`, `firstName`, `lastName`, `bio`, `avatarUrl`, `appUserId`, `email` (from linked `AppUser`), `roles` (list of `RoleType` values from the user's `UserRole` records).

#### Scenario: Email is included in response
- **WHEN** `GET /api/users/{id}` is called for a profile with a linked `AppUser`
- **THEN** the response JSON includes an `email` field with the user's email address

#### Scenario: Roles reflect unified user roles
- **WHEN** `GET /api/users/{id}` is called for a profile whose AppUser has Farmer and Provider roles
- **THEN** the response JSON includes `roles: ["Farmer", "Provider"]`

#### Scenario: Response does not include navigation collections
- **WHEN** a user profile endpoint returns a `UserProfileResponse`
- **THEN** the JSON does not contain nested `serviceListings`, `clientBookings`, or `reviews` arrays
