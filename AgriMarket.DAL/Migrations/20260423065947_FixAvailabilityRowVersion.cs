using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixAvailabilityRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Availabilities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Availabilities",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: Array.Empty<byte>());
        }
    }
}
