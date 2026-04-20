## ADDED Requirements

### Requirement: CreateBookingRequest does not accept ClientProfileId
The `CreateBookingRequest` DTO MUST NOT contain a `ClientProfileId` field. The system SHALL derive this value server-side from the authenticated caller's `profileId` JWT claim.

#### Scenario: ClientProfileId field is absent from booking creation contract
- **WHEN** a client sends POST /bookings with a body that includes a `clientProfileId` field
- **THEN** the field is ignored and the booking's ClientProfileId is set to the caller's profileId from the JWT

### Requirement: CreateListingRequest does not accept UserProfileId
The `CreateListingRequest` DTO MUST NOT contain a `UserProfileId` field. The system SHALL derive this value server-side from the authenticated caller's `profileId` JWT claim.

#### Scenario: UserProfileId field is absent from listing creation contract
- **WHEN** a caller sends POST /listings with a body that includes a `userProfileId` field
- **THEN** the field is ignored and the listing's UserProfileId is set to the caller's profileId from the JWT

### Requirement: CreateReviewRequest does not accept ReviewerProfileId
The `CreateReviewRequest` DTO MUST NOT contain a `ReviewerProfileId` field. The system SHALL derive this value server-side from the authenticated caller's `profileId` JWT claim.

#### Scenario: ReviewerProfileId field is absent from review creation contract
- **WHEN** a caller sends POST /reviews with a body that includes a `reviewerProfileId` field
- **THEN** the field is ignored and the review's ReviewerProfileId is set to the caller's profileId from the JWT
