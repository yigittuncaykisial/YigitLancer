using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class NewHashedSeedData3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Age", "IsAdmin", "Name", "Surname", "UserDescription", "UserEmail", "UserJob", "UserName", "UserPassword" },
                values: new object[,]
                {
                    { 8, 25, true, "Yiğit", "Kişial", "Baba", "yigitkisial@gmail.com", "Admin", "yigitk", "AQAAAAIAAYagAAAAEEqlsWXWPKKGYxMXG9BxTuifyq1bvcZ5ZE/iI9O3SbD85iyzve2D9wcnIY4+cMsmCg==" },
                    { 9, 28, false, "Semih", "Aktepe", "Baba", "semihaktepe@gmail.com", "Freelancer", "semihaktepe", "AQAAAAIAAYagAAAAEEqlsWXWPKKGYxMXG9BxTuifyq1bvcZ5ZE/iI9O3SbD85iyzve2D9wcnIY4+cMsmCg==" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Age", "IsAdmin", "Name", "Surname", "UserDescription", "UserEmail", "UserJob", "UserName", "UserPassword" },
                values: new object[,]
                {
                    { 1, 25, true, "Yiğit", "Kişial", "Baba", "yigitkisial@gmail.com", "Admin", "yigitk", "AQAAAAIAAYagAAAAEEqlsWXWPKKGYxMXG9BxTuifyq1bvcZ5ZE/iI9O3SbD85iyzve2D9wcnIY4+cMsmCg==" },
                    { 2, 28, false, "Semih", "Aktepe", "Baba", "semihaktepe@gmail.com", "Freelancer", "semihaktepe", "AQAAAAIAAYagAAAAEEqlsWXWPKKGYxMXG9BxTuifyq1bvcZ5ZE/iI9O3SbD85iyzve2D9wcnIY4+cMsmCg==" }
                });
        }
    }
}
