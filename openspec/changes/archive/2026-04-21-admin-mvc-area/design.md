## Context

AgriMarket.Web is an ASP.NET Core MVC project (.NET 10) that currently has only a HomeController and no authentication. The API project uses JWT bearer auth independently. The domain model supports an Admin role via `ProfileRole.Role == RoleType.Admin` at the profile level. The Web project references AgriMarket.DAL and has access to the full EF Core DbContext.

## Goals / Non-Goals

**Goals:**
- Cookie-based authentication in AgriMarket.Web independent from the API's JWT system
- Admin MVC area with sidebar layout, role-gated access, and full CRUD for core entities
- Dashboard with comprehensive platform statistics
- Strictly ViewModel-driven views — no ViewBag/ViewData

**Non-Goals:**
- Public-facing user registration/login for non-admin users (future work)
- Real-time dashboard updates (WebSocket/SignalR)
- Admin API endpoints (the admin UI talks directly to DbContext, not the API)
- Audit logging of admin actions
- Pagination/search on initial implementation (can be added later but basic filtering by status is included)

## Decisions

### 1. Cookie auth directly against the database

The Web project will configure `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)` and validate credentials directly against `AppDbContext`. On login, it hashes the password with the same BCrypt logic used by `AuthService`, looks up the user's profiles and roles, and issues a cookie with claims including `ClaimTypes.Role = "Admin"`.

**Why not reuse the API's AuthService?** The API project is a separate assembly with JWT-specific dependencies. Extracting shared auth logic into a service layer would be cleaner long-term, but for now a simple `AdminAuthService` in the Web project avoids cross-project coupling. The password hashing is a one-liner with BCrypt.

### 2. Admin area uses ASP.NET MVC Areas

Standard `[Area("Admin")]` attribute routing with `Areas/Admin/Controllers/`, `Areas/Admin/Views/`, and `Areas/Admin/ViewModels/`. Area route registered in Program.cs:
```
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");
```

### 3. Authorization via policy, not just role attribute

Define an `"AdminOnly"` authorization policy that requires `ClaimTypes.Role == "Admin"`. Apply `[Authorize(Policy = "AdminOnly")]` on a base controller or each admin controller. This is more explicit than `[Authorize(Roles = "Admin")]` and easier to extend later.

### 4. ViewModels in the Area directory

ViewModels live at `Areas/Admin/ViewModels/<Entity>/` rather than a shared Models folder. Each view gets its own ViewModel. List views use a ViewModel with an `IEnumerable<T>` of item ViewModels plus filter/summary properties.

### 5. Admin layout with Bootstrap sidebar

The admin area gets its own `_AdminLayout.cshtml` with a collapsible sidebar listing all admin sections (Dashboard, Users, Listings, Bookings, Categories, Payments). Uses Bootstrap 5 (already included in the Web project). The sidebar highlights the current section.

### 6. DbContext access directly from controllers

Admin controllers inject `AppDbContext` directly and use LINQ queries. No repository/service layer — this is an internal admin tool, not a public API. Keeps the implementation simple and avoids premature abstraction.

### 7. AppUser.CreatedAt migration

Add `DateTime CreatedAt` to `AppUser`. Migration sets default to `DateTime.UtcNow` for existing rows. Seeder and registration logic updated to set `CreatedAt`.

## Risks / Trade-offs

- **[Risk] Password hashing duplication** — BCrypt verify logic exists in both API's `AuthService` and Web's `AdminAuthService`. → Mitigation: Both are one-line BCrypt calls. Can extract to shared library later if needed.
- **[Risk] No CSRF protection by default** — MVC forms need `@Html.AntiForgeryToken()` and `[ValidateAntiForgeryToken]` on POST actions. → Mitigation: Include in all form views and POST actions.
- **[Risk] No pagination on list views** — Large datasets could slow down list pages. → Mitigation: Acceptable for initial admin tool with moderate data. Add pagination as a follow-up.
- **[Trade-off] Direct DbContext in controllers** — Couples controllers to EF. Acceptable for admin CRUD; would not do this for public-facing code.
- **[Trade-off] Admin area in Web project** — Could be a separate project. Keeping it in Web is simpler and avoids another deployment unit.
