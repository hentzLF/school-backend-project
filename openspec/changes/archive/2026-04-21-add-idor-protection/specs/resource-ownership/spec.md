## ADDED Requirements

### Requirement: Mutation endpoints require authentication
All POST, PUT, PATCH, and DELETE endpoints on listings, bookings, and reviews SHALL require a valid JWT. Unauthenticated requests MUST receive 401 Unauthorized.

#### Scenario: Unauthenticated mutation is rejected
- **WHEN** a request is made to PUT /listings/{id} without an Authorization header
- **THEN** the system returns 401 Unauthorized

### Requirement: Listing mutations verify caller ownership
PUT and DELETE on /listings/{id} SHALL verify that the `profileId` claim in the JWT matches `listing.UserProfileId`. A mismatch MUST return 403 Forbidden without leaking resource details.

#### Scenario: Owner may update their listing
- **WHEN** an authenticated user with profileId X calls PUT /listings/{id} where the listing's UserProfileId is X
- **THEN** the update is applied and 200 OK is returned

#### Scenario: Non-owner is forbidden from updating a listing
- **WHEN** an authenticated user with profileId Y calls PUT /listings/{id} where the listing's UserProfileId is X (X ≠ Y)
- **THEN** the system returns 403 Forbidden

#### Scenario: Non-owner is forbidden from deleting a listing
- **WHEN** an authenticated user with profileId Y calls DELETE /listings/{id} where the listing's UserProfileId is X (X ≠ Y)
- **THEN** the system returns 403 Forbidden

### Requirement: Review creation is ownership-safe
POST /reviews SHALL derive `ReviewerProfileId` from the JWT `profileId` claim. The request body MUST NOT accept a `ReviewerProfileId` field.

#### Scenario: Review is created under the caller's profile
- **WHEN** an authenticated user with profileId X calls POST /reviews
- **THEN** the created review has ReviewerProfileId = X regardless of any field in the request body

### Requirement: User profile email is protected
GET /users/{id} SHALL only include the `email` field in the response if the authenticated caller's `sub` claim matches the profile's `AppUserId`. Unauthenticated requests and requests from other users SHALL receive a response without the `email` field.

#### Scenario: Owner sees their own email
- **WHEN** an authenticated user whose AppUser.Id matches the profile's AppUserId calls GET /users/{id}
- **THEN** the response includes the email field

#### Scenario: Other user does not see email
- **WHEN** an authenticated user whose AppUser.Id does NOT match the profile's AppUserId calls GET /users/{id}
- **THEN** the response omits the email field

#### Scenario: Unauthenticated caller does not see email
- **WHEN** an unauthenticated request is made to GET /users/{id}
- **THEN** the response omits the email field
