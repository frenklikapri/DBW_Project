using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileSharing.Infrastructure.Migrations
{
    public partial class RemovedDownloadLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownloadLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeDownloadedAt",
                table: "FileDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTimeDownloadedAt",
                table: "FileDocuments");

            migrationBuilder.CreateTable(
                name: "DownloadLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
    }
}
