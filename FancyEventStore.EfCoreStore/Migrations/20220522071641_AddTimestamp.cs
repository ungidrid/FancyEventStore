using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FancyEventStore.EfCoreStore.Migrations
{
    public partial class AddTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "EventStreams",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "EventStreams");
        }
    }
}
