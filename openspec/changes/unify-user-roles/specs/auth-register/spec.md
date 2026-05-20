## MODIFIED Requirements

### Requirement: User can register a new account
The system SHALL accept a registration request containing email, password, first name, and last name. It SHALL create an `AppUser`, a `UserProfile`, and `UserRole` entries for both Farmer and Provider roles, then return HTTP 201 with no tokens. The registration request SHALL NOT accept a role field — all regular users receive both Farmer and Provider roles by default.

#### Scenario: Successful registration
- **WHEN** a POST request is sent to `/api/auth/register` with valid email, password, firstName, and lastName
- **THEN** the system creates AppUser + UserProfile + UserRole(Farmer) + UserRole(Provider) and returns HTTP 201

#### Scenario: Duplicate email rejected
- **WHEN** a POST request is sent to `/api/auth/register` with an email that already exists in AppUsers
- **THEN** the system returns HTTP 409 Conflict with a ProblemDetails body

#### Scenario: Password too short rejected
- **WHEN** a POST request is sent to `/api/auth/register` with a password shorter than 6 characters
- **THEN** the system returns HTTP 400 Bad Request with validation errors

#### Scenario: Password too long rejected
- **WHEN** a POST request is sent to `/api/auth/register` with a password longer than 20 characters
- **THEN** the system returns HTTP 400 Bad Request with validation errors

#### Scenario: Password with non-alphanumeric characters rejected
- **WHEN** a POST request is sent to `/api/auth/register` with a password containing characters other than letters (a-z, A-Z) and digits (0-9)
- **THEN** the system returns HTTP 400 Bad Request with validation errors

#### Scenario: Admin role is not self-assignable
- **WHEN** a user registers via `/api/auth/register`
- **THEN** the system assigns only Farmer and Provider roles; Admin role is never assigned through registration
