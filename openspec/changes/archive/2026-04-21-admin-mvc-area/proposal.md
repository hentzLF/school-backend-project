## Why

The AgriMarket platform needs an administrative interface for platform operators to manage users, listings, bookings, categories, and payments/disputes. Currently there is no way to administer the platform — the Web project has no authentication and the API has no admin-facing endpoints. An Admin MVC area provides a server-rendered management dashboard that is independent from the public-facing API.

## What Changes

- Add cookie-based authentication to AgriMarket.Web (login/register flow)
- Create `Areas/Admin/` MVC area with dedicated layout and sidebar navigation
- Protect the entire Admin area behind `RoleType.Admin` authorization
- Add Admin CRUD controllers and views for: Users, Listings, Bookings, Categories, Payments/Disputes
- Add a Dashboard with comprehensive platform statistics (user counts, booking stats, revenue, disputes)
- Add `CreatedAt` field to `AppUser` entity (needed for "new users" metrics)
- Use ViewModels exclusively — no ViewBag/ViewData anywhere in the Admin area
- Admin dispute resolution: change payment status from Disputed to Refunded or Released

## Capabilities

### New Capabilities
- `web-cookie-auth`: Cookie-based authentication for AgriMarket.Web — login, register, sign-out, claims-based identity with role checks
- `admin-area-layout`: Admin MVC area structure with dedicated layout, sidebar navigation, and area routing
- `admin-dashboard`: Platform statistics dashboard — user counts, listing stats, booking breakdowns by status, revenue/fees, dispute counts, trends
- `admin-user-management`: CRUD for AppUsers and their profiles — list, view details, edit, lock/unlock accounts
- `admin-listing-management`: CRUD for ServiceListings — list with filters, view details, edit, activate/deactivate
- `admin-booking-management`: CRUD for Bookings — list with status filters, view details, update status
- `admin-category-management`: CRUD for ServiceCategories — list, create, edit, delete
- `admin-payment-management`: Payment overview and dispute resolution — list payments, filter by status, resolve disputes (Disputed → Refunded or Released)

### Modified Capabilities

## Impact

- **AgriMarket.Web**: Major changes — add authentication middleware, cookie auth config, area routing, all Admin controllers/views/ViewModels
- **AgriMarket.DAL**: New EF migration for `AppUser.CreatedAt` column
- **AgriMarket.Domain**: Add `CreatedAt` property to `AppUser` entity
- **Dependencies**: May need `Microsoft.AspNetCore.Authentication.Cookies` (built-in, no new NuGet package)
- **Database**: Migration required — adds non-nullable `CreatedAt` column with default value to existing `AppUser` rows
