using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Service.Migrations
{
    /// <inheritdoc />
    public partial class FailedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailedEventLogs",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TraceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Retries = table.Column<int>(type: "int", nullable: false),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedEventLogs", x => x.EventId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailedEventLogs");
        }
    }
}
