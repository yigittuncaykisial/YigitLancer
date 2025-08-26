using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class newDB3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {




            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Age", "IsAdmin", "Name", "ProfileImagePath", "Surname", "UserDescription", "UserEmail", "UserJob", "UserName", "UserPassword" },
                values: new object[] { 77, 25, true, "Yiğit", null, "Kişial", null, null, null, "yigitk", "123456" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 77);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UserId1",
                table: "Jobs",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_UserId1",
                table: "Jobs",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
