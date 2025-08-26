using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YigitLancer.Migrations
{
    /// <inheritdoc />
    public partial class JobPurchasedbyWho2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conversations_BuyerUserId",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BuyerUserId_FreelancerUserId_JobId",
                table: "Conversations",
                columns: new[] { "BuyerUserId", "FreelancerUserId", "JobId" },
                unique: true,
                filter: "[JobId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conversations_BuyerUserId_FreelancerUserId_JobId",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BuyerUserId",
                table: "Conversations",
                column: "BuyerUserId");
        }
    }
}
