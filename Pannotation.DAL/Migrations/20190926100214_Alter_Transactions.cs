using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Alter_Transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TransactionStatus",
                table: "Transaction",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "PaymentOrderId",
                table: "Transaction",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentOrderId",
                table: "Transaction");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionStatus",
                table: "Transaction",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
