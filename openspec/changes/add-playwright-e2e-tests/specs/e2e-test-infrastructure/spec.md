## ADDED Requirements

### Requirement: E2E test project exists
The solution SHALL contain an `AgriMarket.E2E` xUnit project with dependencies on `Microsoft.Playwright`, `Microsoft.AspNetCore.Mvc.Testing`, and `Testcontainers.PostgreSql`.

#### Scenario: Project builds and references Web project
- **WHEN** `dotnet build AgriMarket.E2E` is executed
- **THEN** the project compiles without errors and has a project reference to `AgriMarket.Web`

### Requirement: Shared test fixture with Testcontainers
The project SHALL provide an `E2EFixture` class implementing `IAsyncLifetime` that starts a PostgreSQL Testcontainers container, creates a `WebApplicationFactory<Program>` configured to use the container's connection string, runs migrations, and seeds the database.

#### Scenario: Fixture initializes database with seed data
- **WHEN** the fixture's `InitializeAsync` completes
- **THEN** the test database contains the 3 seed users (admin, provider, farmer), 7 service categories, and all counties/municipalities

#### Scenario: Fixture disposes cleanly
- **WHEN** all tests in the collection complete
- **THEN** the PostgreSQL container is stopped and removed

### Requirement: Playwright browser lifecycle
The fixture SHALL install Playwright browsers if not present and provide a shared `IBrowser` instance for test classes.

#### Scenario: Browser is available for tests
- **WHEN** a test class receives the fixture
- **THEN** it can create new `IBrowserContext` and `IPage` instances

### Requirement: Authenticated page helper
The project SHALL provide a helper method that logs in as a given user (email/password) by submitting the login form and returns an authenticated `IPage`.

#### Scenario: Login as provider
- **WHEN** `CreateAuthenticatedPage("provider@agrimarket.ee", "Provider123!")` is called
- **THEN** the returned page has a valid authentication cookie and can access `/Client/MyListings`

#### Scenario: Login as admin
- **WHEN** `CreateAuthenticatedPage("admin@agrimarket.ee", "Admin123!")` is called with admin login path
- **THEN** the returned page has a valid authentication cookie and can access `/Admin/Dashboard`

### Requirement: Page Object base class
The project SHALL provide a `PageBase` class that wraps an `IPage` and exposes the base URL, navigation helpers, and common assertion methods.

#### Scenario: Navigate to path
- **WHEN** `NavigateTo("/Client/Listings")` is called on a page object
- **THEN** the browser navigates to `{baseUrl}/Client/Listings`

### Requirement: Test collection attribute
All E2E test classes SHALL share the same `E2EFixture` via an xUnit `[Collection("E2E")]` attribute to avoid multiple container starts.

#### Scenario: Two test classes share one database
- **WHEN** `AuthTests` and `ListingTests` both use `[Collection("E2E")]`
- **THEN** they share the same PostgreSQL container and WebApplicationFactory instance
