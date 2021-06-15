using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Create_ApplicationUser_ShouldCancelSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShouldCancelSubscription",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldCancelSubscription",
                table: "AspNetUsers");
        }
    }
}
