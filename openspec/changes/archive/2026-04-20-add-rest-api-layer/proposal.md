## Why

The project currently has `AgriMarket.Web` (MVC/Razor) but no REST API surface. A dedicated API layer is needed so that frontend clients and mobile apps can consume structured JSON endpoints, and so that authentication, versioning, and API-specific middleware can be managed separately from the server-rendered web app.

## What Changes

- New `AgriMarket.Api` project added to the solution alongside `AgriMarket.Web`
- REST controllers added for core domain resources (listings, bookings, users, reviews)
- DTOs introduced to decouple API contracts from domain entities
- Swagger/OpenAPI configured for documentation and manual testing

## Capabilities

### New Capabilities

- `api-project-setup`: Scaffold `AgriMarket.Api` project, wire DI (DAL, Domain), configure JSON serialization and routing
- `service-listings-api`: CRUD endpoints for `ServiceListing` with request/response DTOs
- `bookings-api`: Endpoints for creating and managing `Booking` records
- `users-api`: Read endpoints for `UserProfile` and `AppUser` data
- `reviews-api`: Endpoints for submitting and reading `Review` records
- `swagger-setup`: Integrate Swashbuckle, configure OpenAPI doc generation and Swagger UI

### Modified Capabilities

## Impact

- Adds one new project: `AgriMarket.Api` (references `AgriMarket.DAL` and `AgriMarket.Domain`)
- No changes to `AgriMarket.Web`, `AgriMarket.DAL`, or `AgriMarket.Domain`
- Requires updating `AgriMarket.slnx` to include the new project
- Swagger UI served only in Development environment
