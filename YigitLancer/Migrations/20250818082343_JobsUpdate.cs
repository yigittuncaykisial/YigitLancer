using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class JobsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPurchased",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "JobPrice",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedByUserId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Photoshop" },
                    { 2, "Video Edit" },
                    { 3, "Software" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PurchasedByUserId",
                table: "Jobs",
                column: "PurchasedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_PurchasedByUserId",
                table: "Jobs",
                column: "PurchasedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_PurchasedByUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_PurchasedByUserId",
                table: "Jobs");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "IsPurchased",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobPrice",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PurchasedByUserId",
                table: "Jobs");
        }
    }
}
