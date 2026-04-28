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
The API SHALL expose `PUT /api/listings/{id}` accepting an `UpdateListingDto` (from BLL) and returning the updated `ListingDto`. The controller SHALL delegate to `IListingService.UpdateAsync(userId, dto)`.

#### Scenario: Valid update
- **WHEN** `PUT /api/listings/{id}` is called with a valid body by the listing owner
- **THEN** the response returns HTTP 200 with the updated `ListingDto`

#### Scenario: Non-existent listing
- **WHEN** `PUT /api/listings/{id}` is called with an ID that does not exist
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
`ServiceListingResponse` SHALL include: `id`, `title`, `description`, `pricePerHectare`, `isActive`, `userProfileId`, `serviceCategoryId`, `locationId`.

#### Scenario: Response does not include navigation objects
- **WHEN** a listing endpoint returns a `ServiceListingResponse`
- **THEN** the JSON does not contain nested `userProfile`, `serviceCategory`, or `location` objects

### Requirement: CreateListingRequest DTO shape
`CreateListingRequest` SHALL require: `title` (non-empty string), `pricePerHectare` (positive decimal), `userProfileId` (valid Guid), `serviceCategoryId` (valid Guid). `description` and `locationId` are optional.

#### Scenario: Request is validated before hitting the database
- **WHEN** `POST /api/listings` is called with `pricePerHectare: -5`
- **THEN** the response returns HTTP 400 without writing to the database

### Requirement: API controllers do not reference domain entities
API controllers SHALL NOT contain `using AgriMarket.Domain.Entities`. All data exchange with BLL services SHALL use DTOs from `AgriMarket.BLL.Dtos`.

#### Scenario: No entity imports in API controllers
- **WHEN** the API project is compiled
- **THEN** no controller file contains a `using AgriMarket.Domain.Entities` directive
