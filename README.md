# AgriMarket Backend

This is the backend solution for **AgriMarket**, built with .NET 10. The system provides a platform for agricultural service listings, bookings, and reviews.

## Solution Structure

The solution (`AgriMarket.slnx`) follows a Clean Architecture/N-Tier approach and is composed of the following projects:

*   **`AgriMarket.Api`**: The core ASP.NET Core Web API providing RESTful endpoints for users, service listings, bookings, and reviews. It serves as the primary backend interface.
*   **`AgriMarket.Web`**: An ASP.NET Core Web UI project.
*   **`AgriMarket.DAL`**: The Data Access Layer. It uses Entity Framework Core with a PostgreSQL database (`Npgsql`) to handle data persistence, migrations, and database seeding.
*   **`AgriMarket.Domain`**: The domain layer containing core entities (e.g., `AppUser`, `ServiceListing`, `Booking`, `Review`), enums, and domain logic. It has no dependencies on other layers.

## Features

*   **RESTful API**: Standardized JSON API with camelCase serialization and ProblemDetails for error handling.
*   **OpenAPI/Swagger**: API documentation and exploration enabled via Swagger UI (`/swagger`).
*   **Data Persistence**: Entity Framework Core with PostgreSQL.
*   **Modular Domains**:
    *   **Users**: Profiles and roles (Admin, Provider, Customer).
    *   **Service Listings**: Browsing and managing agricultural services.
    *   **Bookings**: Requesting and managing service appointments.
    *   **Reviews**: Rating and reviewing completed services.
*   **Testing & Specs**: `openspec` specs are used to maintain API design and project setup guidelines.

## Prerequisites

*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [PostgreSQL](https://www.postgresql.org/) (Ensure an instance is running and accessible)
*   Your preferred IDE (Visual Studio, Rider, VS Code)

## Getting Started

1.  **Clone the repository** and navigate to the backend directory.
2.  **Configure Database**:
    Update the `ConnectionStrings:DefaultConnection` in `AgriMarket.Api/appsettings.json` and `AgriMarket.Web/appsettings.json` to point to your local PostgreSQL instance.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=AgriMarketDb;Username=postgres;Password=yourpassword"
    }
    ```
3.  **Apply Migrations**:
    Navigate to the `AgriMarket.DAL` or run the following command from the root to apply database migrations:
    ```bash
    dotnet ef database update --project AgriMarket.DAL --startup-project AgriMarket.Web
    ```
4.  **Run the API**:
    Navigate to the `AgriMarket.Api` directory and start the application:
    ```bash
    cd AgriMarket.Api
    dotnet run
    ```
5.  **Explore the API**:
    Open your browser and navigate to the Swagger UI:
    `http://localhost:<port>/swagger`

## Development Guidelines

*   **API Standards**: Endpoints must adhere to the definitions in the `openspec/specs` directory.
*   **Error Handling**: All endpoints utilize RFC 7807 ProblemDetails for standardized error reporting.
*   **Null Handling**: Null values are ignored in JSON responses by default to reduce payload sizes.

## Specifications

The `openspec` directory contains design proposals and detailed behavior specifications for various API domains (Users, Listings, Bookings, Reviews). Please review these documents when making architectural changes or extending API functionality.
