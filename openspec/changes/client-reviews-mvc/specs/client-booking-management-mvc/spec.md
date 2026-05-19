## MODIFIED Requirements

### Requirement: Client can view own booking details
The system SHALL provide booking details at `/Client/Bookings/Details/{id}` for bookings owned by the authenticated client profile. When the booking is in `ClientConfirmed` status and no review exists for the booking, the details page SHALL display a review creation form (rating selector 1-5 and optional comment field) that posts to `/Client/Reviews/Create`. When a review already exists for the booking, the details page SHALL display the review (rating, comment, date) with links to edit and delete the review.

#### Scenario: View booking details with review form
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for an owned booking in `ClientConfirmed` status that has no review
- **THEN** the system shows booking details and a review creation section with a rating selector and comment field

#### Scenario: View booking details with existing review
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for an owned booking that already has a review
- **THEN** the system shows booking details and the existing review with edit and delete action links

#### Scenario: View booking details for non-completed booking
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for an owned booking not in `ClientConfirmed` status
- **THEN** the system shows booking details without a review section
