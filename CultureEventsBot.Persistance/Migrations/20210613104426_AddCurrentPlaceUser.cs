using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class AddCurrentPlaceUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentPlace",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPlace",
                table: "Users");
        }
    }
}
