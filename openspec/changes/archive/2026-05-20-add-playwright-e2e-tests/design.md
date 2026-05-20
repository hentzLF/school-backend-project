## Context

AgriMarket is a dual-stack (API + MVC) agricultural marketplace. The existing test suite (`AgriMarket.Tests`) contains unit tests (services, mappers) and controller integration tests using mocked dependencies. There are no end-to-end tests that exercise the full stack: browser → MVC → BLL → DAL → PostgreSQL.

The MVC web application uses cookie-based authentication, area routing (Admin/Client), anti-forgery tokens in forms, and localization. E2E tests must handle all of these concerns.

MANUAL_TESTING.md documents ~200 manual test scenarios across 17 sections. This design covers automating the critical paths with Playwright.

## Goals / Non-Goals

**Goals:**
- Automate critical user journeys from MANUAL_TESTING.md as Playwright E2E tests
- Run against a real PostgreSQL database (via Testcontainers) with seed data
- Tests are self-contained: no external setup required beyond Docker
- Cover auth, listings, bookings, payments, reviews, equipment, messaging, admin, and authorization
- Page Object Model for maintainability

**Non-Goals:**
- Visual regression testing (screenshot comparison)
- Performance/load testing
- API-only E2E tests (existing integration tests cover this)
- Testing SignalR real-time messaging (Playwright cannot subscribe to WebSocket hubs)
- 100% coverage of every MANUAL_TESTING.md checkbox — focus on critical paths
- Mobile/responsive testing

## Decisions

### 1. Separate E2E project (`AgriMarket.E2E`)

**Decision:** Create a new `AgriMarket.E2E` project instead of adding to `AgriMarket.Tests`.

**Rationale:** E2E tests have different dependencies (Playwright, Testcontainers), longer execution times, and require Docker. Separating them allows `dotnet test --filter` by project and prevents CI from failing when Docker is unavailable.

**Alternative considered:** Adding to existing `AgriMarket.Tests` — rejected because it would slow down the fast unit test feedback loop.

### 2. WebApplicationFactory + Testcontainers PostgreSQL

**Decision:** Use `WebApplicationFactory<Program>` to boot the MVC app in-process, with Testcontainers providing a fresh PostgreSQL container per test class.

**Rationale:** This gives a real HTTP server on a random port with the full middleware pipeline (auth, anti-forgery, routing, localization). Testcontainers ensures test isolation without requiring a pre-existing database.

**Alternative considered:** In-memory SQLite — rejected because it doesn't support PostgreSQL-specific features (EHAK codes, unique constraints behave differently) and would mask real database issues.

### 3. Page Object Model (POM)

**Decision:** Use Page Object classes to encapsulate page interactions and selectors.

**Rationale:** MVC views may change markup. POMs isolate selector changes to one place. Each page object exposes actions (Login, CreateListing) and assertions (IsLoggedIn, HasError).

### 4. Authenticated browser contexts via cookie injection

**Decision:** For tests that need a logged-in user, perform a real login via the `/Client/Account/Login` form in a setup step, then reuse the browser context.

**Rationale:** Cookie-based auth means we can log in once per test class and reuse the session. This is simpler and more realistic than injecting fake cookies.

### 5. xUnit collection fixtures for test lifecycle

**Decision:** Use `IAsyncLifetime` on a shared fixture class that starts the WebApplicationFactory + Testcontainers container once, and share it across test classes using xUnit `[Collection]`.

**Rationale:** Starting PostgreSQL + migrating + seeding takes ~5-10 seconds. Doing this once per test run (not per test) keeps execution time manageable.

### 6. Anti-forgery token handling

**Decision:** Extract `__RequestVerificationToken` from form pages before POST submissions.

**Rationale:** The MVC app uses `[ValidateAntiForgeryToken]` on POST actions. Playwright can read the hidden input value and include it in form submissions naturally since it fills forms like a real user.

## Risks / Trade-offs

- **[Docker dependency]** → Tests require Docker for Testcontainers. CI environments without Docker will skip E2E tests. Mitigated by making the E2E project a separate test project that can be excluded via `--filter`.

- **[Flaky selectors]** → MVC views use framework-generated HTML (tag helpers, validation spans). Mitigated by using `data-testid` attributes where needed, or stable selectors like form `[action]`, `input[name]`, and link text.

- **[Test execution time]** → Full E2E suite with browser automation is slow (~2-5 min). Mitigated by shared fixtures, parallel test classes where safe, and keeping individual tests focused.

- **[Seed data coupling]** → Tests depend on seed data (3 users, 7 categories, counties). If seed data changes, tests break. Mitigated by documenting seed data assumptions and using constants for known values.

- **[Port conflicts]** → WebApplicationFactory uses random ports, which Playwright connects to. No conflict risk.
