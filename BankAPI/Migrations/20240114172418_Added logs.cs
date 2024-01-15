using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptedInfo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "CryptedInfo",
                value: "hrnR0Hvz9bDBaOqtKTD8T8WHGEl5mJA6Q1i9RE5jKEpJAy1WmVq82kVapmVySwVM7CpDZ3sUfNcW+YrmxrUv2mjdwIYTkQLdyW3kaRHT0olFmSXBOkVmo0BSCZpqRbvE");

            migrationBuilder.UpdateData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "CryptedInfo",
                value: "hrnR0Hvz9bDBaOqtKTD8T8WHGEl5mJA6Q1i9RE5jKEoWdu6ZpVheZKpOdLru/uCzjQCoAyBdRZ945loCctYJiqILqFvs3aNY+lUw2xN5T/eM7zXl8fBTLZLuH5Qak114");

            migrationBuilder.UpdateData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "CryptedInfo",
                value: "hrnR0Hvz9bDBaOqtKTD8T8WHGEl5mJA6Q1i9RE5jKEqxoilWwdn2qmydxCShlty7pGs5AFJR1ziZT71XnMPo50AoMIH4DA7Dfb6Hz0kTL1Suc5L1+/obo0YuT0tE45WC");

            migrationBuilder.InsertData(
                table: "Logs",
                columns: new[] { "Id", "CryptedInfo" },
                values: new object[] { 1, "PPfQrALMIQrHfWui0uoXGXxeXNv6b9MP4OckRNUJjpYrbw592WxQ7ei9jxLRlwMa9OwXcopIFgldUavdXy6HZQGxm2bP4T+PiVhYbs5u1ppcKssEe/y4sUwMIgGNRXwPltjZfEXRbLle4bt8WpmC9Q==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.UpdateData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "CryptedInfo",
                value: "hrnR0Hvz9bDBaOqtKTD8T7O91xI0LgScpxHddA+gv2k+VIGYJMXDmxrviPGozD/oYsuSSVw1C2gqOUBNoetgWzK8EvbdPP18pnXSUoiuR24k4rxjNhluuiFjaY31A+F3");

            migrationBuilder.UpdateData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "CryptedInfo",
                value: "hrnR0Hvz9bDBaOqtKTD8T7O91xI0LgScpxHddA+gv2maomC7HpAzLTvH1AcvAjgDl+ta43Xqu5xR3v/lRHoU0trl8owSt/eRJiKvYK0g3XJIv6tyJ2CcCvoVwZI6oHPo");

            migrationBuilder.UpdateData(
                table: "AccStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "CryptedInfo",
                value: "hrnR0Hvz9bDBaOqtKTD8T7O91xI0LgScpxHddA+gv2k59XX3O6//hsMuzJPS3yZ8CIz+7p5GMQ3d3Ghqr6Auy0u3ARt8Rkd1589N99dJbTgK1Epafe3hv9I2oaG6OnhN");
        }
    }
}
