using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class UpdateChoosePlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Users",
                newName: "ChoosePlan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChoosePlan",
                table: "Users",
                newName: "MyProperty");
        }
    }
}
