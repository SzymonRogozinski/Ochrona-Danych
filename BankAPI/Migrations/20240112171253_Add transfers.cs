using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addtransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Transfers");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Transfers",
                newName: "CryptedInfo");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Accounts",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "AccStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HowManyTrials = table.Column<int>(type: "int", nullable: false),
                    PasswordChanged = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Transfers",
                columns: new[] { "Id", "Address", "CryptedInfo", "Sender" },
                values: new object[,]
                {
                    { 1, 2, "RfEtDRzoOaSuT8BykxYZvRgApNo/Phe9DAcEkA/8v6fioe7MKnSO8A5k39HJ0n/2fuVDISBKZ+brvbor9LSTw7lrmVQi3dY6MEZflh8tSEZMsSkbr5Bm3+Ll9q3538lk", 1 },
                    { 2, 3, "17Ekpj5CXERlXjlD1IJXe04vrjsg91U7X8xvs3gpx1+OWfX3KGTVqI3Bq+4ilavWdDKSJ46HgArhqxhXyycVbhOLxtx7pOHgbUs6fFNV1v7iPVA2TOMNZPrFiRWE8eLC", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccStatus");

            migrationBuilder.DeleteData(
                table: "Transfers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Transfers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "CryptedInfo",
                table: "Transfers",
                newName: "Title");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Transfers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Transfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HowManyTrials = table.Column<int>(type: "int", nullable: false),
                    PasswordChanged = table.Column<bool>(type: "bit", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                });
        }
    }
}
