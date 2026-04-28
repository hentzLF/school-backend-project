# Spec: Admin Dashboard

## Purpose
Defines the platform-wide statistics and overview page shown to admins upon login, covering user, listing, booking, revenue, dispute, and recent activity summaries.

## Requirements

### Requirement: Dashboard overview page
The system SHALL provide a dashboard at `/Admin/Dashboard` displaying platform-wide statistics via a `DashboardViewModel`.

#### Scenario: Dashboard loads with statistics
- **WHEN** an admin navigates to `/Admin/Dashboard`
- **THEN** the system displays the dashboard with all statistic cards and summaries

### Requirement: User statistics
The dashboard SHALL display: total user count, new users this month, and new users this week. The system SHALL use `AppUser.CreatedAt` for time-based filtering.

#### Scenario: User counts displayed
- **WHEN** the dashboard loads
- **THEN** it shows total users, new users this week, and new users this month

### Requirement: Listing statistics
The dashboard SHALL display: total listings count, active listings count, and inactive listings count.

#### Scenario: Listing counts displayed
- **WHEN** the dashboard loads
- **THEN** it shows total, active, and inactive listing counts

### Requirement: Booking statistics
The dashboard SHALL display: total bookings count and a breakdown by `BookingStatus` (Pending, Confirmed, InProgress, ProviderCompleted, ClientConfirmed, Archived, Cancelled, Disputed).

#### Scenario: Booking breakdown displayed
- **WHEN** the dashboard loads
- **THEN** it shows total bookings and count per status

### Requirement: Revenue statistics
The dashboard SHALL display: total revenue (sum of `Payment.Amount`), total platform fees earned (sum of `Payment.PlatformFee`), and revenue this month.

#### Scenario: Revenue figures displayed
- **WHEN** the dashboard loads
- **THEN** it shows total revenue, platform fees, and this month's revenue

### Requirement: Dispute summary
The dashboard SHALL display: count of active disputes (payments with `PaymentStatus.Disputed`) and count of recently resolved disputes (payments changed from Disputed to Released or Refunded).

#### Scenario: Dispute counts displayed
- **WHEN** the dashboard loads
- **THEN** it shows active dispute count and resolved dispute count

### Requirement: Recent activity
The dashboard SHALL display the 10 most recent bookings with their status, client name, listing title, and creation date.

#### Scenario: Recent bookings displayed
- **WHEN** the dashboard loads
- **THEN** it shows the 10 most recent bookings with key details

### Requirement: AppUser CreatedAt field
The `AppUser` entity SHALL have a `DateTime CreatedAt` property. A new EF migration SHALL add this column with a default value of `DateTime.UtcNow` for existing rows.

#### Scenario: New user gets CreatedAt
- **WHEN** a new AppUser is created
- **THEN** the `CreatedAt` property is set to the current UTC time

### Requirement: Dashboard stats are computed in the database
The system SHALL compute all dashboard aggregate values (counts, sums, breakdowns) using database-level queries. The implementation MUST NOT load full entity collections into application memory for the purpose of aggregation. Scalar results (`CountAsync`, `SumAsync`) and grouped projections SHALL be used instead.

#### Scenario: Dashboard loads without full table scans
- **WHEN** an admin navigates to `/Admin/Dashboard`
- **THEN** the dashboard stats are produced by SQL aggregation queries, and no full rows of `AppUser`, `ServiceListing`, `Booking`, or `Payment` are loaded into memory solely for counting or summing purposes

#### Scenario: Recent bookings list remains a materialized query
- **WHEN** the dashboard loads the recent activity section
- **THEN** only the 10 most recent booking rows (with their required navigation properties) are fetched from the database
