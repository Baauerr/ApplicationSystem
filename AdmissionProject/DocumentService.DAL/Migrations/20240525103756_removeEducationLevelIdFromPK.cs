using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeEducationLevelIdFromPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EducationDocumentsData",
                table: "EducationDocumentsData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducationDocumentsData",
                table: "EducationDocumentsData",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EducationDocumentsData",
                table: "EducationDocumentsData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducationDocumentsData",
                table: "EducationDocumentsData",
                columns: new[] { "OwnerId", "EducationLevelId" });
        }
    }
}
