## Context

The current system models user roles through a multi-profile architecture: `AppUser (1) → (N) UserProfile → (N) ProfileRole`. A user with both Farmer and Provider roles has two separate `UserProfile` records, each with its own `ProfileRole`. At login, users with multiple profiles must select one via a two-step flow (session token → profile selection → access token). The JWT access token locks in a single `profileId` and `role`.

This creates unnecessary complexity: the profile selection UI/API, session tokens, and the assumption that a user acts as only one role at a time. In practice, an agricultural business owner both provides services and hires others simultaneously.

All domain resources (listings, bookings, reviews, equipment, conversations) are linked to `UserProfile.Id` via foreign keys. This relationship must be preserved during migration.

## Goals / Non-Goals

**Goals:**
- Flatten role assignment: roles belong directly to `AppUser`, not to profiles
- Make `UserProfile` a 1:1 relationship with `AppUser` (one profile per user)
- Simplify login to always return tokens immediately (no profile selection step)
- JWT tokens carry all user roles as multiple `role` claims
- Preserve all existing resource ownership (FK references to `UserProfile.Id` stay intact)

**Non-Goals:**
- Merging `UserProfile` into `AppUser` — keeping them separate preserves the clean separation between auth identity and public profile data
- Changing the Admin role behavior — Admin remains a separately-assigned role
- Modifying resource ownership checks — `profileId` in JWT and FK references to `UserProfile.Id` remain unchanged
- Frontend/client changes — only backend API contracts change

## Decisions

### 1. Roles move to AppUser, not UserProfile

**Decision:** Create `UserRole` entity with FK to `AppUser.Id`, replacing `ProfileRole` (FK to `UserProfile.Id`).

**Rationale:** Roles are an identity-level concern (authentication/authorization), not a profile-level concern. Linking roles to `AppUser` aligns with the principle that authorization derives from identity. The alternative — keeping roles on `UserProfile` but enforcing 1:1 — would leave a misleading indirection.

### 2. UserProfile becomes 1:1 with AppUser

**Decision:** Add a unique index on `UserProfiles.AppUserId` and update `AppUser.Profiles` navigation from `ICollection<UserProfile>` to a single `UserProfile? Profile`.

**Rationale:** With roles decoupled from profiles, there is no reason for multiple profiles per user. The 1:1 constraint prevents future drift back to multi-profile patterns. Domain resources continue to reference `UserProfile.Id` unchanged.

### 3. Migration merges existing multi-profile data

**Decision:** The EF migration will use SQL to consolidate multi-profile users. For users with multiple profiles, keep the first profile (ordered by Id) and re-assign any resources from secondary profiles to the primary one. Then create `UserRoles` from the union of all `ProfileRoles` for that user.

**Rationale:** This preserves all resource ownership while collapsing to one profile per user. The alternative (failing migration on multi-profile users) would block deployment in environments with existing test data.

### 4. JWT carries multiple role claims

**Decision:** `TokenService.GenerateAccessToken` accepts `IEnumerable<RoleType>` and emits one `role` claim per role. ASP.NET Core's `[Authorize(Roles = "...")]` and policy-based auth natively support multiple role claims.

**Rationale:** This is the standard ASP.NET Core pattern. The alternative (comma-separated single claim) would require custom authorization handlers.

### 5. Registration assigns Farmer + Provider by default

**Decision:** `RegisterAsync` no longer accepts a role field at all. Every registered user automatically receives both Farmer and Provider roles. Admin remains non-self-assignable and can only be granted directly in the database.

**Rationale:** Since every regular user can act as both farmer and provider, there is no meaningful role choice at registration. Removing the field entirely (rather than making it optional with a default) eliminates an unnecessary API surface and prevents clients from accidentally creating single-role users.

### 6. Remove session token and profile selection

**Decision:** Delete `GenerateSessionToken`, `ValidateSessionToken`, `SelectProfileAsync`, and all related DTOs (`ProfileSelectionResponse`, `ProfileSummary`, `SelectProfileRequest`). Login always returns `TokenResponse` directly.

**Rationale:** With one profile per user and all roles in the token, there is no selection to make. The session token mechanism exists solely for the profile selection flow.

## Risks / Trade-offs

- **Breaking API change** → Document in API changelog; coordinate with frontend. The login response shape changes and `POST /api/auth/select-profile` is removed.
- **Data migration complexity** → Multi-profile users need resource consolidation. Mitigated by writing explicit SQL in the migration that handles the merge deterministically.
- **Existing JWT tokens invalidated** → Users will need to re-login after deployment. Mitigated by the short (15 min) access token expiry; only refresh tokens are affected, and those can be bulk-revoked.
- **ProviderOnly policy semantics** → Users with both roles will pass `ProviderOnly` checks even when acting as a farmer. This is intentional — the unified model means all users have provider capabilities.

## Open Questions

None — all technical decisions are resolved. The migration strategy, JWT claim format, and registration behavior are defined above.
