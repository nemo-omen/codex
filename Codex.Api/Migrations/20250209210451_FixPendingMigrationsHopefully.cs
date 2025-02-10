using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codex.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingMigrationsHopefully : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Collections_Name_UserId",
                table: "Collections",
                columns: new[] { "Name", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Collections_Name_UserId",
                table: "Collections");
        }
    }
}
