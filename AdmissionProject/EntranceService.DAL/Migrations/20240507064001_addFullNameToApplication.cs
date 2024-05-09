using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addFullNameToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationDocumentsData");

            migrationBuilder.DropTable(
                name: "PassportsData");

            migrationBuilder.AddColumn<string>(
                name: "FacultyId",
                table: "ApplicationsPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FacultyName",
                table: "ApplicationsPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgramName",
                table: "ApplicationsPrograms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Applications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "ApplicationsPrograms");

            migrationBuilder.DropColumn(
                name: "FacultyName",
                table: "ApplicationsPrograms");

            migrationBuilder.DropColumn(
                name: "ProgramName",
                table: "ApplicationsPrograms");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Applications");

            migrationBuilder.CreateTable(
                name: "EducationDocumentsData",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLevelId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentsData", x => new { x.OwnerId, x.EducationDocumentId });
                });

            migrationBuilder.CreateTable(
                name: "PassportsData",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BirthPlace = table.Column<string>(type: "text", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuePlace = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Series = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportsData", x => x.OwnerId);
                });
        }
    }
}
