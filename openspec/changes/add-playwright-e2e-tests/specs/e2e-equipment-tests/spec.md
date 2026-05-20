## ADDED Requirements

### Requirement: Equipment list test
The test suite SHALL verify the provider's equipment list at `/Client/Equipment`.

#### Scenario: Equipment is listed
- **WHEN** a provider navigates to `/Client/Equipment`
- **THEN** the page displays the provider's equipment items with name, make, model, and status

### Requirement: Equipment creation test
The test suite SHALL verify equipment creation.

#### Scenario: Successful equipment creation
- **WHEN** a provider fills the create form with name, make, model, manufacture year, horsepower, and condition and submits
- **THEN** the equipment appears in the equipment list

#### Scenario: Create equipment with missing name
- **WHEN** a provider submits the create form with empty name
- **THEN** validation errors are displayed

### Requirement: Equipment edit test
The test suite SHALL verify equipment editing.

#### Scenario: Successful equipment edit
- **WHEN** a provider changes the name of an existing equipment and saves
- **THEN** the updated name is visible in the equipment list

### Requirement: Equipment deletion test
The test suite SHALL verify equipment deletion.

#### Scenario: Delete unassigned equipment
- **WHEN** a provider confirms deletion of equipment not assigned to any listing
- **THEN** the equipment is removed from the list

### Requirement: Equipment status change test
The test suite SHALL verify equipment status updates.

#### Scenario: Change equipment status
- **WHEN** a provider changes an equipment's status (e.g., Active to Maintenance)
- **THEN** the updated status is displayed in the equipment list

### Requirement: Equipment listing assignment test
The test suite SHALL verify assigning equipment to listings.

#### Scenario: Assign equipment to listing
- **WHEN** a provider navigates to the assign page, selects a listing, and submits
- **THEN** the equipment appears on the selected listing's detail page

#### Scenario: Unassign equipment from listing
- **WHEN** a provider removes an equipment assignment from a listing
- **THEN** the equipment no longer appears on that listing's detail page
