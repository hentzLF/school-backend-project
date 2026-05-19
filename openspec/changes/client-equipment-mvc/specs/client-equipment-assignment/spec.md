## ADDED Requirements

### Requirement: Provider can assign equipment to a listing
The system SHALL provide an equipment assignment page at `/Client/Equipment/AssignToListing/{listingId}` showing a checkbox form of the provider's equipment inventory. Currently assigned equipment SHALL be pre-checked. The controller SHALL call `IEquipmentService.AssignToListingAsync` with the selected equipment IDs on form submission.

#### Scenario: Assign equipment to listing successfully
- **WHEN** an authenticated Provider selects equipment items on the assignment form and submits
- **THEN** the system replaces the listing's equipment assignments with the selected set and redirects to the listing details page at `/Client/MyListings/Details/{listingId}`

#### Scenario: Remove all equipment from listing
- **WHEN** an authenticated Provider submits the assignment form with no equipment selected
- **THEN** the system removes all equipment assignments from the listing and redirects to listing details

#### Scenario: Assignment page shows current assignments pre-checked
- **WHEN** an authenticated Provider navigates to `/Client/Equipment/AssignToListing/{listingId}` for a listing that already has equipment assigned
- **THEN** the form displays all provider equipment with currently assigned items pre-checked

#### Scenario: Assignment blocked for non-owned listing
- **WHEN** an authenticated Provider attempts to assign equipment to a listing they do not own
- **THEN** the system returns 404

### Requirement: Provider can view equipment assigned to a listing
The system SHALL display assigned equipment on the listing details page at `/Client/MyListings/Details/{id}`. The equipment section SHALL show equipment name, make, model, and condition for each assigned item. A "Manage Equipment" link SHALL navigate to the assignment page.

#### Scenario: View assigned equipment on listing details
- **WHEN** an authenticated Provider views details for a listing that has equipment assigned
- **THEN** the listing details page displays the assigned equipment items with name, make, model, and condition

#### Scenario: No equipment assigned to listing
- **WHEN** an authenticated Provider views details for a listing with no equipment assigned
- **THEN** the listing details page displays an empty-state message in the equipment section with a link to assign equipment

### Requirement: Client can see equipment on listing detail page
The system SHALL display assigned equipment on the public listing detail page at `/Client/Listings/Details/{id}` in a read-only section. The equipment section SHALL show equipment name, make, model, and condition for each assigned item.

#### Scenario: Client views listing with equipment
- **WHEN** any authenticated user views a listing detail page for a listing that has equipment assigned
- **THEN** the page displays the assigned equipment items with name, make, model, and condition in a read-only card

#### Scenario: Client views listing without equipment
- **WHEN** any authenticated user views a listing detail page for a listing with no equipment assigned
- **THEN** the equipment section is not rendered on the page
