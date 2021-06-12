using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class UpdateUserAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdminWritingPost",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdminWritingPost",
                table: "Users");
        }
    }
}
