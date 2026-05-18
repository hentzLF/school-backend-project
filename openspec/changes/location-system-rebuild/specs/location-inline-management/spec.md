## ADDED Requirements

### Requirement: Location entity restructure
The `Location` entity SHALL have properties: `Id` (Guid, PK), `MunicipalityId` (Guid, FK to Municipality, required), `Address` (string, optional), `Latitude` (double, optional), `Longitude` (double, optional). The fields `City`, `Country`, and `PostalCode` SHALL be removed.

#### Scenario: Location references a Municipality
- **WHEN** a Location record is read with its navigation property
- **THEN** it SHALL have a non-null `Municipality` reference with a valid `County`

#### Scenario: Address is optional
- **WHEN** a Location is created without an Address
- **THEN** the Location SHALL be persisted with `Address` as null

### Requirement: Coordinate validation
The system SHALL validate that `Latitude` is between -90 and 90 (inclusive), and `Longitude` is between -180 and 180 (inclusive), when coordinates are provided.

#### Scenario: Valid coordinates
- **WHEN** a Location is created with Latitude 59.437 and Longitude 24.7536
- **THEN** the Location SHALL be persisted successfully

#### Scenario: Invalid latitude
- **WHEN** a Location is created with Latitude 91.0
- **THEN** the system SHALL reject the request with a validation error

#### Scenario: Invalid longitude
- **WHEN** a Location is created with Longitude -181.0
- **THEN** the system SHALL reject the request with a validation error

#### Scenario: Coordinates are optional
- **WHEN** a Location is created without Latitude and Longitude
- **THEN** the Location SHALL be persisted with both as null

### Requirement: Create Location inline with Listing
When a Listing is created via `POST /api/v1/listings` with a `location` object in the request body, the system SHALL create a new Location record and associate it with the Listing.

#### Scenario: Listing created with location
- **WHEN** `POST /api/v1/listings` is called with a valid `location` object containing `municipalityId`
- **THEN** a new Location record SHALL be created and the Listing's `LocationId` SHALL reference it

#### Scenario: Listing created without location
- **WHEN** `POST /api/v1/listings` is called without a `location` object
- **THEN** the Listing SHALL be created with `LocationId` as null

#### Scenario: Invalid municipalityId
- **WHEN** `POST /api/v1/listings` is called with a `location.municipalityId` that does not exist
- **THEN** the system SHALL return HTTP 400 with a validation error

### Requirement: Update Location inline with Listing
When a Listing is updated via `PUT /api/v1/listings/{id}` with a `location` object, the system SHALL update or create the Location record accordingly.

#### Scenario: Update existing location
- **WHEN** `PUT /api/v1/listings/{id}` is called with a `location` object and the Listing already has a Location
- **THEN** the existing Location record SHALL be updated with the new values

#### Scenario: Add location to listing without one
- **WHEN** `PUT /api/v1/listings/{id}` is called with a `location` object and the Listing has no Location
- **THEN** a new Location record SHALL be created and associated with the Listing

#### Scenario: Remove location from listing
- **WHEN** `PUT /api/v1/listings/{id}` is called with `location` as null
- **THEN** the existing Location record SHALL be deleted and `LocationId` SHALL be set to null

### Requirement: Delete Location with Listing
When a Listing is deleted, its associated Location record SHALL also be deleted.

#### Scenario: Listing deletion cascades to Location
- **WHEN** a Listing with an associated Location is deleted
- **THEN** the Location record SHALL also be removed from the database

### Requirement: LocationDto in responses
The system SHALL include a nested `LocationDto` object in Listing responses containing: `id`, `municipalityId`, `municipalityName`, `countyId`, `countyName`, `address`, `latitude`, `longitude`.

#### Scenario: Listing response includes location details
- **WHEN** a Listing with a Location is retrieved via API
- **THEN** the response SHALL contain a `location` object with municipality name, county name, and all Location fields

#### Scenario: Listing without location
- **WHEN** a Listing without a Location is retrieved via API
- **THEN** the response SHALL contain `location` as null

### Requirement: CreateLocationDto validation
The `CreateLocationDto` SHALL require `municipalityId` (Guid, required). Fields `address` (string), `latitude` (double?), and `longitude` (double?) are optional. If either latitude or longitude is provided, both SHALL be required.

#### Scenario: Valid minimal location
- **WHEN** a CreateLocationDto is submitted with only `municipalityId`
- **THEN** validation SHALL pass

#### Scenario: Latitude without longitude
- **WHEN** a CreateLocationDto is submitted with `latitude` but without `longitude`
- **THEN** validation SHALL fail with an error indicating both coordinates are required
