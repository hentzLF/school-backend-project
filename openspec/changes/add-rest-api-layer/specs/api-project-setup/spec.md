## ADDED Requirements

### Requirement: AgriMarket.Api project exists in the solution
A new ASP.NET Core Web API project named `AgriMarket.Api` SHALL exist in the solution and reference both `AgriMarket.DAL` and `AgriMarket.Domain`.

#### Scenario: Project builds successfully
- **WHEN** `dotnet build` is run on the solution
- **THEN** `AgriMarket.Api` compiles without errors alongside existing projects

### Requirement: AppDbContext is registered in the API
`AppDbContext` SHALL be registered in `AgriMarket.Api`'s DI container using the same PostgreSQL connection string as `AgriMarket.Web`.

#### Scenario: Database connection is available to controllers
- **WHEN** a controller is resolved from DI
- **THEN** `AppDbContext` is injected successfully with a valid connection

### Requirement: JSON serialization is configured
The API SHALL serialize responses using camelCase property names and SHALL ignore null values in responses.

#### Scenario: Response uses camelCase
- **WHEN** a GET endpoint returns a response object
- **THEN** all JSON property names are camelCase (e.g., `pricePerHectare`, not `PricePerHectare`)

### Requirement: ProblemDetails error format is enabled
All unhandled exceptions and error responses SHALL return a JSON body conforming to RFC 7807 ProblemDetails with `type`, `title`, `status`, and `detail` fields.

#### Scenario: 404 returns ProblemDetails
- **WHEN** a resource is not found and the controller returns `NotFound()`
- **THEN** the response has `Content-Type: application/problem+json` and a `status: 404` field

#### Scenario: Unhandled exception returns ProblemDetails
- **WHEN** an unhandled exception occurs during a request
- **THEN** the response returns HTTP 500 with a ProblemDetails body
