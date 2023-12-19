using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qujat.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Categories_IconBlobId",
                schema: "public",
                table: "Categories",
                column: "IconBlobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Icons_IconBlobId",
                schema: "public",
                table: "Categories",
                column: "IconBlobId",
                principalSchema: "public",
                principalTable: "Icons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Icons_IconBlobId",
                schema: "public",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IconBlobId",
                schema: "public",
                table: "Categories");
        }
    }
}
