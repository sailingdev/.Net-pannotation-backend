using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Create_Subscription_NextPaymentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextPaymentDate",
                table: "Subscriptions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextPaymentDate",
                table: "Subscriptions");
        }
    }
}
