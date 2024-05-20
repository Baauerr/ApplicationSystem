using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addEducationLevelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationLevelName",
                table: "EducationDocumentsData",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevelName",
                table: "EducationDocumentsData");
        }
    }
}
