## MODIFIED Requirements

### Requirement: CreateListingRequest DTO shape
`CreateListingDto` SHALL accept an optional nested `location` object (of type `CreateLocationDto`) instead of `LocationId`. The `CreateLocationDto` SHALL contain: `municipalityId` (Guid, required), `address` (string, optional), `latitude` (double, optional), `longitude` (double, optional).

#### Scenario: Request with inline location
- **WHEN** `POST /api/v1/listings` is called with `{ "title": "...", "location": { "municipalityId": "..." } }`
- **THEN** the system SHALL create a Location and associate it with the new Listing

#### Scenario: Request without location
- **WHEN** `POST /api/v1/listings` is called without a `location` field
- **THEN** the Listing SHALL be created with no associated Location

#### Scenario: Request is validated before hitting the database
- **WHEN** `POST /api/v1/listings` is called with `pricePerHectare: -5`
- **THEN** the response returns HTTP 400 without writing to the database

### Requirement: ServiceListingResponse DTO shape
`ListingDto` SHALL include a nested `LocationDto` object (or null) instead of `LocationId`. The `LocationDto` SHALL contain: `id`, `municipalityId`, `municipalityName`, `countyId`, `countyName`, `address`, `latitude`, `longitude`.

#### Scenario: Response includes nested location
- **WHEN** a Listing endpoint returns a response for a Listing with a Location
- **THEN** the JSON SHALL contain a `location` object with `municipalityName`, `countyName`, and all location fields

#### Scenario: Response for listing without location
- **WHEN** a Listing endpoint returns a response for a Listing without a Location
- **THEN** the JSON SHALL contain `"location": null`

#### Scenario: LocationId no longer in response
- **WHEN** any Listing endpoint returns a response
- **THEN** the JSON SHALL NOT contain a top-level `locationId` field

### Requirement: Update service listing
The API SHALL expose `PUT /api/v1/listings/{id}` accepting an `UpdateListingDto` with an optional nested `location` object (of type `UpdateLocationDto`) instead of `LocationId`.

#### Scenario: Valid update with location change
- **WHEN** `PUT /api/v1/listings/{id}` is called with a `location` object by the listing owner
- **THEN** the Location SHALL be updated (or created) and the response SHALL return HTTP 200 with the updated ListingDto

#### Scenario: Remove location via update
- **WHEN** `PUT /api/v1/listings/{id}` is called with `"location": null`
- **THEN** the existing Location SHALL be deleted and the response SHALL show `"location": null`

#### Scenario: Non-existent listing
- **WHEN** `PUT /api/v1/listings/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body
