using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class UpdateChoosePlanToFilm : Migration
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
                principalColumn: "Id");
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
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
