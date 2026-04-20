# Spec: Booking Authorization

## Purpose

Defines authentication and authorization rules for all booking endpoints. Ensures bookings are scoped to involved parties (client and provider), prevents unauthorized access to booking details, and gates status transitions by the caller's role.

---

## Requirements

### Requirement: Booking creation is scoped to caller
POST /bookings SHALL require authentication. The `ClientProfileId` on the created booking MUST be set to the `profileId` claim from the JWT. The request body MUST NOT accept a `ClientProfileId` field.

#### Scenario: Booking is created under the caller's profile
- **WHEN** an authenticated user with profileId X calls POST /bookings
- **THEN** the created booking has ClientProfileId = X

### Requirement: Booking list is scoped to involved parties
GET /bookings SHALL require authentication. The response MUST only include bookings where the caller's `profileId` matches `ClientProfileId` OR matches the `UserProfileId` of the booking's `ServiceListing`.

#### Scenario: Client sees their own bookings
- **WHEN** an authenticated user with profileId X calls GET /bookings
- **THEN** only bookings with ClientProfileId = X are returned (plus those where caller is the provider)

#### Scenario: Provider sees bookings for their listings
- **WHEN** an authenticated user with profileId X calls GET /bookings and X owns a ServiceListing that has bookings
- **THEN** those bookings are included in the response

#### Scenario: Unrelated bookings are not returned
- **WHEN** an authenticated user with profileId X calls GET /bookings
- **THEN** bookings where X is neither client nor listing owner are not returned

### Requirement: Booking detail requires involvement
GET /bookings/{id} SHALL require authentication. The system MUST return 403 Forbidden if the caller is neither the client nor the provider of the booking's listing.

#### Scenario: Client may view their booking
- **WHEN** an authenticated user with profileId X calls GET /bookings/{id} where booking.ClientProfileId = X
- **THEN** the system returns 200 OK with the booking

#### Scenario: Provider may view a booking on their listing
- **WHEN** an authenticated user with profileId X calls GET /bookings/{id} where the booking's listing.UserProfileId = X
- **THEN** the system returns 200 OK with the booking

#### Scenario: Uninvolved caller is forbidden
- **WHEN** an authenticated user with profileId X calls GET /bookings/{id} where X is neither client nor provider
- **THEN** the system returns 403 Forbidden

### Requirement: Booking status transitions are role-gated
PATCH /bookings/{id}/status SHALL require authentication. The allowed transitions SHALL differ by the caller's role (client or provider). A caller who is neither party MUST receive 403. A legal-role caller requesting a disallowed transition MUST receive 422.

Allowed transitions by role:
- **Client**: Pending → Cancelled, Confirmed → Cancelled, ProviderCompleted → ClientConfirmed
- **Provider**: Pending → Confirmed, Pending → Cancelled, Confirmed → InProgress, InProgress → ProviderCompleted, any active state → Disputed

#### Scenario: Provider confirms a pending booking
- **WHEN** an authenticated provider calls PATCH /bookings/{id}/status with status=Confirmed and current status=Pending
- **THEN** the booking status is updated to Confirmed and 200 OK is returned

#### Scenario: Client cancels a pending booking
- **WHEN** an authenticated client calls PATCH /bookings/{id}/status with status=Cancelled and current status=Pending
- **THEN** the booking status is updated to Cancelled and 200 OK is returned

#### Scenario: Client confirms completion
- **WHEN** an authenticated client calls PATCH /bookings/{id}/status with status=ClientConfirmed and current status=ProviderCompleted
- **THEN** the booking status is updated to ClientConfirmed and 200 OK is returned

#### Scenario: Client attempts a provider-only transition
- **WHEN** an authenticated client calls PATCH /bookings/{id}/status with status=Confirmed
- **THEN** the system returns 422 Unprocessable Entity

#### Scenario: Uninvolved caller is forbidden from status change
- **WHEN** an authenticated user who is neither client nor provider calls PATCH /bookings/{id}/status
- **THEN** the system returns 403 Forbidden
