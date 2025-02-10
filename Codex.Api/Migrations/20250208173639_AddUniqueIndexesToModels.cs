using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codex.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notes_Title_UserId_BookmarkId",
                table: "Notes",
                columns: new[] { "Title", "UserId", "BookmarkId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoteLinks_StartIndex_EndIndex_SourceId_TargetId",
                table: "NoteLinks",
                columns: new[] { "StartIndex", "EndIndex", "SourceId", "TargetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_Url_UserId",
                table: "Bookmarks",
                columns: new[] { "Url", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_Title_UserId_BookmarkId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_NoteLinks_StartIndex_EndIndex_SourceId_TargetId",
                table: "NoteLinks");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_Url_UserId",
                table: "Bookmarks");
        }
    }
}
