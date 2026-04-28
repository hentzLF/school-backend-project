## Why

The API currently has no IDOR protection: any authenticated user can read, modify, or delete resources belonging to other users simply by guessing a resource ID. The JWT auth infrastructure was just wired up, making this the right moment to lock down ownership before the API is consumed by a frontend.

## What Changes

- **BREAKING** — `POST /bookings`: remove `ClientProfileId` from request body; derive from JWT `profileId` claim
- **BREAKING** — `POST /listings`: remove `UserProfileId` from request body; derive from JWT `profileId` claim
- **BREAKING** — `POST /reviews`: remove `ReviewerProfileId` from request body; derive from JWT `profileId` claim
- Add `[Authorize]` to all mutation endpoints: `POST`, `PUT`, `PATCH`, `DELETE` across `BookingsController`, `ListingsController`, `ReviewsController`
- `PUT /listings/{id}` and `DELETE /listings/{id}`: return `403 Forbidden` if caller's `profileId` ≠ `listing.UserProfileId`
- `POST /bookings/{id}/status`: enforce role-based transition rules — only the client (booking owner) or the provider (listing owner) may transition, limited to transitions legal for their role
- `GET /bookings` and `GET /bookings/{id}`: require auth; scope results to bookings where caller is the client or the provider of the listing
- `GET /users/{id}`: strip `Email` from response unless the caller owns that profile

## Capabilities

### New Capabilities
- `resource-ownership`: Verifying that the authenticated caller (via `profileId` JWT claim) owns a resource before allowing mutation; returning 403 on mismatch
- `booking-authz`: Role-aware booking access — creation scoped to caller, status transitions gated by client vs. provider role, reads scoped to involved parties
- `dto-ownership-derivation`: Removing owner-ID fields from create request DTOs and deriving them server-side from the JWT

### Modified Capabilities

(none — no existing specs)

## Impact

- `AgriMarket.Api/Controllers/BookingsController.cs`
- `AgriMarket.Api/Controllers/ListingsController.cs`
- `AgriMarket.Api/Controllers/ReviewsController.cs`
- `AgriMarket.Api/Controllers/UsersController.cs`
- `AgriMarket.Api/Dtos/Bookings/CreateBookingRequest.cs` — remove `ClientProfileId`
- `AgriMarket.Api/Dtos/ServiceListings/CreateListingRequest.cs` — remove `UserProfileId`
- `AgriMarket.Api/Dtos/Reviews/CreateReviewRequest.cs` — remove `ReviewerProfileId`
- All callers of these DTOs (frontend / tests) must stop sending the removed fields
