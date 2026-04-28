## Purpose

Defines the requirements for Swagger UI and OpenAPI document generation in the AgriMarket API, including environment-specific availability rules.

---

## Requirements

### Requirement: Swagger UI is available in Development
The API SHALL serve Swagger UI at `/swagger` when running in the Development environment.

#### Scenario: Swagger UI loads in Development
- **WHEN** the app runs with `ASPNETCORE_ENVIRONMENT=Development` and a browser navigates to `/swagger`
- **THEN** the Swagger UI page loads and lists all registered API endpoints

#### Scenario: Swagger UI is not available in Production
- **WHEN** the app runs with `ASPNETCORE_ENVIRONMENT=Production`
- **THEN** a request to `/swagger` returns HTTP 404

### Requirement: OpenAPI document is generated
The API SHALL expose an OpenAPI JSON document at `/swagger/v1/swagger.json` describing all endpoints, request bodies, and response shapes.

#### Scenario: OpenAPI document is accessible
- **WHEN** `GET /swagger/v1/swagger.json` is called in Development
- **THEN** the response returns HTTP 200 with a valid OpenAPI 3.x JSON document

#### Scenario: All controllers are documented
- **WHEN** the OpenAPI document is fetched
- **THEN** it contains entries for `/api/listings`, `/api/bookings`, `/api/users`, and `/api/reviews`
