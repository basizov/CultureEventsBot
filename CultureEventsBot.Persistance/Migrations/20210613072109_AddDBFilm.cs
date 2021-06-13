using Microsoft.EntityFrameworkCore.Migrations;

namespace CultureEventsBot.Persistance.Migrations
{
    public partial class AddDBFilm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageResponse_Events_EventId",
                table: "ImageResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Favourite");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "ImageResponse",
                newName: "FavouriteId");

            migrationBuilder.RenameIndex(
                name: "IX_ImageResponse_EventId",
                table: "ImageResponse",
                newName: "IX_ImageResponse_FavouriteId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_UserId",
                table: "Favourite",
                newName: "IX_Favourite_UserId");

            migrationBuilder.AlterColumn<bool>(
                name: "Is_Free",
                table: "Favourite",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Favourite",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favourite",
                table: "Favourite",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favourite_Users_UserId",
                table: "Favourite",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageResponse_Favourite_FavouriteId",
                table: "ImageResponse",
                column: "FavouriteId",
                principalTable: "Favourite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favourite_Users_UserId",
                table: "Favourite");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageResponse_Favourite_FavouriteId",
                table: "ImageResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favourite",
                table: "Favourite");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Favourite");

            migrationBuilder.RenameTable(
                name: "Favourite",
                newName: "Events");

            migrationBuilder.RenameColumn(
                name: "FavouriteId",
                table: "ImageResponse",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_ImageResponse_FavouriteId",
                table: "ImageResponse",
                newName: "IX_ImageResponse_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Favourite_UserId",
                table: "Events",
                newName: "IX_Events_UserId");

            migrationBuilder.AlterColumn<bool>(
                name: "Is_Free",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageResponse_Events_EventId",
                table: "ImageResponse",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
