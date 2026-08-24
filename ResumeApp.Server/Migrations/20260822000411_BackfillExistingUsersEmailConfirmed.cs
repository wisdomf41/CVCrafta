using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class BackfillExistingUsersEmailConfirmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Updated: Preserve access for users created before email-confirmation enforcement.
            migrationBuilder.Sql(
                """
                UPDATE [AspNetUsers]
                SET [EmailConfirmed] = CAST(1 AS bit)
                WHERE [EmailConfirmed] = CAST(0 AS bit);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}