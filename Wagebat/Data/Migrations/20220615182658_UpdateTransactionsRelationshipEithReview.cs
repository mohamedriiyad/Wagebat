using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class UpdateTransactionsRelationshipEithReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_TransactionId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TransactionId",
                table: "Reviews",
                column: "TransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_TransactionId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TransactionId",
                table: "Reviews",
                column: "TransactionId");
        }
    }
}
