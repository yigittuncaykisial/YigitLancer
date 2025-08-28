using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class JobCreatedDateTime1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_PurchasedByUserId",
                table: "Jobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_PurchasedByUserId",
                table: "Jobs",
                column: "PurchasedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_PurchasedByUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Jobs");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_PurchasedByUserId",
                table: "Jobs",
                column: "PurchasedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
