## Why

Farmers can browse and view service listings but cannot book them because the `Availability` table is always empty — providers have no UI to add time slots to their listings. The booking form in `Details.cshtml` only renders when `Availabilities.Any()`, so the fix is giving providers a way to manage their availability slots.

## What Changes

- New **Manage Availability Slots** page for providers: lists existing slots on a listing, with an inline add form and per-slot delete
- `MyListingsController` gains three new actions: `Availabilities` (GET), `AddAvailability` (POST), `DeleteAvailability` (POST)
- `MyListings/Details.cshtml` gets a "Manage Slots" link leading to the new page
- New ViewModels: `ManageAvailabilitiesViewModel`, `AvailabilityItemViewModel`, `AddAvailabilityViewModel`

## Capabilities

### New Capabilities

- `provider-availability-management`: Provider can view, add, and delete availability time slots for their own listings. Adding requires StartTime < EndTime. Deleting is only permitted for unbooked slots (IsBooked = false).

### Modified Capabilities

_(none — farmer-facing booking flow is unchanged)_

## Impact

- `AgriMarket.Web/Areas/Client/Controllers/MyListingsController.cs` — new actions
- `AgriMarket.Web/Areas/Client/Views/MyListings/Details.cshtml` — add "Manage Slots" link
- New view: `AgriMarket.Web/Areas/Client/Views/MyListings/Availabilities.cshtml`
- New ViewModels under `AgriMarket.Web/Areas/Client/ViewModels/MyListings/`
- `AgriMarket.Domain/Entities/Availability.cs` — read-only; no entity changes
- `AgriMarket.DAL/AppDbContext.cs` — `Availabilities` DbSet already exists; no migration needed
