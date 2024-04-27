using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changePassportNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "series",
                table: "PassportsData",
                newName: "Series");

            migrationBuilder.RenameColumn(
                name: "number",
                table: "PassportsData",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "issuePlace",
                table: "PassportsData",
                newName: "IssuePlace");

            migrationBuilder.RenameColumn(
                name: "issueDate",
                table: "PassportsData",
                newName: "IssueDate");

            migrationBuilder.RenameColumn(
                name: "birthPlace",
                table: "PassportsData",
                newName: "BirthPlace");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                table: "PassportsData",
                newName: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Series",
                table: "PassportsData",
                newName: "series");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "PassportsData",
                newName: "number");

            migrationBuilder.RenameColumn(
                name: "IssuePlace",
                table: "PassportsData",
                newName: "issuePlace");

            migrationBuilder.RenameColumn(
                name: "IssueDate",
                table: "PassportsData",
                newName: "issueDate");

            migrationBuilder.RenameColumn(
                name: "BirthPlace",
                table: "PassportsData",
                newName: "birthPlace");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "PassportsData",
                newName: "ownerId");
        }
    }
}
