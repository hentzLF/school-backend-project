# auth-tokens

## Purpose

Defines the structure, lifecycle, and rotation behavior of JWT access tokens and refresh tokens used for authentication.

## Requirements

### Requirement: Access tokens carry user identity and profile claims
The system SHALL issue JWTs containing `sub` (userId), `profileId`, and `role` (RoleType as string). Access tokens SHALL expire after 15 minutes.

#### Scenario: Access token contains required claims
- **WHEN** an access token is issued after successful login or profile selection
- **THEN** decoding the JWT reveals `sub`, `profileId`, and `role` claims with correct values

#### Scenario: Access token expires after 15 minutes
- **WHEN** an access token is used more than 15 minutes after issuance
- **THEN** the request is rejected with HTTP 401 Unauthorized

### Requirement: Refresh tokens are stored in the database and rotated on use
The system SHALL persist refresh tokens as `RefreshToken` records linked to the `AppUser`. Each refresh token SHALL have a long expiry (30 days). On each use of `/api/auth/refresh`, the old token SHALL be revoked and a new one issued (rotation).

#### Scenario: Successful token refresh
- **WHEN** a POST request is sent to `/api/auth/refresh` with a valid, non-expired, non-revoked refresh token
- **THEN** the system returns HTTP 200 with a new `{ accessToken, refreshToken }` and marks the old refresh token as revoked

#### Scenario: Revoked refresh token rejected
- **WHEN** a POST request is sent to `/api/auth/refresh` with a refresh token that has already been used or explicitly revoked
- **THEN** the system returns HTTP 401 Unauthorized

#### Scenario: Expired refresh token rejected
- **WHEN** a POST request is sent to `/api/auth/refresh` with a refresh token older than 30 days
- **THEN** the system returns HTTP 401 Unauthorized

### Requirement: User can log out by revoking their refresh token
The system SHALL accept a refresh token on `POST /api/auth/logout`, mark it as revoked in the database, and return HTTP 204. Subsequent refresh attempts with that token SHALL fail.

#### Scenario: Successful logout
- **WHEN** a POST request is sent to `/api/auth/logout` with a valid refresh token
- **THEN** the system marks the token revoked and returns HTTP 204

#### Scenario: Logout with unknown token returns 204
- **WHEN** a POST request is sent to `/api/auth/logout` with a token that does not exist in the database
- **THEN** the system returns HTTP 204 (idempotent — no error exposed)
