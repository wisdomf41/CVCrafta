using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class SeedResumeData_and_Added_my_details : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Resumes",
                columns: new[] { "Id", "Email", "Name", "Title" },
                values: new object[] { 1, "wisdom41@gmail.com", "Future Ibeche", "C# /.NET Developer" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Resumes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
