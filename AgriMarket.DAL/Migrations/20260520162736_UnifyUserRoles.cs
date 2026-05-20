using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UnifyUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AppUserId_Role",
                table: "UserRoles",
                columns: new[] { "AppUserId", "Role" },
                unique: true);

            // Migrate data: copy ProfileRoles → UserRoles via UserProfile.AppUserId
            migrationBuilder.Sql(@"
                INSERT INTO ""UserRoles"" (""Id"", ""AppUserId"", ""Role"")
                SELECT gen_random_uuid(), up.""AppUserId"", pr.""Role""
                FROM ""ProfileRoles"" pr
                JOIN ""UserProfiles"" up ON pr.""UserProfileId"" = up.""Id""
                ON CONFLICT (""AppUserId"", ""Role"") DO NOTHING;
            ");

            migrationBuilder.DropTable(
                name: "ProfileRoles");

            // Consolidate multi-profile users: keep first profile, re-assign resources
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT ""Id"", ""AppUserId"",
                           ROW_NUMBER() OVER (PARTITION BY ""AppUserId"" ORDER BY ""Id"") AS rn
                    FROM ""UserProfiles""
                ),
                primary_profiles AS (
                    SELECT ""Id"" AS ""PrimaryId"", ""AppUserId""
                    FROM ranked WHERE rn = 1
                ),
                secondary_profiles AS (
                    SELECT r.""Id"" AS ""SecondaryId"", pp.""PrimaryId""
                    FROM ranked r
                    JOIN primary_profiles pp ON r.""AppUserId"" = pp.""AppUserId""
                    WHERE r.rn > 1
                )
                UPDATE ""ServiceListings"" sl
                SET ""UserProfileId"" = sp.""PrimaryId""
                FROM secondary_profiles sp
                WHERE sl.""UserProfileId"" = sp.""SecondaryId"";
            ");

            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT ""Id"", ""AppUserId"",
                           ROW_NUMBER() OVER (PARTITION BY ""AppUserId"" ORDER BY ""Id"") AS rn
                    FROM ""UserProfiles""
                ),
                primary_profiles AS (
                    SELECT ""Id"" AS ""PrimaryId"", ""AppUserId""
                    FROM ranked WHERE rn = 1
                ),
                secondary_profiles AS (
                    SELECT r.""Id"" AS ""SecondaryId"", pp.""PrimaryId""
                    FROM ranked r
                    JOIN primary_profiles pp ON r.""AppUserId"" = pp.""AppUserId""
                    WHERE r.rn > 1
                )
                UPDATE ""Bookings"" b
                SET ""ClientProfileId"" = sp.""PrimaryId""
                FROM secondary_profiles sp
                WHERE b.""ClientProfileId"" = sp.""SecondaryId"";
            ");

            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT ""Id"", ""AppUserId"",
                           ROW_NUMBER() OVER (PARTITION BY ""AppUserId"" ORDER BY ""Id"") AS rn
                    FROM ""UserProfiles""
                )
                DELETE FROM ""UserProfiles""
                WHERE ""Id"" IN (SELECT ""Id"" FROM ranked WHERE rn > 1);
            ");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_AppUserId",
                table: "UserProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_AppUserId",
                table: "UserProfiles",
                column: "AppUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_AppUserId",
                table: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "ProfileRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileRoles_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_AppUserId",
                table: "UserProfiles",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRoles_UserProfileId_Role",
                table: "ProfileRoles",
                columns: new[] { "UserProfileId", "Role" },
                unique: true);
        }
    }
}
