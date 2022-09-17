using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FancyEventStore.EfCoreStore.Migrations
{
    public partial class AddEventStreamTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_StreamId_Version",
                table: "Events");

            migrationBuilder.CreateTable(
                name: "EventStreams",
                columns: table => new
                {
                    StreamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStreams", x => x.StreamId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_StreamId",
                table: "Events",
                column: "StreamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventStreams_StreamId",
                table: "Events",
                column: "StreamId",
                principalTable: "EventStreams",
                principalColumn: "StreamId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventStreams_StreamId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventStreams");

            migrationBuilder.DropIndex(
                name: "IX_Events_StreamId",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_Events_StreamId_Version",
                table: "Events",
                columns: new[] { "StreamId", "Version" },
                unique: true);
        }
    }
}
