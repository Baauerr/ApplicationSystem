using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeNamesInDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "EducationDocumentsData",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                table: "EducationDocumentsData",
                newName: "OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevelId",
                table: "EducationDocumentsData",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "EducationDocumentsData");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EducationDocumentsData",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "EducationDocumentsData",
                newName: "ownerId");
        }
    }
}
