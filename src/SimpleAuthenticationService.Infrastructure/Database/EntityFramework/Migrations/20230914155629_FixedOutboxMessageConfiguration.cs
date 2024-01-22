using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleAuthenticationService.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixedOutboxMessageConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "outbox_messages",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Error",
                table: "outbox_messages",
                newName: "error");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "outbox_messages",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "outbox_messages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProcessedAtUtc",
                table: "outbox_messages",
                newName: "processed_at_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "outbox_messages",
                newName: "created_at_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "outbox_messages",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "error",
                table: "outbox_messages",
                newName: "Error");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "outbox_messages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "outbox_messages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "processed_at_utc",
                table: "outbox_messages",
                newName: "ProcessedAtUtc");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "outbox_messages",
                newName: "CreatedAtUtc");
        }
    }
}
