using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class UpdateDBEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiteUrl",
                table: "Events",
                newName: "Site_Url");

            migrationBuilder.RenameColumn(
                name: "ShortTitle",
                table: "Events",
                newName: "Short_Title");

            migrationBuilder.RenameColumn(
                name: "IsFree",
                table: "Events",
                newName: "Is_Free");

            migrationBuilder.RenameColumn(
                name: "AgeRestriction",
                table: "Events",
                newName: "Age_Restriction");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Site_Url",
                table: "Events",
                newName: "SiteUrl");

            migrationBuilder.RenameColumn(
                name: "Short_Title",
                table: "Events",
                newName: "ShortTitle");

            migrationBuilder.RenameColumn(
                name: "Is_Free",
                table: "Events",
                newName: "IsFree");

            migrationBuilder.RenameColumn(
                name: "Age_Restriction",
                table: "Events",
                newName: "AgeRestriction");
        }
    }
}
