## Context

`AgriMarket.Web` has a Client MVC area serving both `Farmer` and `Provider` roles, but it only exposes consumer-facing pages (browse listings, create bookings, manage own bookings). Providers have no self-service surface to manage their own `ServiceListing` records — they must rely on admin intervention. The `ServiceListing` entity already carries a `UserProfileId` foreign key that ties a listing to its owning provider's profile, so ownership enforcement requires no schema changes.

Constraints:
- Follow the same area/controller/viewmodel/view conventions established in the Client and Admin areas.
- All provider management actions must be scoped strictly to listings owned by the authenticated Provider's `UserProfile`.
- Reuse existing `ServiceCategory`, `ServiceListing`, and `Booking` entities without schema changes.
- The existing public listing browse (`ListingsController`) remains unchanged and accessible to all users.

## Goals / Non-Goals

**Goals:**
- Give Providers a self-service MVC surface to create, view, edit, delete, and toggle activation of their own listings.
- Allow Providers to view bookings made against their own listings (read-only).
- Enforce ownership: a Provider cannot view, edit, or delete another Provider's listings or see their bookings.
- Register a `ProviderOnly` authorization policy (`RoleType.Provider`) and apply it to all provider management actions.

**Non-Goals:**
- Availability slot management (adding/removing availability windows on a listing) — deferred.
- Provider-side booking status transitions (e.g., marking `ProviderCompleted`) — deferred.
- Equipment management on listings — deferred.
- Separate provider area route; provider management lives as additional controllers inside the existing Client area.

## Decisions

### Decision: Provider management controllers live inside the existing Client area
- **Choice:** Add `Areas/Client/Controllers/Provider/ListingsController` and `Areas/Client/Controllers/Provider/BookingsController` (or top-level `MyListingsController` / `MyListingBookingsController`) rather than a separate `Areas/Provider` area.
- **Rationale:** A Provider is also a Client-area user (they can browse and book services too). A separate area would force a second login and double the layout/auth setup. Keeping them in the Client area with a `ProviderOnly` policy on management actions is simpler and consistent with the existing `ClientOnly` policy pattern.
- **Alternatives considered:**
  - Separate `Areas/Provider` area with its own layout and route: stronger separation, but requires duplicate auth setup and a third login endpoint.
  - Single shared controller with role-branching inside actions: harder to authorize cleanly and violates single-responsibility.

### Decision: `ProviderOnly` authorization policy
- **Choice:** Register a new `ProviderOnly` policy that requires `RoleType.Provider` (the claim is already issued at login). Apply `[Authorize(Policy = "ProviderOnly")]` to all provider management actions.
- **Rationale:** Consistent with the existing `AdminOnly` and `ClientOnly` policy pattern. Centralizes role enforcement at the policy level rather than sprinkling role checks inside action bodies.
- **Alternatives considered:**
  - `[Authorize(Roles = "Provider")]` attribute directly: works but bypasses the centralized policy system already established.

### Decision: Ownership enforced by filtering on `UserProfileId`, not by claim alone
- **Choice:** On every read/write action, resolve the authenticated user's `UserProfile.Id` from the database (using `ClaimTypes.NameIdentifier` → `AppUser.Id` → `UserProfile.Id`) and filter all queries by `UserProfileId == clientProfile.Id`. For non-index actions, return 404 if the listing does not exist or does not belong to the authenticated provider.
- **Rationale:** Returning 404 (rather than 403) avoids leaking the existence of other providers' listings. Consistent with the ownership pattern used in `BookingsController` in the Client area.
- **Alternatives considered:**
  - Store `UserProfileId` in a claim at login and filter by claim value: avoids one DB round-trip but adds stale-data risk if profile ID changes.

### Decision: Delete guarded by active-bookings check
- **Choice:** Before deleting a listing, check whether any `Booking` records reference it with a non-terminal status (anything except `Archived`, `Cancelled`, `ClientConfirmed`). If active bookings exist, reject the delete and return a validation error.
- **Rationale:** Prevents data integrity issues where deleting a listing would orphan in-flight bookings. Admin hard-delete remains available without this guard.
- **Alternatives considered:**
  - Soft-delete (set `IsActive = false`): less destructive but leaves stale data. Provider can already deactivate via toggle; delete should be a true removal.

## Risks / Trade-offs

- **[Risk] Provider accidentally deactivates a listing with pending bookings** → **Mitigation:** Deactivation (toggle) is allowed freely; warn on the confirmation page that existing bookings are unaffected by deactivation status.
- **[Risk] Ownership check adds a DB round-trip per action** → **Mitigation:** Acceptable for a low-traffic MVC surface; profile lookup can be consolidated into a shared private helper per controller.
- **[Trade-off] No separate Provider area means Provider and Farmer share a layout and nav** → **Mitigation:** Client layout conditionally renders provider nav items (`My Listings`) only for authenticated users with `RoleType.Provider`.
