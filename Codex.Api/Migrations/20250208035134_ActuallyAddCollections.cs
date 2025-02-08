using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codex.Api.Migrations
{
    /// <inheritdoc />
    public partial class ActuallyAddCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CollectionId",
                table: "Notes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CollectionId",
                table: "Bookmarks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CollectionId",
                table: "Notes",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_CollectionId",
                table: "Bookmarks",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Collections_CollectionId",
                table: "Bookmarks",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Collections_CollectionId",
                table: "Notes",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Collections_CollectionId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Collections_CollectionId",
                table: "Notes");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Notes_CollectionId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_CollectionId",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Bookmarks");
        }
    }
}
