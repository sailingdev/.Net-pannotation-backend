using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Create_OtherFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtherFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 750, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    FileType = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    PreviewId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OtherFiles_Images_PreviewId",
                        column: x => x.PreviewId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtherFiles_FileId",
                table: "OtherFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherFiles_PreviewId",
                table: "OtherFiles",
                column: "PreviewId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtherFiles");
        }
    }
}
