# auth-login

## Purpose

Defines the login flow for users authenticating with email and password, including single-profile and multi-profile login paths, as well as profile selection after multi-profile login.

## Requirements

### Requirement: User can log in with email and password
The system SHALL verify the provided email and BCrypt password for web MVC login endpoints dedicated to each audience. Admin login requires at least one `UserProfile` with `RoleType.Admin`. Client login requires at least one `UserProfile` with `RoleType.Farmer` or `RoleType.Provider`. The system SHALL issue an authenticated web principal only when credentials and audience role checks both succeed. Each audience login endpoint is independent; credentials valid for one audience do NOT grant access via the other endpoint.

#### Scenario: Successful admin web login
- **WHEN** a POST request is sent to the admin login endpoint with valid credentials and the user has an Admin role
- **THEN** the system signs in and redirects to the admin area

#### Scenario: Successful client web login
- **WHEN** a POST request is sent to the client login endpoint with valid credentials and the user has a client-facing role
- **THEN** the system signs in and redirects to the client area

#### Scenario: Valid credentials but missing required audience role
- **WHEN** a POST request is sent to either audience login endpoint with valid credentials but without the role required by that endpoint
- **THEN** the system rejects the login and returns an authorization error message

#### Scenario: Invalid credentials
- **WHEN** a POST request is sent to an audience login endpoint with invalid email or password
- **THEN** the system rejects login without revealing whether the email exists

### Requirement: User can select a profile after multi-profile login
The system SHALL accept a session token and a profileId. It SHALL validate that the session token is a valid, non-expired JWT containing only userId, and that the profileId belongs to that user. It SHALL then return full access and refresh tokens.

#### Scenario: Valid profile selection returns tokens
- **WHEN** a POST request is sent to `/api/auth/select-profile` with a valid session token and a profileId that belongs to the user
- **THEN** the system returns HTTP 200 with `{ accessToken, refreshToken }`

#### Scenario: Expired session token rejected
- **WHEN** a POST request is sent to `/api/auth/select-profile` with a session token older than 2 minutes
- **THEN** the system returns HTTP 401 Unauthorized

#### Scenario: Profile not owned by user rejected
- **WHEN** a POST request is sent to `/api/auth/select-profile` with a valid session token but a profileId that belongs to a different user
- **THEN** the system returns HTTP 403 Forbidden

#### Scenario: Full access token rejected as session token
- **WHEN** a POST request is sent to `/api/auth/select-profile` with a full JWT access token (contains profileId claim)
- **THEN** the system returns HTTP 401 Unauthorized
