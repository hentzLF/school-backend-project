using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEquipmentToProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_ServiceListings_ServiceListingId",
                table: "Equipments");

            migrationBuilder.RenameColumn(
                name: "ServiceListingId",
                table: "Equipments",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_ServiceListingId",
                table: "Equipments",
                newName: "IX_Equipments_UserProfileId");

            migrationBuilder.AddColumn<int>(
                name: "Condition",
                table: "Equipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HorsePower",
                table: "Equipments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Equipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Equipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceListingEquipments",
                columns: table => new
                {
                    ServiceListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceListingEquipments", x => new { x.ServiceListingId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_ServiceListingEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceListingEquipments_ServiceListings_ServiceListingId",
                        column: x => x.ServiceListingId,
                        principalTable: "ServiceListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceListingEquipments_EquipmentId",
                table: "ServiceListingEquipments",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_UserProfiles_UserProfileId",
                table: "Equipments",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_UserProfiles_UserProfileId",
                table: "Equipments");

            migrationBuilder.DropTable(
                name: "ServiceListingEquipments");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "HorsePower",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Equipments");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "Equipments",
                newName: "ServiceListingId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_UserProfileId",
                table: "Equipments",
                newName: "IX_Equipments_ServiceListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_ServiceListings_ServiceListingId",
                table: "Equipments",
                column: "ServiceListingId",
                principalTable: "ServiceListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
