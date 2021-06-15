using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class CreateCategoryDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genre_Favourite_FilmId",
                table: "Genre");

            migrationBuilder.AddForeignKey(
                name: "FK_Genre_Favourite_FilmId",
                table: "Genre",
                column: "FilmId",
                principalTable: "Favourite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genre_Favourite_FilmId",
                table: "Genre");

            migrationBuilder.AddForeignKey(
                name: "FK_Genre_Favourite_FilmId",
                table: "Genre",
                column: "FilmId",
                principalTable: "Favourite",
                principalColumn: "Id");
        }
    }
}
