using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qujat.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlobStorageUri",
                schema: "public",
                table: "DocumentBlobs",
                newName: "Uri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Uri",
                schema: "public",
                table: "DocumentBlobs",
                newName: "BlobStorageUri");
        }
    }
}
