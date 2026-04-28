## ADDED Requirements

### Requirement: Dashboard stats are computed in the database
The system SHALL compute all dashboard aggregate values (counts, sums, breakdowns) using database-level queries. The implementation MUST NOT load full entity collections into application memory for the purpose of aggregation. Scalar results (`CountAsync`, `SumAsync`) and grouped projections SHALL be used instead.

#### Scenario: Dashboard loads without full table scans
- **WHEN** an admin navigates to `/Admin/Dashboard`
- **THEN** the dashboard stats are produced by SQL aggregation queries, and no full rows of `AppUser`, `ServiceListing`, `Booking`, or `Payment` are loaded into memory solely for counting or summing purposes

#### Scenario: Recent bookings list remains a materialized query
- **WHEN** the dashboard loads the recent activity section
- **THEN** only the 10 most recent booking rows (with their required navigation properties) are fetched from the database
