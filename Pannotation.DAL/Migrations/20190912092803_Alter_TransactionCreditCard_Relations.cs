using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Alter_TransactionCreditCard_Relations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_CardId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CardId",
                table: "Transaction",
                column: "CardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_CardId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CardId",
                table: "Transaction",
                column: "CardId",
                unique: true);
        }
    }
}
