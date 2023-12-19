using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qujat.Core.Migrations
{
    /// <inheritdoc />
    public partial class Migration_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SubcategoryId",
                schema: "public",
                table: "DocumentActionEventEntities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubcategoryId",
                schema: "public",
                table: "DocumentActionEventEntities");
        }
    }
}
