## MODIFIED Requirements

### Requirement: MyListings Details view shows assigned equipment
The system SHALL display an assigned equipment section on the `MyListings/Details` page. The section SHALL show a count of assigned equipment items, a summary list with name, make, and condition, and a "Manage Equipment" link navigating to `/Client/Equipment/AssignToListing/{listingId}`.

#### Scenario: Listing has assigned equipment
- **WHEN** an authenticated Provider views `/Client/MyListings/Details/{id}` for a listing with assigned equipment
- **THEN** the details page displays an equipment section showing the count of assigned items, each item's name, make, and condition, and a "Manage Equipment" link

#### Scenario: Listing has no assigned equipment
- **WHEN** an authenticated Provider views `/Client/MyListings/Details/{id}` for a listing with no assigned equipment
- **THEN** the details page displays an equipment section with a message indicating no equipment is assigned and a link to assign equipment
