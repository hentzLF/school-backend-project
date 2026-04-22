## MODIFIED Requirements

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

### Requirement: API controllers do not reference domain entities
API controllers SHALL NOT contain `using AgriMarket.Domain.Entities`. All data exchange with BLL services SHALL use DTOs from `AgriMarket.BLL.Dtos`.

#### Scenario: No entity imports in API controllers
- **WHEN** the API project is compiled
- **THEN** no controller file contains a `using AgriMarket.Domain.Entities` directive
