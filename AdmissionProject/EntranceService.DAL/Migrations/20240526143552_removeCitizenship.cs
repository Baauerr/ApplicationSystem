using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeCitizenship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Citizenship",
                table: "Applications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Citizenship",
                table: "Applications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
