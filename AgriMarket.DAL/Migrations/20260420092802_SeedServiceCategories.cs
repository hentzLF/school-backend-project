using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedServiceCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000001"), "Round and square baling services", "Hay Baling" },
                    { new Guid("a1b2c3d4-0002-0000-0000-000000000002"), "Grain and cereal harvesting", "Combine Harvesting" },
                    { new Guid("a1b2c3d4-0003-0000-0000-000000000003"), "Crop protection and fertilizer spraying", "Spraying" },
                    { new Guid("a1b2c3d4-0004-0000-0000-000000000004"), "Ploughing, discing, and cultivating", "Soil Preparation" },
                    { new Guid("a1b2c3d4-0005-0000-0000-000000000005"), "Precision and broadcast seeding", "Seeding" },
                    { new Guid("a1b2c3d4-0006-0000-0000-000000000006"), "Grass and hay mowing services", "Mowing" },
                    { new Guid("a1b2c3d4-0007-0000-0000-000000000007"), "Agricultural cargo transport", "Transport" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0002-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0003-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0004-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0005-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0006-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0007-0000-0000-000000000007"));
        }
    }
}
