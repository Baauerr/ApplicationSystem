using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class renameManagerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ManagerName",
                table: "Managers",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "ManagerEmail",
                table: "Managers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Managers",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Managers",
                newName: "ManagerName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Managers",
                newName: "ManagerEmail");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Managers",
                newName: "ManagerId");
        }
    }
}
