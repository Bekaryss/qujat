using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qujat.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlobStorageUri",
                schema: "public",
                table: "Icons",
                newName: "Uri");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "public",
                table: "Blobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "public",
                table: "Blobs");

            migrationBuilder.RenameColumn(
                name: "Uri",
                schema: "public",
                table: "Icons",
                newName: "BlobStorageUri");
        }
    }
}
