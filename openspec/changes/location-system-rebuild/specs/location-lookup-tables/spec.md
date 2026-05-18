## ADDED Requirements

### Requirement: County entity
The system SHALL define a `County` entity with properties: `Id` (Guid, PK), `Name` (string, required), `EhakCode` (string, required, unique).

#### Scenario: County has all required fields
- **WHEN** a County record is read from the database
- **THEN** it SHALL contain a non-empty `Id`, `Name`, and `EhakCode`

#### Scenario: EhakCode is unique
- **WHEN** two County records exist in the database
- **THEN** their `EhakCode` values SHALL be distinct

### Requirement: Municipality entity
The system SHALL define a `Municipality` entity with properties: `Id` (Guid, PK), `Name` (string, required), `EhakCode` (string, required, unique), `CountyId` (Guid, FK to County, required).

#### Scenario: Municipality belongs to a County
- **WHEN** a Municipality record is read with its navigation property
- **THEN** it SHALL have a non-null `County` reference

#### Scenario: EhakCode is unique
- **WHEN** two Municipality records exist in the database
- **THEN** their `EhakCode` values SHALL be distinct

### Requirement: EHAK seed data
The system SHALL seed all 15 Estonian counties and all 79 municipalities using EF Core `HasData()` with static Guid IDs.

#### Scenario: Counties are seeded
- **WHEN** the database is created or migrated
- **THEN** the Counties table SHALL contain exactly 15 rows matching EHAK county data

#### Scenario: Municipalities are seeded
- **WHEN** the database is created or migrated
- **THEN** the Municipalities table SHALL contain exactly 79 rows, each referencing a valid County

### Requirement: Read-only Counties endpoint
The API SHALL expose `GET /api/v1/counties` returning all counties ordered by Name.

#### Scenario: List all counties
- **WHEN** `GET /api/v1/counties` is called
- **THEN** the response SHALL return HTTP 200 with an array of `{ id, name, ehakCode }`

#### Scenario: Response is ordered alphabetically
- **WHEN** `GET /api/v1/counties` is called
- **THEN** the counties SHALL be sorted by `Name` ascending

### Requirement: Read-only Municipalities endpoint
The API SHALL expose `GET /api/v1/counties/{countyId}/municipalities` returning all municipalities for a given county, ordered by Name.

#### Scenario: List municipalities for a county
- **WHEN** `GET /api/v1/counties/{countyId}/municipalities` is called with a valid county ID
- **THEN** the response SHALL return HTTP 200 with an array of `{ id, name, ehakCode, countyId }`

#### Scenario: Non-existent county
- **WHEN** `GET /api/v1/counties/{countyId}/municipalities` is called with an ID that does not exist
- **THEN** the response SHALL return HTTP 404

#### Scenario: Response is ordered alphabetically
- **WHEN** municipalities are returned for a county
- **THEN** they SHALL be sorted by `Name` ascending

### Requirement: County cascade delete behavior
The system SHALL use `DeleteBehavior.Restrict` for County → Municipality, preventing county deletion while municipalities reference it.

#### Scenario: Cannot delete county with municipalities
- **WHEN** a delete operation is attempted on a County that has Municipality references
- **THEN** the operation SHALL fail with a database constraint violation

### Requirement: Municipality cascade delete behavior
The system SHALL use `DeleteBehavior.Restrict` for Municipality → Location, preventing municipality deletion while locations reference it.

#### Scenario: Cannot delete municipality with locations
- **WHEN** a delete operation is attempted on a Municipality that has Location references
- **THEN** the operation SHALL fail with a database constraint violation
