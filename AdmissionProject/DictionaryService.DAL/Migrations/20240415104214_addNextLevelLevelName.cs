using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DictionaryService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addNextLevelLevelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NextEducationLevelDocuments_EducationLevels_EducationLevelId",
                table: "NextEducationLevelDocuments");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevelName",
                table: "NextEducationLevelDocuments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevelName",
                table: "NextEducationLevelDocuments");

            migrationBuilder.AddForeignKey(
                name: "FK_NextEducationLevelDocuments_EducationLevels_EducationLevelId",
                table: "NextEducationLevelDocuments",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
