using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Alter_Subscription_Transaction_Relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_SubscriptionId",
                table: "Transaction");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchasedAt",
                table: "Subscriptions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_SubscriptionId",
                table: "Transaction",
                column: "SubscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_SubscriptionId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "PurchasedAt",
                table: "Subscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_SubscriptionId",
                table: "Transaction",
                column: "SubscriptionId",
                unique: true,
                filter: "[SubscriptionId] IS NOT NULL");
        }
    }
}
