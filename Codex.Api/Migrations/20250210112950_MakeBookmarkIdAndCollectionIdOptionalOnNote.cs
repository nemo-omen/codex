using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codex.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeBookmarkIdAndCollectionIdOptionalOnNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Bookmarks_BookmarkId",
                table: "Notes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notes",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookmarkId",
                table: "Notes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Bookmarks_BookmarkId",
                table: "Notes",
                column: "BookmarkId",
                principalTable: "Bookmarks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Bookmarks_BookmarkId",
                table: "Notes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "BookmarkId",
                table: "Notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Bookmarks_BookmarkId",
                table: "Notes",
                column: "BookmarkId",
                principalTable: "Bookmarks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
