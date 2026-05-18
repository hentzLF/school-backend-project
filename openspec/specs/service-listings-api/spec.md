## Purpose

Defines the REST API endpoints, request/response DTO shapes, and validation rules for managing `ServiceListing` resources in the AgriMarket API.

---
## Requirements
### Requirement: List service listings
The API SHALL expose `GET /api/listings` returning a paginated list of active `ServiceListing` records.

#### Scenario: Default pagination
- **WHEN** `GET /api/listings` is called with no query params
- **THEN** the response returns HTTP 200 with `{ items, page: 1, pageSize: 20, totalCount }`

#### Scenario: Custom page and size
- **WHEN** `GET /api/listings?page=2&pageSize=10` is called
- **THEN** the response returns the correct page slice with matching `page` and `pageSize` in the body

#### Scenario: pageSize exceeds maximum
- **WHEN** `GET /api/listings?pageSize=200` is called
- **THEN** the API SHALL clamp pageSize to 100 and return at most 100 items

### Requirement: Get single service listing
The API SHALL expose `GET /api/listings/{id}` returning a single `ServiceListingResponse`.

#### Scenario: Existing listing
- **WHEN** `GET /api/listings/{id}` is called with a valid existing ID
- **THEN** the response returns HTTP 200 with a `ServiceListingResponse` body

#### Scenario: Non-existent listing
- **WHEN** `GET /api/listings/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: Create service listing
The API SHALL expose `POST /api/listings` accepting a `CreateListingDto` (from BLL) and returning the created `ListingDto`. The controller SHALL NOT construct a `ServiceListing` entity — it SHALL delegate entity creation to `IListingService.CreateAsync(userId, dto)`.

#### Scenario: Valid request
- **WHEN** `POST /api/listings` is called with a valid `CreateListingDto`
- **THEN** the controller passes the DTO and authenticated userId to the BLL service, and returns HTTP 201 with the `ListingDto` and a `Location` header

#### Scenario: Missing required fields
- **WHEN** `POST /api/listings` is called with a missing `Title`
- **THEN** the response returns HTTP 400 with a ProblemDetails body listing validation errors

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

### Requirement: Delete service listing
The API SHALL expose `DELETE /api/listings/{id}` removing the listing.

#### Scenario: Successful delete
- **WHEN** `DELETE /api/listings/{id}` is called with a valid existing ID
- **THEN** the listing is removed and the response returns HTTP 204

#### Scenario: Non-existent listing
- **WHEN** `DELETE /api/listings/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

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

### Requirement: API controllers do not reference domain entities
API controllers SHALL NOT contain `using AgriMarket.Domain.Entities`. All data exchange with BLL services SHALL use DTOs from `AgriMarket.BLL.Dtos`.

#### Scenario: No entity imports in API controllers
- **WHEN** the API project is compiled
- **THEN** no controller file contains a `using AgriMarket.Domain.Entities` directive

