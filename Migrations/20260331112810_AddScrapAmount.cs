using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HurdaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddScrapAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScrapAmount",
                table: "HurdaItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScrapAmount",
                table: "HurdaItems");
        }
    }
}
