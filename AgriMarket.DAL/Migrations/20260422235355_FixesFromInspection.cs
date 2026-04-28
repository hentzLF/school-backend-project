using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixesFromInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "Bookings" WHERE "ServiceListingId" IN (
                    SELECT "Id" FROM "ServiceListings" WHERE "ServiceCategoryId" IN (
                        'a1b2c3d4-0001-0000-0000-000000000001',
                        'a1b2c3d4-0002-0000-0000-000000000002',
                        'a1b2c3d4-0003-0000-0000-000000000003',
                        'a1b2c3d4-0004-0000-0000-000000000004',
                        'a1b2c3d4-0005-0000-0000-000000000005',
                        'a1b2c3d4-0006-0000-0000-000000000006',
                        'a1b2c3d4-0007-0000-0000-000000000007'
                    )
                );
                DELETE FROM "Availabilities" WHERE "ServiceListingId" IN (
                    SELECT "Id" FROM "ServiceListings" WHERE "ServiceCategoryId" IN (
                        'a1b2c3d4-0001-0000-0000-000000000001',
                        'a1b2c3d4-0002-0000-0000-000000000002',
                        'a1b2c3d4-0003-0000-0000-000000000003',
                        'a1b2c3d4-0004-0000-0000-000000000004',
                        'a1b2c3d4-0005-0000-0000-000000000005',
                        'a1b2c3d4-0006-0000-0000-000000000006',
                        'a1b2c3d4-0007-0000-0000-000000000007'
                    )
                );
                DELETE FROM "ServiceListings" WHERE "ServiceCategoryId" IN (
                    'a1b2c3d4-0001-0000-0000-000000000001',
                    'a1b2c3d4-0002-0000-0000-000000000002',
                    'a1b2c3d4-0003-0000-0000-000000000003',
                    'a1b2c3d4-0004-0000-0000-000000000004',
                    'a1b2c3d4-0005-0000-0000-000000000005',
                    'a1b2c3d4-0006-0000-0000-000000000006',
                    'a1b2c3d4-0007-0000-0000-000000000007'
                );
                """);

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

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedProfileId",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaInHectares",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Availabilities",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceListings_IsActive",
                table: "ServiceListings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewedProfileId",
                table: "Reviews",
                column: "ReviewedProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentAt",
                table: "Messages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                table: "Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_IsBooked",
                table: "Availabilities",
                column: "IsBooked");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_UserProfiles_ReviewedProfileId",
                table: "Reviews",
                column: "ReviewedProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_UserProfiles_ReviewedProfileId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_ServiceListings_IsActive",
                table: "ServiceListings");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewedProfileId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SentAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Status",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Availabilities_IsBooked",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "ReviewedProfileId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Availabilities");

            migrationBuilder.AlterColumn<double>(
                name: "AreaInHectares",
                table: "Bookings",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

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
    }
}
