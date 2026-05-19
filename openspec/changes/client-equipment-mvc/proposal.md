## Why

Providers cannot manage their equipment inventory through the MVC web interface despite `IEquipmentService` being fully implemented in the BLL with complete CRUD, status management, and listing assignment capabilities. Equipment is a core domain concept that providers need to create, view, edit, delete, and assign to their service listings. Currently the only equipment management surface is the REST API (`ProviderEquipmentController`), which is not accessible from the provider's web dashboard.

## What Changes

- Add a new `EquipmentController` in the Client area (`ProviderOnly` policy) providing full equipment inventory CRUD at `/Client/Equipment`.
- Add equipment CRUD views: Index (inventory list), Create (form), Edit (form), Delete (confirmation).
- Add equipment assignment management: providers can assign/unassign equipment to their listings from the listing details page.
- Update `MyListings/Details` view to display assigned equipment with a link to manage assignments.
- Update client-facing `Listings/Details` view to show equipment assigned to a listing (read-only for farmers/clients).
- Add `Equipment` nav link to `_ClientLayout` for authenticated providers.

## Capabilities

### New Capabilities

- `client-equipment-crud`: Provider can create, view, edit, delete, and update the status of their own equipment items. Equipment fields include name, make, model, manufacture year, horsepower, condition, and description. Status transitions (Available, InUse, UnderMaintenance, Retired) are managed from the equipment detail or index page.
- `client-equipment-assignment`: Provider can assign equipment to their own service listings and view which equipment is currently assigned to a listing. Clients/farmers can see equipment listed on a service listing's public detail page.

### Modified Capabilities

- `provider-listing-management`: MyListings Details view now shows an assigned equipment section with a count and links to manage equipment assignments for that listing.

## Impact

- `AgriMarket.Web/Areas/Client/Controllers/` — new `EquipmentController` with full CRUD and status actions.
- `AgriMarket.Web/Areas/Client/ViewModels/Equipment/` — new ViewModels for equipment index, create, edit, delete, and assignment.
- `AgriMarket.Web/Areas/Client/Views/Equipment/` — new Razor views for all equipment actions.
- `AgriMarket.Web/Mappers/` — new `EquipmentViewModelMapper` for DTO-to-ViewModel and ViewModel-to-DTO conversions.
- `AgriMarket.Web/Areas/Client/Views/MyListings/Details.cshtml` — equipment section added.
- `AgriMarket.Web/Areas/Client/Views/Listings/Details.cshtml` — equipment display added for clients.
- `AgriMarket.Web/Areas/Client/Views/Shared/_ClientLayout.cshtml` — `Equipment` nav link for providers.
- Localization `.resx` files — new keys for equipment-related labels and messages (EN + ET).
- `AgriMarket.Domain` / `AgriMarket.DAL` — no schema or migration changes; reuses existing `Equipment`, `ServiceListingEquipment`, and enum types.
