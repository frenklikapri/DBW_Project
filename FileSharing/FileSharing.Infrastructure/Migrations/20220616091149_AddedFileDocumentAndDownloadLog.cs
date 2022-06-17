using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileSharing.Infrastructure.Migrations
{
    public partial class AddedFileDocumentAndDownloadLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Bytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DownloadLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DownloadLogs_FileDocuments_FileDocumentId",
                        column: x => x.FileDocumentId,
                        principalTable: "FileDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DownloadLogs_FileDocumentId",
                table: "DownloadLogs",
                column: "FileDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownloadLogs");

            migrationBuilder.DropTable(
                name: "FileDocuments");
        }
    }
}
