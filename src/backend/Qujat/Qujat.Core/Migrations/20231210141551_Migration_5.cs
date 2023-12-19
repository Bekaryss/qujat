using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qujat.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                schema: "public",
                table: "Links",
                newName: "Uri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Uri",
                schema: "public",
                table: "Links",
                newName: "Link");
        }
    }
}
