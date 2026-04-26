using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class Updatedresumetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stack",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Resumes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Country", "Stack", "Summary", "Url" },
                values: new object[] { null, "", null, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Stack",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Resumes");
        }
    }
}
