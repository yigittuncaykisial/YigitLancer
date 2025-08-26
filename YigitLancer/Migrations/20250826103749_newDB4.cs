using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class newDB4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 77);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Age", "IsAdmin", "Name", "ProfileImagePath", "Surname", "UserDescription", "UserEmail", "UserJob", "UserName", "UserPassword" },
                values: new object[] { 90, 25, true, "admin", null, "admin", null, null, null, "admin", "123456" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 90);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Age", "IsAdmin", "Name", "ProfileImagePath", "Surname", "UserDescription", "UserEmail", "UserJob", "UserName", "UserPassword" },
                values: new object[] { 77, 25, true, "Yiğit", null, "Kişial", null, null, null, "yigitk", "123456" });
        }
    }
}
