## Purpose

Defines rating statistics aggregation for profiles and listings, and the integration of `AverageRating` and `ReviewCount` fields into existing DTOs.

---

## Requirements

### Requirement: Rating statistics for profile
The system SHALL provide a method to compute `AverageRating` (double, rounded to 2 decimals) and `ReviewCount` (int) for a given user profile, based on reviews where `ReviewedProfileId` matches the profile.

#### Scenario: Profile with reviews
- **WHEN** rating stats are requested for a profile that has received 3 reviews with ratings 3, 4, 5
- **THEN** the result SHALL be `{ AverageRating: 4.0, ReviewCount: 3 }`

#### Scenario: Profile with no reviews
- **WHEN** rating stats are requested for a profile that has received no reviews
- **THEN** the result SHALL be `{ AverageRating: 0, ReviewCount: 0 }`

### Requirement: Rating statistics for listing
The system SHALL provide a method to compute `AverageRating` and `ReviewCount` for a given service listing, based on reviews linked through `Review.Booking.ServiceListingId`.

#### Scenario: Listing with reviews
- **WHEN** rating stats are requested for a listing that has bookings with 2 reviews rated 4 and 5
- **THEN** the result SHALL be `{ AverageRating: 4.5, ReviewCount: 2 }`

#### Scenario: Listing with no reviews
- **WHEN** rating stats are requested for a listing that has no reviewed bookings
- **THEN** the result SHALL be `{ AverageRating: 0, ReviewCount: 0 }`

### Requirement: UserProfileDto includes rating statistics
`UserProfileDto` SHALL include `AverageRating` (double) and `ReviewCount` (int) fields, populated from the profile's review statistics.

#### Scenario: UserProfileDto with reviews
- **WHEN** a user profile is fetched via `GET /api/users/{id}` and the profile has received reviews
- **THEN** the response SHALL include `averageRating` and `reviewCount` reflecting the profile's received reviews

#### Scenario: UserProfileDto with no reviews
- **WHEN** a user profile is fetched and the profile has no reviews
- **THEN** the response SHALL include `averageRating: 0` and `reviewCount: 0`

### Requirement: ListingDto includes rating statistics
`ListingDto` SHALL include `AverageRating` (double) and `ReviewCount` (int) fields, populated from the listing's review statistics.

#### Scenario: ListingDto with reviews
- **WHEN** a listing is fetched via `GET /api/listings/{id}` and the listing has reviewed bookings
- **THEN** the response SHALL include `averageRating` and `reviewCount` reflecting the listing's reviews

#### Scenario: ListingDto with no reviews
- **WHEN** a listing is fetched and has no reviewed bookings
- **THEN** the response SHALL include `averageRating: 0` and `reviewCount: 0`

### Requirement: ListingSummaryDto includes rating statistics
`ListingSummaryDto` SHALL include `AverageRating` (double) and `ReviewCount` (int) fields, populated from the listing's review statistics.

#### Scenario: ListingSummaryDto in listing list
- **WHEN** listings are fetched via `GET /api/listings` and a listing has reviewed bookings
- **THEN** each item in the response SHALL include `averageRating` and `reviewCount`
