using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleAuthenticationService.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UserAccountLoginAsUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_login",
                table: "user_accounts",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_accounts_login",
                table: "user_accounts");
        }
    }
}
