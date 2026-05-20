## MODIFIED Requirements

### Requirement: Access tokens carry user identity and role claims
The system SHALL issue JWTs containing `sub` (userId), `profileId`, and one `role` claim per assigned role (e.g., both `role: Farmer` and `role: Provider` for a user with both roles). Access tokens SHALL expire after 15 minutes.

#### Scenario: Access token contains required claims
- **WHEN** an access token is issued after successful login
- **THEN** decoding the JWT reveals `sub`, `profileId`, and one or more `role` claims with correct values

#### Scenario: Multi-role user gets multiple role claims
- **WHEN** an access token is issued for a user with Farmer and Provider roles
- **THEN** decoding the JWT reveals two `role` claims: one for Farmer and one for Provider

#### Scenario: Access token expires after 15 minutes
- **WHEN** an access token is used more than 15 minutes after issuance
- **THEN** the request is rejected with HTTP 401 Unauthorized

