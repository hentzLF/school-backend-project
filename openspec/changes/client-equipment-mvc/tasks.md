## 1. ViewModels

- [ ] 1.1 Create `EquipmentListItemViewModel` in `Areas/Client/ViewModels/Equipment/` with properties: `Id`, `Name`, `Make`, `Model`, `ManufactureYear`, `HorsePower`, `Condition` (string), `Status` (string).
- [ ] 1.2 Create `EquipmentIndexViewModel` in `Areas/Client/ViewModels/Equipment/` containing a `List<EquipmentListItemViewModel>` for the equipment inventory list.
- [ ] 1.3 Create `EquipmentCreateViewModel` in `Areas/Client/ViewModels/Equipment/` with required fields `Name`, `Make`, `Condition` (enum) and optional fields `Model`, `ManufactureYear`, `HorsePower`, `Description`. Add data annotation validation matching `CreateEquipmentDto`.
- [ ] 1.4 Create `EquipmentEditViewModel` in `Areas/Client/ViewModels/Equipment/` with `Id` (hidden), `Name`, `Make`, `Condition` (enum), and optional fields `Model`, `ManufactureYear`, `HorsePower`, `Description`. Add data annotation validation matching `UpdateEquipmentDto`.
- [ ] 1.5 Create `EquipmentDeleteViewModel` in `Areas/Client/ViewModels/Equipment/` with `Id`, `Name`, `Make`, `Model` for the confirmation page.
- [ ] 1.6 Create `EquipmentAssignViewModel` in `Areas/Client/ViewModels/Equipment/` with `ListingId`, `ListingTitle`, `List<EquipmentAssignItemViewModel>` where each item has `EquipmentId`, `Name`, `Make`, `Model`, `Status` (string), `IsSelected` (bool).

## 2. Mappers

- [ ] 2.1 Create `EquipmentViewModelMapper` in `AgriMarket.Web/Mappers/` as a static class with extension methods:
  - `ToListItem(this EquipmentDto dto)` → `EquipmentListItemViewModel`
  - `ToEditViewModel(this EquipmentDto dto)` → `EquipmentEditViewModel`
  - `ToDeleteViewModel(this EquipmentDto dto)` → `EquipmentDeleteViewModel`
  - `ToCreateDto(this EquipmentCreateViewModel vm)` → `CreateEquipmentDto`
  - `ToUpdateDto(this EquipmentEditViewModel vm)` → `UpdateEquipmentDto`

## 3. Controller

- [ ] 3.1 Create `EquipmentController` in `Areas/Client/Controllers/` with `[Area("Client")]` and `[Authorize(Policy = "ProviderOnly")]`. Inject `IEquipmentService`, `IListingService`, `IStringLocalizer<SharedResource>`, and `UserManager<AppUser>`. Add a private `GetProviderProfileIdAsync()` helper resolving the authenticated user's `UserProfile.Id`.
- [ ] 3.2 Implement `EquipmentController.Index` (GET) — calls `IEquipmentService.GetByProviderAsync(profileId)`, maps results to `EquipmentIndexViewModel`, returns view.
- [ ] 3.3 Implement `EquipmentController.Create` (GET) — returns view with empty `EquipmentCreateViewModel` and populates condition enum select list.
- [ ] 3.4 Implement `EquipmentController.Create` (POST) — validates `ModelState`, maps `EquipmentCreateViewModel` to `CreateEquipmentDto` via mapper, calls `IEquipmentService.CreateAsync`, redirects to `Index` on success or redisplays form on validation failure.
- [ ] 3.5 Implement `EquipmentController.Edit` (GET) — calls `IEquipmentService.GetByIdAsync(profileId, id)`, returns 404 if null, maps to `EquipmentEditViewModel`, returns view with condition enum select list.
- [ ] 3.6 Implement `EquipmentController.Edit` (POST) — validates `ModelState`, maps to `UpdateEquipmentDto`, calls `IEquipmentService.UpdateAsync`, redirects to `Index` on success. Catches `BusinessRuleException` for ownership and returns 404.
- [ ] 3.7 Implement `EquipmentController.Delete` (GET) — calls `IEquipmentService.GetByIdAsync(profileId, id)`, returns 404 if null, maps to `EquipmentDeleteViewModel`, returns confirmation view.
- [ ] 3.8 Implement `EquipmentController.Delete` (POST) — calls `IEquipmentService.DeleteAsync(profileId, id)`, redirects to `Index`. Catches `BusinessRuleException` for ownership and returns 404.
- [ ] 3.9 Implement `EquipmentController.UpdateStatus` (POST) — accepts `id` and `EquipmentStatus`, calls `IEquipmentService.UpdateStatusAsync(profileId, id, status)`, redirects to `Index`. Catches `BusinessRuleException` for ownership and returns 404.

## 4. Views

- [ ] 4.1 Create `Areas/Client/Views/Equipment/Index.cshtml` — table/card list of equipment items with name, make, model, condition badge, status badge, and action links (Edit, Delete, status dropdown). Includes empty-state message with "Add Equipment" link when no items exist.
- [ ] 4.2 Create `Areas/Client/Views/Equipment/Create.cshtml` — form with validation summary, fields for name (required), make (required), model, manufacture year, horsepower, condition dropdown (required), and description textarea. Submit and Cancel buttons.
- [ ] 4.3 Create `Areas/Client/Views/Equipment/Edit.cshtml` — same fields as Create with current values populated. Hidden `Id` field. Submit and Cancel buttons.
- [ ] 4.4 Create `Areas/Client/Views/Equipment/Delete.cshtml` — confirmation page showing equipment name, make, and model. Warning text about listing assignment removal. Confirm Delete and Cancel buttons.
- [ ] 4.5 Create `Areas/Client/Views/Equipment/_EquipmentCard.cshtml` partial view — renders a list of equipment items (name, make, model, condition) as a Bootstrap card. Accepts a model with equipment items and an optional "manage" link URL.

