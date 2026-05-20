## Why

The current system forces users to maintain separate profiles per role (Farmer, Provider) and choose one at login. This adds unnecessary complexity to the auth flow (profile selection step, session tokens, multiple profiles) and doesn't match the real-world model where a single agricultural business owner both provides services and hires others. Unifying roles onto the user directly simplifies the auth flow, reduces frontend complexity, and eliminates a confusing UX step.

## What Changes

- **BREAKING**: Remove the profile selection login flow — login always returns tokens immediately
- **BREAKING**: Remove `POST /api/auth/select-profile` endpoint and related DTOs (`ProfileSelectionResponse`, `ProfileSummary`, `SelectProfileRequest`)
- **BREAKING**: JWT access tokens carry multiple `role` claims instead of a single `role` claim; `profileId` claim remains (1:1 with user)
- Replace `ProfileRole` entity (linked to `UserProfile`) with `UserRole` entity (linked directly to `AppUser`)
- Make `UserProfile` a 1:1 relationship with `AppUser` (currently 1:N)
- Registration assigns both Farmer and Provider roles by default (Admin remains separately assignable)
- Update all authorization policies and role-gating to work with multi-role users
- Database migration to flatten profile-role data

## Capabilities

### New Capabilities

_None — this change simplifies existing capabilities._

### Modified Capabilities

- `auth-login`: Remove multi-profile login path and profile selection; login always returns tokens directly
- `auth-register`: Registration creates user with both Farmer+Provider roles by default; remove single-role assignment
- `auth-tokens`: Access tokens carry multiple `role` claims instead of single; `profileId` remains as 1:1 identifier
- `booking-authz`: Role-gating on status transitions checks user's role set instead of single role; ownership checks unchanged (still use profileId)
- `resource-ownership`: No behavioral change — profileId remains the ownership key (now 1:1 with user)
- `bll-dto-contracts`: `UserProfileDto` reflects updated role structure

## Impact

- **API contracts**: Login response shape changes (no more `ProfileSelectionResponse`), register request drops single `role` field, JWT claims change
- **Database**: New migration replacing `ProfileRoles` table with `UserRoles`, adding unique constraint on `UserProfiles.AppUserId`
- **Domain entities**: `ProfileRole` deleted, `UserRole` created, `AppUser`/`UserProfile` navigation properties updated
- **Auth services**: `AuthService` simplified (remove `SelectProfileAsync`), `TokenService` emits multiple role claims
- **Controllers**: Provider-specific controllers and booking status transitions updated for multi-role checks
- **Seed data**: Test users updated to reflect unified model
- **Frontend**: Must remove profile selection UI, update login flow handling
