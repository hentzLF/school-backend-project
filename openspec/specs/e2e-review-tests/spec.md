## Purpose

E2E tests for review CRUD and rating display on listings.

## Requirements

### Requirement: Review creation test
The test suite SHALL verify that a farmer can create a review for a completed booking.

#### Scenario: Successful review creation
- **WHEN** a farmer navigates to `/Client/Reviews/Create/{bookingId}` for a completed booking, enters a rating (1-5) and comment, and submits
- **THEN** the review is created and visible on the listing's detail page

#### Scenario: Review with invalid rating
- **WHEN** a farmer submits a review with rating outside 1-5 range
- **THEN** a validation error is displayed

### Requirement: Review edit test
The test suite SHALL verify review editing.

#### Scenario: Successful review edit
- **WHEN** a farmer edits their existing review (changes rating and comment) and saves
- **THEN** the updated review is visible on the listing detail page

### Requirement: Review deletion test
The test suite SHALL verify review deletion.

#### Scenario: Successful review deletion
- **WHEN** a farmer confirms deletion of their review
- **THEN** the review is removed from the listing detail page

### Requirement: Rating display test
The test suite SHALL verify that ratings are correctly displayed on listings.

#### Scenario: Average rating updates after review
- **WHEN** a review is created for a listing
- **THEN** the listing's average rating and review count update on the listing index and detail pages
