using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Citizenship = table.Column<string>(type: "text", nullable: false),
                    LastChangeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApplicationStatus = table.Column<int>(type: "integer", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationsPrograms",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationsPrograms", x => new { x.ApplicationId, x.ProgramId });
                });

            migrationBuilder.CreateTable(
                name: "EducationDocumentsData",
                columns: table => new
                {
                    ownerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentsData", x => new { x.ownerId, x.EducationDocumentId });
                });

            migrationBuilder.CreateTable(
                name: "PassportsData",
                columns: table => new
                {
                    ownerId = table.Column<Guid>(type: "uuid", nullable: false),
                    series = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    birthPlace = table.Column<string>(type: "text", nullable: false),
                    issueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    issuePlace = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportsData", x => x.ownerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "ApplicationsPrograms");

            migrationBuilder.DropTable(
                name: "EducationDocumentsData");

            migrationBuilder.DropTable(
                name: "PassportsData");
        }
    }
}
