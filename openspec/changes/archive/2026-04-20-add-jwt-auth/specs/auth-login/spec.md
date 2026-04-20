## ADDED Requirements

### Requirement: User can log in with email and password
The system SHALL verify the provided email and BCrypt password. If the user has exactly one profile, it SHALL return access and refresh tokens directly. If the user has multiple profiles, it SHALL return a short-lived session token (2-minute JWT containing only userId) and a list of available profiles.

#### Scenario: Login with single profile returns tokens
- **WHEN** a POST request is sent to `/api/auth/login` with valid credentials and the user has exactly one profile
- **THEN** the system returns HTTP 200 with `{ accessToken, refreshToken }`

#### Scenario: Login with multiple profiles returns session token and profile list
- **WHEN** a POST request is sent to `/api/auth/login` with valid credentials and the user has more than one profile
- **THEN** the system returns HTTP 200 with `{ sessionToken, profiles: [{ profileId, fullName, role }] }`

#### Scenario: Invalid password rejected
- **WHEN** a POST request is sent to `/api/auth/login` with a correct email but wrong password
- **THEN** the system returns HTTP 401 Unauthorized

#### Scenario: Unknown email rejected
- **WHEN** a POST request is sent to `/api/auth/login` with an email that does not exist
- **THEN** the system returns HTTP 401 Unauthorized (same response as wrong password — no user enumeration)

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
