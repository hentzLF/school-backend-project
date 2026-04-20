## ADDED Requirements

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
The API SHALL expose `POST /api/listings` accepting a `CreateListingRequest` and returning the created `ServiceListingResponse`.

#### Scenario: Valid request
- **WHEN** `POST /api/listings` is called with a valid `CreateListingRequest`
- **THEN** the response returns HTTP 201 with the created resource and a `Location` header pointing to `GET /api/listings/{newId}`

#### Scenario: Missing required fields
- **WHEN** `POST /api/listings` is called with a missing `Title` or `UserProfileId`
- **THEN** the response returns HTTP 400 with a ProblemDetails body listing validation errors

### Requirement: Update service listing
The API SHALL expose `PUT /api/listings/{id}` accepting an `UpdateListingRequest` and returning the updated `ServiceListingResponse`.

#### Scenario: Valid update
- **WHEN** `PUT /api/listings/{id}` is called with a valid body
- **THEN** the response returns HTTP 200 with the updated resource

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
