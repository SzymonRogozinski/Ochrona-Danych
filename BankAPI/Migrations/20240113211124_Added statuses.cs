using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedstatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HowManyTrials",
                table: "AccStatus");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "AccStatus");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "AccStatus",
                newName: "CryptedInfo");

            migrationBuilder.InsertData(
                table: "AccStatus",
                columns: new[] { "Id", "CryptedInfo", "UserId" },
                values: new object[,]
                {
                    { 1, "hrnR0Hvz9bDBaOqtKTD8T7O91xI0LgScpxHddA+gv2k+VIGYJMXDmxrviPGozD/oYsuSSVw1C2gqOUBNoetgWzK8EvbdPP18pnXSUoiuR24k4rxjNhluuiFjaY31A+F3", 1 },
                    { 2, "hrnR0Hvz9bDBaOqtKTD8T7O91xI0LgScpxHddA+gv2maomC7HpAzLTvH1AcvAjgDl+ta43Xqu5xR3v/lRHoU0trl8owSt/eRJiKvYK0g3XJIv6tyJ2CcCvoVwZI6oHPo", 2 },
                    { 3, "hrnR0Hvz9bDBaOqtKTD8T7O91xI0LgScpxHddA+gv2k59XX3O6//hsMuzJPS3yZ8CIz+7p5GMQ3d3Ghqr6Auy0u3ARt8Rkd1589N99dJbTgK1Epafe3hv9I2oaG6OnhN", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "CryptedInfo",
                table: "AccStatus",
                newName: "status");

            migrationBuilder.AddColumn<int>(
                name: "HowManyTrials",
                table: "AccStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "AccStatus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