## 5. Equipment Assignment

- [ ] 5.1 Implement `EquipmentController.AssignToListing` (GET) — accepts `listingId`, verifies listing ownership via `IListingService`, loads provider equipment via `IEquipmentService.GetByProviderAsync`, loads currently assigned equipment via `IEquipmentService.GetByListingAsync(listingId)`, builds `EquipmentAssignViewModel` with pre-checked items, returns view.
- [ ] 5.2 Implement `EquipmentController.AssignToListing` (POST) — accepts `listingId` and list of selected equipment IDs, calls `IEquipmentService.AssignToListingAsync(profileId, listingId, selectedIds)`, redirects to `/Client/MyListings/Details/{listingId}`.
- [ ] 5.3 Create `Areas/Client/Views/Equipment/AssignToListing.cshtml` — form showing listing title, checkbox list of provider equipment (name, make, model, status), with currently assigned items pre-checked. Submit ("Save Assignments") and Cancel buttons.

## 6. Update Existing Views

- [ ] 6.1 Update `Areas/Client/Views/MyListings/Details.cshtml` — add an equipment section (collapsible Bootstrap card) showing assigned equipment count. If equipment exists, render `_EquipmentCard` partial with a "Manage Equipment" link to `/Client/Equipment/AssignToListing/{listingId}`. If no equipment, show empty-state text with an "Assign Equipment" link.
- [ ] 6.2 Update `Areas/Client/Views/Listings/Details.cshtml` — add a read-only equipment section. If the listing has assigned equipment, render `_EquipmentCard` partial (no management links). If no equipment, do not render the section.
- [ ] 6.3 Update `Areas/Client/Views/Shared/_ClientLayout.cshtml` — add an "Equipment" nav link (`/Client/Equipment`) conditionally rendered for authenticated users with the `Provider` role, positioned after the "My Listings" link.
- [ ] 6.4 Update `MyListingDetailsViewModel` to include a `List<EquipmentListItemViewModel> AssignedEquipment` property and an `int AssignedEquipmentCount` property.
- [ ] 6.5 Update `ListingDetailsViewModel` (Client) to include a `List<EquipmentListItemViewModel> Equipment` property for client-facing display.
- [ ] 6.6 Update `MyListingsController.Details` action to load assigned equipment via `IEquipmentService.GetByListingAsync(listingId)` and populate the ViewModel.
- [ ] 6.7 Update `ListingsController.Details` action (Client) to load assigned equipment via `IEquipmentService.GetByListingAsync(listingId)` and populate the ViewModel.

## 7. Localization

- [ ] 7.1 Add English (EN) resx keys to `SharedResource.en.resx` for equipment labels: `Equipment_Title`, `Equipment_Name`, `Equipment_Make`, `Equipment_Model`, `Equipment_ManufactureYear`, `Equipment_HorsePower`, `Equipment_Condition`, `Equipment_Status`, `Equipment_Description`, `Equipment_Create`, `Equipment_Edit`, `Equipment_Delete`, `Equipment_DeleteWarning`, `Equipment_UpdateStatus`, `Equipment_AssignToListing`, `Equipment_ManageEquipment`, `Equipment_NoEquipment`, `Equipment_NoEquipmentAssigned`, `Equipment_AssignedCount`.
- [ ] 7.2 Add Estonian (ET) resx keys to `SharedResource.et.resx` with corresponding Estonian translations for all keys added in 7.1.
- [ ] 7.3 Add enum display name resx keys for `EquipmentCondition` values: `EquipmentCondition_New`, `EquipmentCondition_Excellent`, `EquipmentCondition_Good`, `EquipmentCondition_Fair`, `EquipmentCondition_Poor` in both EN and ET.
- [ ] 7.4 Add enum display name resx keys for `EquipmentStatus` values: `EquipmentStatus_Available`, `EquipmentStatus_InUse`, `EquipmentStatus_UnderMaintenance`, `EquipmentStatus_Retired` in both EN and ET.

## 8. Tests

- [ ] 8.1 Write unit tests for `EquipmentViewModelMapper` — verify all mapping methods correctly convert between DTOs and ViewModels, including null optional fields.
- [ ] 8.2 Write unit tests for `EquipmentController.Index` — verify it calls `GetByProviderAsync` and returns the correct ViewModel.
- [ ] 8.3 Write unit tests for `EquipmentController.Create` (POST) — verify valid input calls `CreateAsync` and redirects, invalid input redisplays form.
- [ ] 8.4 Write unit tests for `EquipmentController.Edit` (GET/POST) — verify ownership check (404 for non-owned), valid input calls `UpdateAsync`, invalid input redisplays form.
- [ ] 8.5 Write unit tests for `EquipmentController.Delete` (GET/POST) — verify ownership check (404 for non-owned), confirm calls `DeleteAsync` and redirects.
- [ ] 8.6 Write unit tests for `EquipmentController.UpdateStatus` — verify valid status calls `UpdateStatusAsync`, ownership enforced.
- [ ] 8.7 Write unit tests for `EquipmentController.AssignToListing` (GET/POST) — verify listing ownership check, equipment pre-selection, and `AssignToListingAsync` call with correct IDs.
