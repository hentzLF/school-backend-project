## MODIFIED Requirements

### Requirement: Booking status transitions are role-gated
PATCH /bookings/{id}/status SHALL require authentication. The allowed transitions SHALL differ by the caller's role relative to the booking. A caller who is neither the client nor the provider MUST receive 403. A caller requesting a disallowed transition MUST receive 422. Role determination SHALL check the user's `UserRole` records (not a single JWT role claim). A user with both Farmer and Provider roles SHALL have their role in a booking determined by whether they are the client or the listing owner, not by their assigned roles.

Allowed transitions by booking relationship:
- **Client** (caller's profileId matches booking.ClientProfileId): Pending → Cancelled, Confirmed → Cancelled, ProviderCompleted → ClientConfirmed
- **Provider** (caller's profileId matches listing.UserProfileId): Pending → Confirmed, Pending → Cancelled, Confirmed → InProgress, InProgress → ProviderCompleted, any active state → Disputed

#### Scenario: Provider confirms a pending booking
- **WHEN** an authenticated provider (listing owner) calls PATCH /bookings/{id}/status with status=Confirmed and current status=Pending
- **THEN** the booking status is updated to Confirmed and 200 OK is returned

#### Scenario: Client cancels a pending booking
- **WHEN** an authenticated client (booking creator) calls PATCH /bookings/{id}/status with status=Cancelled and current status=Pending
- **THEN** the booking status is updated to Cancelled and 200 OK is returned

#### Scenario: Client confirms completion
- **WHEN** an authenticated client calls PATCH /bookings/{id}/status with status=ClientConfirmed and current status=ProviderCompleted
- **THEN** the booking status is updated to ClientConfirmed and 200 OK is returned

#### Scenario: Client attempts a provider-only transition
- **WHEN** an authenticated client (not the listing owner) calls PATCH /bookings/{id}/status with status=Confirmed
- **THEN** the system returns 422 Unprocessable Entity

#### Scenario: Uninvolved caller is forbidden from status change
- **WHEN** an authenticated user who is neither client nor provider calls PATCH /bookings/{id}/status
- **THEN** the system returns 403 Forbidden
