## MODIFIED Requirements

### Requirement: Listing mutations verify caller ownership
PUT and DELETE on /listings/{id} SHALL verify that the `profileId` claim in the JWT matches `listing.UserProfileId`. A mismatch MUST return 403 Forbidden without leaking resource details. The `profileId` claim represents the user's single profile (1:1 with AppUser) and remains the ownership key for all resources.

#### Scenario: Owner may update their listing
- **WHEN** an authenticated user with profileId X calls PUT /listings/{id} where the listing's UserProfileId is X
- **THEN** the update is applied and 200 OK is returned

#### Scenario: Non-owner is forbidden from updating a listing
- **WHEN** an authenticated user with profileId Y calls PUT /listings/{id} where the listing's UserProfileId is X (X ≠ Y)
- **THEN** the system returns 403 Forbidden

#### Scenario: Non-owner is forbidden from deleting a listing
- **WHEN** an authenticated user with profileId Y calls DELETE /listings/{id} where the listing's UserProfileId is X (X ≠ Y)
- **THEN** the system returns 403 Forbidden
