using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Service.Migrations
{
    /// <inheritdoc />
    public partial class FailedEventsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SuccessfullyHandled",
                table: "FailedEventLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuccessfullyHandled",
                table: "FailedEventLogs");
        }
    }
}
