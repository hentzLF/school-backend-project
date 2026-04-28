# Spec: Admin Category Management

## Purpose
Defines admin CRUD operations for service categories, including listing, creating, editing, and safe deletion with listing-count guards.

## Requirements

### Requirement: Category list view
The system SHALL provide a list of all `ServiceCategory` records at `/Admin/Categories` with columns: Name, Description, Listings count. The view SHALL use `CategoryListViewModel`.

#### Scenario: View all categories
- **WHEN** an admin navigates to `/Admin/Categories`
- **THEN** the system displays a table of all categories with listing counts

### Requirement: Category create
The system SHALL provide a create form at `/Admin/Categories/Create` accepting Name and Description. The form SHALL use `CategoryCreateViewModel` with validation (Name is required).

#### Scenario: Create category successfully
- **WHEN** an admin submits valid category data
- **THEN** the system creates the category and redirects to the category list

#### Scenario: Create with validation error
- **WHEN** an admin submits without a name
- **THEN** the system re-displays the form with a validation error

### Requirement: Category edit
The system SHALL provide an edit form at `/Admin/Categories/Edit/{id}` allowing the admin to update Name and Description. The form SHALL use `CategoryEditViewModel`.

#### Scenario: Edit category successfully
- **WHEN** an admin submits valid changes
- **THEN** the system saves changes and redirects to the category list

#### Scenario: Category not found on edit
- **WHEN** an admin navigates to edit a non-existent category
- **THEN** the system returns a 404 Not Found page

### Requirement: Category delete
The system SHALL allow admins to delete a category at `/Admin/Categories/Delete/{id}` with a confirmation page. The system SHALL prevent deletion if the category has associated listings.

#### Scenario: Delete category with no listings
- **WHEN** an admin confirms deletion of a category with no listings
- **THEN** the category is removed from the database

#### Scenario: Delete category with listings
- **WHEN** an admin attempts to delete a category that has associated listings
- **THEN** the system displays an error "Cannot delete category with existing listings"
