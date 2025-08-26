using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class NewHashedSeedData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "AQAAAAIAAYagAAAAEEqlsWXWPKKGYxMXG9BxTuifyq1bvcZ5ZE/iI9O3SbD85iyzve2D9wcnIY4+cMsmCg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "UserPassword",
                value: "AQAAAAIAAYagAAAAEEqlsWXWPKKGYxMXG9BxTuifyq1bvcZ5ZE/iI9O3SbD85iyzve2D9wcnIY4+cMsmCg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "123456");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "UserPassword",
                value: "123456");
        }
    }
}
