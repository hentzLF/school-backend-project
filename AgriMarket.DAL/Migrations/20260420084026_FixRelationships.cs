using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_ServiceListings_ServiceListingId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_UserProfiles_ClientProfileId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_UserProfiles_UserProfileId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Bookings_BookingId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReads_UserProfiles_UserProfileId",
                table: "MessageReads");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserProfiles_SenderProfileId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserProfiles_UserProfileId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_UserProfiles_ReviewerProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceListings_Locations_LocationId",
                table: "ServiceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceListings_ServiceCategories_ServiceCategoryId",
                table: "ServiceListings");

            migrationBuilder.DropIndex(
                name: "IX_ProfileRoles_UserProfileId",
                table: "ProfileRoles");

            migrationBuilder.DropIndex(
                name: "IX_MessageReads_MessageId",
                table: "MessageReads");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "LucationId",
                table: "ServiceListings");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_Name",
                table: "ServiceCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRoles_UserProfileId_Role",
                table: "ProfileRoles",
                columns: new[] { "UserProfileId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OAuthAccounts_Provider_ProviderAccountId",
                table: "OAuthAccounts",
                columns: new[] { "Provider", "ProviderAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageId_UserProfileId",
                table: "MessageReads",
                columns: new[] { "MessageId", "UserProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId_UserProfileId",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_ServiceListings_ServiceListingId",
                table: "Bookings",
                column: "ServiceListingId",
                principalTable: "ServiceListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_UserProfiles_ClientProfileId",
                table: "Bookings",
                column: "ClientProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_UserProfiles_UserProfileId",
                table: "ConversationParticipants",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Bookings_BookingId",
                table: "Conversations",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReads_UserProfiles_UserProfileId",
                table: "MessageReads",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserProfiles_SenderProfileId",
                table: "Messages",
                column: "SenderProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserProfiles_UserProfileId",
                table: "Notifications",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_UserProfiles_ReviewerProfileId",
                table: "Reviews",
                column: "ReviewerProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceListings_Locations_LocationId",
                table: "ServiceListings",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceListings_ServiceCategories_ServiceCategoryId",
                table: "ServiceListings",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_ServiceListings_ServiceListingId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_UserProfiles_ClientProfileId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_UserProfiles_UserProfileId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Bookings_BookingId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReads_UserProfiles_UserProfileId",
                table: "MessageReads");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserProfiles_SenderProfileId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserProfiles_UserProfileId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_UserProfiles_ReviewerProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceListings_Locations_LocationId",
                table: "ServiceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceListings_ServiceCategories_ServiceCategoryId",
                table: "ServiceListings");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCategories_Name",
                table: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileRoles_UserProfileId_Role",
                table: "ProfileRoles");

            migrationBuilder.DropIndex(
                name: "IX_OAuthAccounts_Provider_ProviderAccountId",
                table: "OAuthAccounts");

            migrationBuilder.DropIndex(
                name: "IX_MessageReads_MessageId_UserProfileId",
                table: "MessageReads");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_ConversationId_UserProfileId",
                table: "ConversationParticipants");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "LucationId",
                table: "ServiceListings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRoles_UserProfileId",
                table: "ProfileRoles",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageId",
                table: "MessageReads",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipants",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_ServiceListings_ServiceListingId",
                table: "Bookings",
                column: "ServiceListingId",
                principalTable: "ServiceListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_UserProfiles_ClientProfileId",
                table: "Bookings",
                column: "ClientProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_UserProfiles_UserProfileId",
                table: "ConversationParticipants",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Bookings_BookingId",
                table: "Conversations",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReads_UserProfiles_UserProfileId",
                table: "MessageReads",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserProfiles_SenderProfileId",
                table: "Messages",
                column: "SenderProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserProfiles_UserProfileId",
                table: "Notifications",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_UserProfiles_ReviewerProfileId",
                table: "Reviews",
                column: "ReviewerProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceListings_Locations_LocationId",
                table: "ServiceListings",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceListings_ServiceCategories_ServiceCategoryId",
                table: "ServiceListings",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
