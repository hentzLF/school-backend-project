## Context

The `Availability` entity and `AppDbContext.Availabilities` DbSet already exist. `MyListingsController` is the Provider's listing management hub (CRUD + toggle-active + view-bookings). The farmer-facing `ListingsController.Details` already renders available slots and the booking form when `Availabilities.Any()` — no changes needed there.

No new migrations are required; the `Availabilities` table and its FK to `ServiceListings` are already in place.

## Goals / Non-Goals

**Goals:**
- Providers can view all availability slots for a listing they own
- Providers can add a new slot (StartTime, EndTime) to an owned listing
- Providers can delete an unbooked slot from an owned listing
- Slot management is surfaced from the existing `MyListings/Details` page

**Non-Goals:**
- Editing an existing slot (delete + re-add is sufficient)
- Bulk slot creation or recurrence patterns
- Any changes to the farmer-facing booking flow
- API-layer availability endpoints (web MVC only)

## Decisions

**Single page for list + add form**: The `Availabilities` GET action renders the slot list and an inline add form on the same page. Avoids a separate Create page for such a simple form.

**POST-only for add and delete**: `AddAvailability` and `DeleteAvailability` are POST actions protected by `[ValidateAntiForgeryToken]`. No GET needed for delete (confirmation is inline).

**Ownership enforced via `GetProviderProfileAsync()`**: Reuses the existing helper — all actions 404 if the listing is not owned by the authenticated provider.

**Delete guard on `IsBooked`**: If a slot has `IsBooked = true`, the delete POST returns a 400/redirect with an error. The UI hides the delete button for booked slots as a first-level guard.

**`IsActive` not required for slot management**: Providers may want to pre-load slots before activating the listing. No active-status guard on availability CRUD.

## Risks / Trade-offs

- **Concurrent booking race**: Two farmers could attempt to book the same slot simultaneously. The existing `Book` action already queries `!a.IsBooked` and marks it booked atomically in the same `SaveChangesAsync` call — no additional locking needed for this scope.
- **No overlap validation**: Overlapping slots are allowed (two different farmers could book overlapping windows for the same provider). Out of scope for this change; could be added later.
