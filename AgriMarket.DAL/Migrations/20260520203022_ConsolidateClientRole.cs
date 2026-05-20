using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateClientRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert Provider-only users to Client (where no Farmer/Client row exists yet)
            migrationBuilder.Sql(
                """
                UPDATE "UserRoles" SET "Role" = 1
                WHERE "Role" = 2
                AND "AppUserId" NOT IN (SELECT "AppUserId" FROM "UserRoles" WHERE "Role" = 1);
                """);

            // Delete remaining Provider rows (duplicates)
            migrationBuilder.Sql(
                """
                DELETE FROM "UserRoles" WHERE "Role" = 2;
                """);

            // Renumber Admin from 3 to 2
            migrationBuilder.Sql(
                """
                UPDATE "UserRoles" SET "Role" = 2 WHERE "Role" = 3;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
