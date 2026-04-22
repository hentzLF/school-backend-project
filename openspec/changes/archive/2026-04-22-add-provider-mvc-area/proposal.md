## Why

Providers (users with `RoleType.Provider`) currently have no self-service MVC surface — they can only manage listings and view booking activity through the Admin panel, which is not appropriate for end users. This change gives Providers their own management pages within the existing Client area so they can run their service business independently.

## What Changes

- Add a `Provider`-gated section to the Client area where authenticated Providers can create, view, edit, and delete their own `ServiceListing` records.
- Add a read-only bookings view per listing so Providers can see who has booked their services, booking status, and key booking metadata.
- Listings created by a Provider are scoped to that Provider's `UserProfile` — they cannot view or modify other Providers' listings through this surface.
- No booking status changes are in scope; Providers can only observe booking state.

## Capabilities

### New Capabilities

- `provider-listing-management`: Provider can create, edit, delete, and list their own `ServiceListing` records. Listing creation requires title, description, price-per-hectare, and category. Edit covers the same fields. Delete is allowed only when no active bookings exist (or with a confirmation guard). List shows only the authenticated Provider's own listings.
- `provider-booking-visibility`: Provider can view bookings made against their listings in read-only mode. Shows client name, booking status, area, total price, and creation date. Access is restricted to bookings on listings owned by the authenticated Provider.

### Modified Capabilities

*(none — no existing spec-level behavior changes)*

## Impact

- `AgriMarket.Web` — new controllers (`Provider/ListingsController`, `Provider/BookingsController`) and views under `Areas/Client/Views/Provider/` (or a sub-navigation within the existing Client area), new ViewModels.
- `AgriMarket.Domain` / `AgriMarket.DAL` — no schema changes; reuses existing `ServiceListing`, `Booking`, `ServiceCategory`, and `UserProfile` entities.
- Authorization — a new `ProviderOnly` policy (requires `RoleType.Provider`) applied to all Provider management actions.
- Client area navigation — Provider nav items (My Listings, My Listing Bookings) added conditionally for authenticated Provider users.
