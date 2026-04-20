## ADDED Requirements

### Requirement: JWT bearer middleware is configured in Program.cs
The system SHALL register `AddAuthentication(JwtBearer)` and `AddJwtBearer(...)` in `Program.cs` with signing key, issuer, and audience read from configuration. `UseAuthentication()` and `UseAuthorization()` SHALL be called in the middleware pipeline before `MapControllers()`.

#### Scenario: Valid JWT grants access to protected endpoint
- **WHEN** a request is sent to an `[Authorize]`-decorated endpoint with a valid, non-expired JWT in the `Authorization: Bearer <token>` header
- **THEN** the request is processed and returns the expected response

#### Scenario: Missing token rejected on protected endpoint
- **WHEN** a request is sent to an `[Authorize]`-decorated endpoint with no Authorization header
- **THEN** the system returns HTTP 401 Unauthorized

#### Scenario: Expired token rejected
- **WHEN** a request is sent to an `[Authorize]`-decorated endpoint with an expired JWT
- **THEN** the system returns HTTP 401 Unauthorized

#### Scenario: Tampered token rejected
- **WHEN** a request is sent to an `[Authorize]`-decorated endpoint with a JWT whose signature does not match the signing key
- **THEN** the system returns HTTP 401 Unauthorized

### Requirement: JWT signing key is loaded from configuration
The system SHALL read the JWT signing key from `appsettings.json` under `Jwt:Key`. In production, this value SHALL be overridable via environment variable. The key SHALL be at least 32 characters long.

#### Scenario: Missing JWT key causes startup failure
- **WHEN** the application starts with no `Jwt:Key` value configured
- **THEN** the application throws a configuration exception at startup rather than serving requests with an insecure default
