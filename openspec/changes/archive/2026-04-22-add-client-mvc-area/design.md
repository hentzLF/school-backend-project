## Context

`AgriMarket.Web` currently exposes a complete Admin MVC area and a single `AccountController` flow that is admin-oriented for both login and registration. This leaves the client booking journey unavailable in MVC even though core domain entities (`ServiceListing`, `Booking`, `UserProfile`) already exist and are used by admin features.

The change introduces a new Client MVC area and separates audience-specific web authentication entry points so admin and client flows remain distinct in routing, UX, and authorization expectations.

Constraints:
- Keep parity with existing Admin area structure and conventions (strongly typed view models, area routing, shared layout usage).
- Preserve existing admin behavior and routes.
- Avoid breaking API auth contracts and focus on MVC/web behavior.

## Goals / Non-Goals

**Goals:**
- Add a functional `Areas/Client` MVC surface for browse, details, booking, account/profile, and booking management.
- Establish explicit separate login flows for admin and client users.
- Keep cookie-auth and policy behavior predictable by area and role.
- Reuse existing domain/data model and repository patterns where possible.

**Non-Goals:**
- Redesigning UI/branding beyond functional pages.
- Replacing cookie authentication with JWT for MVC.
- Reworking existing API endpoints or mobile-facing authentication flows.
- Introducing provider-facing MVC area in this change.

## Decisions

### Decision: Keep one identity store, split web entry points by audience
- **Choice:** Continue using the same `AppUser`/`UserProfile` model, but expose separate MVC endpoints (`/Admin/Account/*` and `/Client/Account/*`) and role checks.
- **Rationale:** Prevents account duplication and data fragmentation while giving clear UX and security boundaries.
- **Alternatives considered:**
  - Separate user tables for admin/client: stronger physical separation but high migration/maintenance cost.
  - Single shared login form with post-login branching: less explicit, easier to misuse and harder to secure by route intent.

### Decision: Introduce a dedicated Client area mirroring Admin area conventions
- **Choice:** Create `Areas/Client/{Controllers,ViewModels,Views}` with a client layout and area route.
- **Rationale:** Aligns with existing architecture, improves maintainability, and reduces onboarding cost.
- **Alternatives considered:**
  - Flat controllers/views outside areas: simpler initially but weak separation and routing clarity.
  - Razor Pages for client while admin stays MVC: mixed paradigms increase complexity.

### Decision: Two separate login pages, single cookie scheme
- **Choice:** Expose dedicated login/access-denied endpoints per audience (`/Admin/Account/Login`, `/Client/Account/Login`). Retain a single `CookieAuthenticationDefaults.AuthenticationScheme`; implement audience routing via `OnRedirectToLogin` and `OnRedirectToAccessDenied` event handlers that inspect the request path prefix (`/Admin/` → admin login, `/Client/` → client login) and redirect accordingly.
- **Rationale:** One shared login form with post-auth branching is less explicit and harder to secure by intent — a client credential should never be silently redirected into the admin area. Two pages give clear UX boundaries and are easy to route-protect independently. A single cookie scheme avoids the sign-out complexity and state management overhead of two parallel schemes while still giving each area its own entry point.
- **Alternatives considered:**
  - Two cookie schemes (admin/client): enables true parallel sessions but requires separate scheme names on every `[Authorize]` attribute and complicates sign-out.
  - Single shared login page with post-login role branching: simpler initially but merges security concerns, harder to audit, and confusing when a user accidentally hits the wrong area.
  - No new policy, rely only on controller checks: weaker centralization and harder to audit.

### Decision: Client role is chosen at registration (Farmer or Provider)
- **Choice:** The client registration form presents a role selector with two options: `Farmer` and `Provider`. The selected role is assigned as the user's `RoleType` on the created `UserProfile`. Both roles qualify for client area access; no separate generic `Client` role is introduced.
- **Rationale:** Farmer and Provider have different semantic meaning in the domain (service consumer vs. service supplier) and surfacing that distinction at registration keeps identity data useful for downstream features (e.g., provider-facing area later). Introducing a generic `Client` role would be throwaway data that needs a later migration.
- **Alternatives considered:**
  - Dedicated `RoleType.Client` role: cleaner access-control boundary but loses domain context; would need another migration when a provider-area is introduced.
  - Post-registration role selection: extra step, worse UX, and defers required data collection.

### Decision: Booking management is client-limited lifecycle actions
- **Choice:** Client booking pages support viewing bookings, viewing details/status, and confirming completion when status allows.
- **Rationale:** Delivers the required client journey without expanding into admin/provider workflows.
- **Alternatives considered:**
  - Full booking state machine controls in client UI: over-scoped and error-prone.
  - Read-only booking history only: insufficient for completion confirmation requirement.

## Risks / Trade-offs

- **[Risk] Role ambiguity for multi-role users (Admin + non-admin profile)** → **Mitigation:** Authenticate via audience-specific endpoint and validate required role/profile before issuing principal for that audience.
- **[Risk] Redirect confusion between area-protected routes and login paths** → **Mitigation:** Define explicit access-denied/login routes per audience and test unauthenticated/unauthorized navigation for both areas.
- **[Risk] Functional pages may expose incomplete booking validation paths** → **Mitigation:** Use existing domain constraints and explicit server-side validation in POST actions; return model errors rather than silent fallback.
- **[Trade-off] Single cookie scheme limits simultaneous independent admin/client browser sessions** → **Mitigation:** Accept for now to reduce complexity; revisit if product requires true dual-session behavior.

## Migration Plan

1. Add OpenSpec artifacts and implement client area and auth split in web project.
2. Register client area route and client authorization policy while preserving existing admin routes.
3. Introduce separate account controllers/views per audience and update links/layouts accordingly.
4. Deploy behind normal release process; verify admin and client login, booking browse, and booking completion flows.
5. Rollback strategy: revert web-layer changes and route/policy additions (no data migration required).

## Resolved Questions

- **Client role at registration:** Registration presents a Farmer / Provider selector; selected role is assigned as `RoleType` on `UserProfile`. Both qualify for client area access. No generic `Client` role is introduced.
- **Profile management scope:** Initial client profile edit covers name and contact fields available on the existing `UserProfile` model. Extended fields (location, etc.) are deferred to a later change.
- **Concurrent admin + client sessions:** Not supported in this change. Single cookie scheme means one active session per browser. Revisit if product requires dual-session behavior; at that point introduce a second cookie scheme.
