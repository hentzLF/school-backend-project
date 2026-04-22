## 1. Client Area Foundation

- [x] 1.1 Create `Areas/Client` structure (`Controllers/`, `ViewModels/`, `Views/`) with `_ViewStart.cshtml`, `_ViewImports.cshtml`, and `_ClientLayout.cshtml` (navigation items per spec: Listings always visible; My Bookings/Profile/Logout for authenticated; Login/Register for unauthenticated).
- [x] 1.2 Register Client area route in `Program.cs` with pattern `Client/{controller=Listings}/{action=Index}/{id?}` and verify default route resolves to `ListingsController.Index`.
- [x] 1.3 Add `ClientOnly` authorization policy (requires `RoleType.Farmer` or `RoleType.Provider`) and apply `[Authorize(Policy = "ClientOnly")]` to all Client area controllers except `AccountController` and `ListingsController`.

## 2. Client Account and Authentication Separation

- [x] 2.1 Implement `Areas/Client/Controllers/AccountController` with `Login` (GET/POST), `Register` (GET/POST with Farmer/Provider role selector), `Logout` (POST), and `AccessDenied` (GET). Create dedicated view models and views for each. Post-login and post-register redirect target is `/Client/Listings`.
- [x] 2.2 Move existing shared `AccountController` login/register/logout to `Areas/Admin/Controllers/AccountController` (preserving existing admin routes). Ensure no existing admin navigation or redirect target breaks.
- [x] 2.3 Implement `CookieAuthenticationEvents` with `OnRedirectToLogin` and `OnRedirectToAccessDenied` handlers that inspect the request path prefix: `/Admin/` → admin endpoints, `/Client/` or fallback → client endpoints. Set `CookieAuthenticationOptions.LoginPath` to `/Client/Account/Login` and `AccessDeniedPath` to `/Client/Account/AccessDenied` as defaults.
- [x] 2.4 Enforce audience role validation in each login POST: admin login requires `RoleType.Admin`; client login requires `RoleType.Farmer` or `RoleType.Provider`. Return an explicit authorization error message (not a redirect) when credentials are valid but role check fails.

## 3. Client Listings and Booking Creation

- [x] 3.1 Implement `ListingsController.Index` (no `[Authorize]`) showing active listings with title, category, provider, and price-per-hectare. Include empty-state message when no active listings exist.
- [x] 3.2 Implement `ListingsController.Details` (no `[Authorize]`) showing full listing details. Show booking action entry point to authenticated users only; show login prompt to unauthenticated users. Return 404 for non-existent or inactive listings.
- [x] 3.3 Implement booking creation POST (`[Authorize(Policy = "ClientOnly")]`) with server-side validation, ownership assignment to authenticated client profile, and redirect to `/Client/Bookings/Details/{id}` of the newly created booking on success.

## 4. Client Booking and Profile Management

- [x] 4.1 Implement `BookingsController.Index` listing only bookings owned by the authenticated client, with status and key metadata. Include empty-state message when no bookings exist.
- [x] 4.2 Implement `BookingsController.Details` with ownership check; return 403/redirect if the authenticated client does not own the requested booking.
- [x] 4.3 Implement booking completion confirmation POST (`ProviderCompleted` → `ClientConfirmed`) with status guard: reject the action and preserve current status if booking is not in `ProviderCompleted`.
- [x] 4.4 Implement `ProfileController` view (GET) and edit (GET/POST) for `UserProfile` name and contact fields. Display assigned role as read-only. Return validation errors on invalid input; show success message on persist.

## 5. Verification and Regression Safety

- [x] 5.1 Validate admin area authentication and navigation still function after the auth controller split: admin login, access-denied redirect, and all existing admin routes resolve correctly.
- [x] 5.2 Validate client area access control: unauthenticated users are redirected to `/Client/Account/Login`; a user with only `RoleType.Admin` is redirected to `/Client/Account/AccessDenied` when accessing a protected client route.
- [x] 5.3 Add tests for: (a) client login succeeds with Farmer/Provider role and fails with Admin role; (b) admin login succeeds with Admin role and fails with Farmer/Provider role; (c) booking ownership check blocks access to a booking owned by a different client; (d) completion confirmation is rejected when booking status is not `ProviderCompleted`.
- [x] 5.4 Run a final manual flow pass: client register as Farmer → login → browse listings (unauthenticated view) → login → view listing details → create booking → view booking details → confirm completion → view profile → edit profile → logout.

