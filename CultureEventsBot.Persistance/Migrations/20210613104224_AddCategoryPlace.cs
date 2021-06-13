using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class AddCategoryPlace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Place_Categories",
                table: "Favourite",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Place_Categories",
                table: "Favourite");
        }
    }
}
