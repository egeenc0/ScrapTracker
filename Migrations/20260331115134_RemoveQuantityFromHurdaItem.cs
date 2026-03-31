using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HurdaApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuantityFromHurdaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "HurdaItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "HurdaItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
