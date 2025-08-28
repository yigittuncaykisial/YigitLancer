using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class ChatSeen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SeenByBuyerAt",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SeenByFreelancerAt",
                table: "Messages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeenByBuyerAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SeenByFreelancerAt",
                table: "Messages");
        }
    }
}
