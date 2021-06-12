using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class UpdateUserNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age_Restriction",
                table: "Events");

            migrationBuilder.AddColumn<bool>(
                name: "MayNotification",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MayNotification",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Age_Restriction",
                table: "Events",
                type: "text",
                nullable: true);
        }
    }
}
