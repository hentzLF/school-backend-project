## ADDED Requirements

### Requirement: User can register a new account
The system SHALL accept a registration request containing email, password, first name, last name, and role (Farmer or Provider). It SHALL create an `AppUser`, a `UserProfile`, and a `ProfileRole`, then return HTTP 201 with no tokens.

#### Scenario: Successful registration as Farmer
- **WHEN** a POST request is sent to `/api/auth/register` with valid email, password, firstName, lastName, and role "Farmer"
- **THEN** the system creates AppUser + UserProfile + ProfileRole(Farmer) and returns HTTP 201

#### Scenario: Successful registration as Provider
- **WHEN** a POST request is sent to `/api/auth/register` with valid email, password, firstName, lastName, and role "Provider"
- **THEN** the system creates AppUser + UserProfile + ProfileRole(Provider) and returns HTTP 201

#### Scenario: Duplicate email rejected
- **WHEN** a POST request is sent to `/api/auth/register` with an email that already exists in AppUsers
- **THEN** the system returns HTTP 409 Conflict with a ProblemDetails body

#### Scenario: Admin role not self-assignable
- **WHEN** a POST request is sent to `/api/auth/register` with role "Admin"
- **THEN** the system returns HTTP 400 Bad Request

#### Scenario: Password too short rejected
- **WHEN** a POST request is sent to `/api/auth/register` with a password shorter than 6 characters
- **THEN** the system returns HTTP 400 Bad Request with validation errors

#### Scenario: Password too long rejected
- **WHEN** a POST request is sent to `/api/auth/register` with a password longer than 20 characters
- **THEN** the system returns HTTP 400 Bad Request with validation errors

#### Scenario: Password with non-alphanumeric characters rejected
- **WHEN** a POST request is sent to `/api/auth/register` with a password containing characters other than letters (a-z, A-Z) and digits (0-9)
- **THEN** the system returns HTTP 400 Bad Request with validation errors

### Requirement: Password is stored as a BCrypt hash
The system SHALL hash the provided password using BCrypt before persisting it. The plain-text password SHALL NOT be stored.

#### Scenario: Password is hashed on registration
- **WHEN** a user registers successfully
- **THEN** the stored PasswordHash in AppUser is a valid BCrypt hash and does not equal the plain-text password
