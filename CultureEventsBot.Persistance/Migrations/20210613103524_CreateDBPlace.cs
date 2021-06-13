using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class CreateDBPlace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Genre",
                table: "Genre");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genre",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Genre",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Favourite",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Favourite",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timetable",
                table: "Favourite",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genre",
                table: "Genre",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Genre",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Favourite");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Favourite");

            migrationBuilder.DropColumn(
                name: "Timetable",
                table: "Favourite");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genre",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genre",
                table: "Genre",
                column: "Name");
        }
    }
}
