using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addidtracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordChanged",
                table: "AccStatus");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AccStatus",
                newName: "status");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AccStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AccStatus");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "AccStatus",
                newName: "UserName");

            migrationBuilder.AddColumn<bool>(
                name: "PasswordChanged",
                table: "AccStatus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
