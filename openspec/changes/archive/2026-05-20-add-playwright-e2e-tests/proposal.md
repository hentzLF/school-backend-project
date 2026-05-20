## Why

The application has extensive MVC functionality (auth, listings, bookings, payments, reviews, messaging, equipment, admin panel) but zero end-to-end test coverage. Unit and integration tests verify service logic in isolation, but they cannot catch UI regressions, broken form submissions, incorrect redirects, or authorization bypass issues that only surface when a real browser interacts with the running app. Playwright E2E tests will cover the critical user journeys documented in MANUAL_TESTING.md.

## What Changes

- Add a new **AgriMarket.E2E** test project using Playwright for .NET (Microsoft.Playwright + xUnit)
- Create a **WebApplicationFactory-based** test server that boots the MVC app with a fresh PostgreSQL test database (via Testcontainers) and seed data
- Implement E2E test suites covering:
  - **Authentication flows**: client login/register/logout, admin login/register/logout, access denied scenarios
  - **Listing browsing**: public listing index, listing details with equipment/reviews/availability
  - **Provider listing management**: CRUD, toggle active, availability management, equipment assignment
  - **Booking lifecycle**: creation, full status flow (Pending → Confirmed → InProgress → ProviderCompleted → ClientConfirmed), cancellation, double-booking prevention
  - **Payments**: checkout flow, receipt display, payment history
  - **Reviews**: create/edit/delete, rating display on listings
  - **Equipment management**: CRUD, status changes, listing assignment
  - **Messaging**: conversation creation, send/receive messages, unread counts
  - **Profile management**: view and edit profile
  - **Admin panel**: dashboard stats, user management (lock/unlock/delete), listing/booking/payment management, category CRUD
  - **Authorization**: role-based access control, cross-user data isolation
  - **Full E2E journeys**: complete service booking lifecycle, messaging flow, admin dispute resolution

## Capabilities

### New Capabilities
- `e2e-test-infrastructure`: Playwright + WebApplicationFactory + Testcontainers setup, page object helpers, authenticated browser contexts, test database lifecycle
- `e2e-auth-tests`: E2E tests for client and admin authentication flows (login, register, logout, access denied)
- `e2e-listing-tests`: E2E tests for listing browsing, provider CRUD, availability management, equipment assignment
- `e2e-booking-tests`: E2E tests for booking creation, lifecycle status transitions, cancellation, edge cases
- `e2e-payment-tests`: E2E tests for checkout, receipt, payment history
- `e2e-review-tests`: E2E tests for review CRUD and rating display
- `e2e-equipment-tests`: E2E tests for equipment CRUD, status changes, listing assignment
- `e2e-messaging-tests`: E2E tests for conversations, message sending, unread counts
- `e2e-admin-tests`: E2E tests for admin dashboard, user/listing/booking/payment/category management
- `e2e-authorization-tests`: E2E tests for role-based access control and data isolation
- `e2e-journeys`: Full end-to-end scenario tests covering complete user workflows

### Modified Capabilities

_(none — this change adds a new test project without modifying existing application behavior)_

## Impact

- **New project**: `AgriMarket.E2E/` with references to `AgriMarket.Web`
- **New dependencies**: `Microsoft.Playwright`, `Testcontainers.PostgreSql`, `Microsoft.AspNetCore.Mvc.Testing`
- **CI impact**: Tests require Docker (for Testcontainers PostgreSQL) and Playwright browser binaries
- **No application code changes** — tests only exercise existing functionality
- **Test database**: Each test run gets a fresh PostgreSQL container with migrations and seed data applied
