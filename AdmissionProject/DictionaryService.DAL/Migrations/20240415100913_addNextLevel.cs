using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DictionaryService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addNextLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NextEducationLevel_DocumentTypes_DocumentTypeId",
                table: "NextEducationLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_NextEducationLevel_EducationLevels_EducationLevelId",
                table: "NextEducationLevel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NextEducationLevel",
                table: "NextEducationLevel");

            migrationBuilder.RenameTable(
                name: "NextEducationLevel",
                newName: "NextEducationLevelDocuments");

            migrationBuilder.RenameIndex(
                name: "IX_NextEducationLevel_DocumentTypeId",
                table: "NextEducationLevelDocuments",
                newName: "IX_NextEducationLevelDocuments_DocumentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NextEducationLevelDocuments",
                table: "NextEducationLevelDocuments",
                columns: new[] { "EducationLevelId", "DocumentTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_NextEducationLevelDocuments_DocumentTypes_DocumentTypeId",
                table: "NextEducationLevelDocuments",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NextEducationLevelDocuments_EducationLevels_EducationLevelId",
                table: "NextEducationLevelDocuments",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NextEducationLevelDocuments_DocumentTypes_DocumentTypeId",
                table: "NextEducationLevelDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_NextEducationLevelDocuments_EducationLevels_EducationLevelId",
                table: "NextEducationLevelDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NextEducationLevelDocuments",
                table: "NextEducationLevelDocuments");

            migrationBuilder.RenameTable(
                name: "NextEducationLevelDocuments",
                newName: "NextEducationLevel");

            migrationBuilder.RenameIndex(
                name: "IX_NextEducationLevelDocuments_DocumentTypeId",
                table: "NextEducationLevel",
                newName: "IX_NextEducationLevel_DocumentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NextEducationLevel",
                table: "NextEducationLevel",
                columns: new[] { "EducationLevelId", "DocumentTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_NextEducationLevel_DocumentTypes_DocumentTypeId",
                table: "NextEducationLevel",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NextEducationLevel_EducationLevels_EducationLevelId",
                table: "NextEducationLevel",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
