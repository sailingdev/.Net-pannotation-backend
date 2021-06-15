using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Drop_CreditCard_and_Create_BillingAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_CreditCard_CardId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_CardId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "Transaction");

            migrationBuilder.AddColumn<string>(
                name: "CardMask",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardType",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardholderName",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Transaction",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BillingAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Country = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true),
                    TransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingAddress_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillingAddress_TransactionId",
                table: "BillingAddress",
                column: "TransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingAddress");

            migrationBuilder.DropColumn(
                name: "CardMask",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CardType",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CardholderName",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Transaction");

            migrationBuilder.AddColumn<int>(
                name: "CardId",
                table: "Transaction",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CreditCard",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardMask = table.Column<string>(nullable: false),
                    CardType = table.Column<string>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCard_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CardId",
                table: "Transaction",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_UserId",
                table: "CreditCard",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_CreditCard_CardId",
                table: "Transaction",
                column: "CardId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
