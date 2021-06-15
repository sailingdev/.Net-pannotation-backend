using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Update_Songsheets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songsheets_Files_TrackId",
                table: "Songsheets");

            migrationBuilder.AlterColumn<int>(
                name: "TrackId",
                table: "Songsheets",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Songsheets_Files_TrackId",
                table: "Songsheets",
                column: "TrackId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songsheets_Files_TrackId",
                table: "Songsheets");

            migrationBuilder.AlterColumn<int>(
                name: "TrackId",
                table: "Songsheets",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Songsheets_Files_TrackId",
                table: "Songsheets",
                column: "TrackId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
