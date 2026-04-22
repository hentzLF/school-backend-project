using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ConversationParticipantCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_ConversationId_UserProfileId",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ConversationParticipants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserProfileId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ConversationParticipants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId_UserProfileId",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserProfileId" },
                unique: true);
        }
    }
}
