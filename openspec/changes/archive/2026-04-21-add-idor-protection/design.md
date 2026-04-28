## Context

The API was built with CRUD scaffolding and JWT infrastructure but no ownership enforcement. All mutation endpoints are unauthenticated and unguarded. The JWT token embeds `sub` (AppUser.Id) and `profileId` (UserProfile.Id). Ownership in the domain is always at the profile level — `ServiceListing.UserProfileId`, `Booking.ClientProfileId`, `Review.ReviewerProfileId` — making `profileId` the single correct identity for ownership checks.

A user may hold multiple `UserProfile` records; the selected profile at login time is embedded in the token.

## Goals / Non-Goals

**Goals:**
- Prevent authenticated users from mutating resources they don't own
- Scope booking reads to the caller's involved bookings only
- Remove client-supplied owner IDs from create request bodies
- Enforce booking status transition rules by role (client vs provider)
- Strip PII (email) from user profile responses unless the caller owns the profile

**Non-Goals:**
- Admin/superuser bypass (out of scope for this change)
- Role-based permission beyond client/provider split for bookings
- Auditing or logging of forbidden access attempts
- Rate limiting or abuse prevention

## Decisions

### D1: Use `profileId` claim (not `sub`) for all ownership checks

`profileId` maps directly to the FK columns used for ownership. Using `AppUser.Id` (`sub`) would require an extra join to `UserProfiles` on every check. Since the token already carries the active `profileId`, no DB round-trip is needed.

**Alternatives considered:**
- Look up the user's profiles on each request — adds latency and complexity with no benefit at this stage.

### D2: Option A — inline ownership checks per action (no abstraction layer)

Each action reads `profileId` from `User.FindFirstValue("profileId")`, loads the resource, and compares FKs inline before proceeding. No custom `IAuthorizationHandler` or service layer.

**Alternatives considered:**
- `IAuthorizationHandler` with resource-based policies (Option B) — correct ASP.NET pattern but significant boilerplate for 5-6 endpoints at this stage. Introduce when the number of protected resources warrants it.
- Service layer owns the check (Option C) — preferred long-term but there is no service layer yet; introducing one is out of scope.

### D3: Booking status transitions gated by role, not just ownership

`PATCH /bookings/{id}/status` loads both `booking.ClientProfileId` and `booking.ServiceListing.UserProfileId`. The caller is identified as **client** or **provider** and the allowed transition set differs:

| Role     | Allowed transitions                                                  |
|----------|----------------------------------------------------------------------|
| Client   | Pending → Cancelled, Confirmed → Cancelled, ProviderCompleted → ClientConfirmed |
| Provider | Pending → Confirmed, Pending → Cancelled, Confirmed → InProgress, InProgress → ProviderCompleted, any → Disputed |

If the caller is neither party, return 403. If the transition is illegal for the role, return 422.

### D4: `GET /bookings` scoped, `GET /bookings/{id}` guarded

`GetAll` filters by `ClientProfileId == callerProfile OR ServiceListing.UserProfileId == callerProfile`.  
`GetById` returns 403 (not 404) if the booking exists but the caller is not involved — to avoid leaking existence.

### D5: `GET /users/{id}` — strip email unless owner

The `Email` field is included in the response only when `profile.AppUserId == callerUserId` (using the `sub` claim). Unauthenticated callers never see email.

## Risks / Trade-offs

- **BREAKING request contracts** — Three create-DTOs lose their owner-ID field. Any existing frontend or test client must be updated. → Mitigation: clearly document in the PR; this is intentional hardening.
- **403 vs 404 on guarded reads** — Returning 403 on `GET /bookings/{id}` leaks resource existence to a non-party. We accept this; returning 404 would be more private but complicates debugging. Can be revisited.
- **No service layer** — Inline checks duplicate the `profileId` extraction pattern across controllers. Accept for now; factor out when the service layer is introduced.

## Migration Plan

1. Update DTOs (remove owner-ID fields from three create requests)
2. Add `[Authorize]` and inline ownership checks to controllers
3. Existing seeded data is unaffected (no schema change)
4. Any integration tests or API clients must remove the dropped fields from create request bodies
