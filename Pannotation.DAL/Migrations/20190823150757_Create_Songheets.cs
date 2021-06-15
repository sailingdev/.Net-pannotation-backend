using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pannotation.DAL.Migrations
{
    public partial class Create_Songheets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songsheets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    ArtistName = table.Column<string>(maxLength: 100, nullable: true),
                    Producer = table.Column<string>(maxLength: 100, nullable: true),
                    Arranger = table.Column<string>(maxLength: 100, nullable: true),
                    YouTubeLink = table.Column<string>(maxLength: 300, nullable: true),
                    Description = table.Column<string>(maxLength: 750, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(8, 2)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsTop = table.Column<bool>(nullable: false),
                    ImageId = table.Column<int>(nullable: false),
                    PreviewId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    TrackId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songsheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songsheets_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Songsheets_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Songsheets_Files_PreviewId",
                        column: x => x.PreviewId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Songsheets_Files_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SongsheetGenres",
                columns: table => new
                {
                    SongsheetId = table.Column<int>(nullable: false),
                    GenreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongsheetGenres", x => new { x.SongsheetId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_SongsheetGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SongsheetGenres_Songsheets_SongsheetId",
                        column: x => x.SongsheetId,
                        principalTable: "Songsheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SongsheetInstruments",
                columns: table => new
                {
                    SongsheetId = table.Column<int>(nullable: false),
                    InstrumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongsheetInstruments", x => new { x.SongsheetId, x.InstrumentId });
                    table.ForeignKey(
                        name: "FK_SongsheetInstruments_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SongsheetInstruments_Songsheets_SongsheetId",
                        column: x => x.SongsheetId,
                        principalTable: "Songsheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongsheetGenres_GenreId",
                table: "SongsheetGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_SongsheetInstruments_InstrumentId",
                table: "SongsheetInstruments",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Songsheets_FileId",
                table: "Songsheets",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Songsheets_ImageId",
                table: "Songsheets",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Songsheets_PreviewId",
                table: "Songsheets",
                column: "PreviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Songsheets_TrackId",
                table: "Songsheets",
                column: "TrackId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongsheetGenres");

            migrationBuilder.DropTable(
                name: "SongsheetInstruments");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Songsheets");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
