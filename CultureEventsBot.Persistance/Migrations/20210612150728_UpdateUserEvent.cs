using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class UpdateUserEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentEvent",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentEvent",
                table: "Users");
        }
    }
}
