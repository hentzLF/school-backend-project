## Context

`IEquipmentService` in the BLL layer is fully implemented with CRUD operations, status management, and listing assignment. The REST API exposes this through `ProviderEquipmentController`, but the MVC Client area has no equipment management surface. Providers using the web dashboard cannot manage their equipment inventory or assign equipment to their service listings.

The Client area already follows established patterns: `MyListingsController` handles provider listing CRUD with `[Authorize(Policy = "ProviderOnly")]`, ViewModels live under `Areas/Client/ViewModels/`, and mappers live in `AgriMarket.Web/Mappers/`. The `IEquipmentService` interface accepts a `profileId` (resolved from the authenticated user) for all ownership-scoped operations.

Constraints:
- Follow existing Client area conventions (controllers, ViewModels, mappers, views).
- All equipment actions must be scoped to the authenticated Provider's `UserProfile` via `IEquipmentService`.
- Reuse existing `IEquipmentService` methods — no new BLL methods needed.
- No schema or migration changes — `Equipment` and `ServiceListingEquipment` tables already exist.

## Goals / Non-Goals

**Goals:**
- Give Providers an MVC surface to create, view, edit, delete, and manage status of their own equipment inventory.
- Allow Providers to assign equipment to their own listings from the listing details page.
- Display assigned equipment on the client-facing listing detail page (read-only for farmers).
- Follow the same controller/ViewModel/mapper/view patterns as `MyListingsController`.

**Non-Goals:**
- Equipment search or filtering on the index page — keep it a simple list for now.
- Equipment photo uploads — deferred to a future change.
- Equipment sharing between providers — each equipment item belongs to exactly one provider.
- Equipment calendar/scheduling — equipment availability is managed solely through status.
- Equipment analytics or usage tracking.

## Decisions

### Decision: Dedicated EquipmentController in Client area
- **Choice:** Create a new `EquipmentController` at `Areas/Client/Controllers/EquipmentController.cs` with `[Authorize(Policy = "ProviderOnly")]` rather than adding equipment actions to `MyListingsController`.
- **Rationale:** Equipment management is a separate domain concern from listing management. A dedicated controller keeps single-responsibility and avoids bloating `MyListingsController` further. Routes at `/Client/Equipment/` are intuitive and parallel `/Client/MyListings/`.
- **Alternatives considered:**
  - Add equipment actions to `MyListingsController`: simpler initial setup but violates SRP and makes the controller too large.
  - Create a sub-controller under `MyListings`: ASP.NET Core MVC does not support sub-controllers natively; would require awkward routing.

### Decision: Equipment assignment managed from EquipmentController
- **Choice:** Add `AssignToListing` (GET/POST) actions on `EquipmentController` rather than on `MyListingsController`. The GET action shows a checkbox form listing the provider's equipment with pre-selected items for currently assigned equipment. The POST action calls `IEquipmentService.AssignToListingAsync`.
- **Rationale:** Assignment is an equipment-centric operation (selecting which equipment to assign). The listing context (which listing to assign to) is passed as a parameter. A link from `MyListings/Details` navigates to `/Client/Equipment/AssignToListing/{listingId}`.
- **Alternatives considered:**
  - Add assignment actions to `MyListingsController`: reasonable but keeps all equipment logic in one controller.
  - Modal/AJAX approach: more complex, deferred to future UX enhancement.

### Decision: Equipment display on client listing details uses partial view
- **Choice:** Create an `_EquipmentCard` partial view that renders equipment items, used in both `MyListings/Details` (provider view) and `Listings/Details` (client view).
- **Rationale:** Avoids duplicating equipment rendering markup. Both views need to show equipment but with different surrounding context (provider has management links, client has read-only view).
- **Alternatives considered:**
  - Inline HTML in each view: duplication and maintenance burden.
  - View component: heavier abstraction than needed for simple display.

### Decision: EquipmentViewModelMapper follows existing mapper pattern
- **Choice:** Create `AgriMarket.Web/Mappers/EquipmentViewModelMapper.cs` as a static class with extension methods, matching the pattern of `ListingViewModelMapper`.
- **Rationale:** Consistency with existing codebase. Extension methods on DTOs provide fluent mapping syntax.

## Risks / Trade-offs

- **[Risk] MyListings/Details view gets more complex with equipment section** — **Mitigation:** Keep the equipment section as a collapsible card using Bootstrap accordion. Use a partial view to encapsulate the markup.
- **[Risk] Assignment page could be confusing if provider has many equipment items** — **Mitigation:** Show equipment name, make, model, and status on each checkbox row so the provider can identify items easily. Pagination deferred until needed.
- **[Trade-off] Equipment assignment replaces the full list rather than adding/removing individually** — **Mitigation:** `IEquipmentService.AssignToListingAsync` already implements full-replacement semantics. The checkbox form naturally represents the full set. This is simpler than incremental add/remove.
- **[Trade-off] No equipment filtering or search on index** — **Mitigation:** Acceptable for initial implementation; most providers will have a manageable number of equipment items. Can add filtering in a future iteration.
