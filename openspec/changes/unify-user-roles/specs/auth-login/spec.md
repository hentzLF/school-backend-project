## MODIFIED Requirements

### Requirement: User can log in with email and password
The system SHALL verify the provided email and BCrypt password for web MVC login endpoints dedicated to each audience. Admin login requires the `AppUser` to have a `UserRole` with `RoleType.Admin`. Client login requires the `AppUser` to have a `UserRole` with `RoleType.Farmer` or `RoleType.Provider`. The system SHALL issue an authenticated web principal only when credentials and audience role checks both succeed. Each audience login endpoint is independent; credentials valid for one audience do NOT grant access via the other endpoint. Login SHALL always return tokens directly without a profile selection step.

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

#### Scenario: API login always returns tokens directly
- **WHEN** a POST request is sent to `/api/auth/login` with valid credentials
- **THEN** the system returns HTTP 200 with `{ accessToken, refreshToken }` immediately, without requiring profile selection

#### Scenario: User with multiple roles receives all roles in token
- **WHEN** a user with both Farmer and Provider roles logs in via the API
- **THEN** the access token contains both role claims

## REMOVED Requirements

### Requirement: User can select a profile after multi-profile login
**Reason**: The unified role model eliminates multi-profile users. Login always returns tokens directly with all roles included.
**Migration**: Remove `POST /api/auth/select-profile` endpoint. Clients should use the login response directly — no profile selection step needed.
