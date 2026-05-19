## ADDED Requirements

### Requirement: Provider can view equipment inventory
The system SHALL provide an equipment inventory index at `/Client/Equipment` that displays only the `Equipment` items owned by the authenticated Provider's `UserProfile`. Each item SHALL show name, make, model, condition, and status.

#### Scenario: View own equipment inventory
- **WHEN** an authenticated Provider navigates to `/Client/Equipment`
- **THEN** the system displays only equipment whose `UserProfileId` matches the authenticated Provider's profile, showing name, make, model, condition badge, and status badge for each item

#### Scenario: No equipment for provider
- **WHEN** an authenticated Provider navigates to `/Client/Equipment` and owns no equipment
- **THEN** the system displays an empty-state message and a prompt to add the first equipment item

#### Scenario: Non-provider access denied
- **WHEN** a user without the `Provider` role navigates to `/Client/Equipment`
- **THEN** the system denies access via the `ProviderOnly` authorization policy

### Requirement: Provider can create new equipment
The system SHALL allow an authenticated Provider to create a new equipment item via a form at `/Client/Equipment/Create`. Required fields are name, make, and condition. Optional fields are model, manufacture year, horsepower, and description. The controller SHALL map the ViewModel to `CreateEquipmentDto` and call `IEquipmentService.CreateAsync`.

#### Scenario: Create equipment successfully
- **WHEN** an authenticated Provider submits valid equipment data (name, make, condition, and optional fields)
- **THEN** the system creates an equipment item owned by the Provider's profile with `Status = Available`, and redirects to the equipment index

#### Scenario: Invalid equipment input rejected
- **WHEN** an authenticated Provider submits equipment data with missing required fields or invalid values (e.g., year out of range, negative horsepower)
- **THEN** the system redisplays the create form with validation errors and does not call the BLL service

### Requirement: Provider can edit own equipment
The system SHALL allow an authenticated Provider to edit their own equipment via a form at `/Client/Equipment/Edit/{id}`. Editable fields are name, make, model, manufacture year, horsepower, condition, and description. The controller SHALL map the ViewModel to `UpdateEquipmentDto` and call `IEquipmentService.UpdateAsync`.

#### Scenario: Edit equipment successfully
- **WHEN** an authenticated Provider submits valid changes to an equipment item they own
- **THEN** the system persists the changes via the BLL service and redirects to the equipment index

#### Scenario: Edit rejected for invalid input
- **WHEN** an authenticated Provider submits invalid data for an equipment item
- **THEN** the system redisplays the edit form with validation errors and does not persist changes

#### Scenario: Edit blocked for non-owned equipment
- **WHEN** an authenticated Provider attempts to edit an equipment item they do not own
- **THEN** the BLL service enforces ownership and the controller returns 404

### Requirement: Provider can delete own equipment
The system SHALL allow an authenticated Provider to delete their own equipment via a confirmation page at `/Client/Equipment/Delete/{id}`. The confirmation page SHALL warn that deleting equipment removes it from all listing assignments. The controller SHALL call `IEquipmentService.DeleteAsync`.

#### Scenario: Delete equipment successfully
- **WHEN** an authenticated Provider confirms deletion of an equipment item they own
- **THEN** the system removes the equipment item and all its listing assignments, then redirects to the equipment index

#### Scenario: Delete blocked for non-owned equipment
- **WHEN** an authenticated Provider attempts to delete an equipment item they do not own
- **THEN** the BLL service enforces ownership and the controller returns 404

#### Scenario: Delete confirmation warns about listing assignments
- **WHEN** an authenticated Provider navigates to `/Client/Equipment/Delete/{id}` for an equipment item they own
- **THEN** the confirmation page displays the equipment name and a warning that deletion will remove the item from all listing assignments

### Requirement: Provider can update equipment status
The system SHALL allow an authenticated Provider to change the status of their own equipment (Available, InUse, UnderMaintenance, Retired) via a POST action at `/Client/Equipment/UpdateStatus/{id}`. The controller SHALL call `IEquipmentService.UpdateStatusAsync`.

#### Scenario: Update status successfully
- **WHEN** an authenticated Provider submits a valid status change for an equipment item they own
- **THEN** the system updates the equipment status and redirects to the equipment index

#### Scenario: Update status blocked for non-owned equipment
- **WHEN** an authenticated Provider attempts to update the status of an equipment item they do not own
- **THEN** the BLL service enforces ownership and the controller returns 404

### Requirement: Web controllers do not construct domain entities
No controller in `AgriMarket.Web` SHALL directly instantiate `Equipment` or `ServiceListingEquipment` entities. Entity construction SHALL be delegated to BLL services via DTOs.

#### Scenario: No entity construction in equipment controller
- **WHEN** the Web project is compiled
- **THEN** no controller file contains `new Equipment(` or `new ServiceListingEquipment(`
