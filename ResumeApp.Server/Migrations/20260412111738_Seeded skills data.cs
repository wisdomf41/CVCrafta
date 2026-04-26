using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class Seededskillsdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Category", "DisplayOrder", "ResumeId", "Skills" },
                values: new object[,]
                {
                    { 1, "Backend", 1, 1, "C#, .NET Core Web API, REST APIs" },
                    { 2, "Database", 2, 1, "SQL Server, Entity Framework Core, LINQ" },
                    { 3, "Frontend", 3, 1, "React, HTML 5, CSS3, JavaScript (ES6+), Vite" },
                    { 4, "Tools", 4, 1, "Git, GitHub, Visual Studio, Azure" },
                    { 5, "Concepts", 5, 1, "Full-Stack Development, API Integration, Component-Based Architecture, Authentication & Authorization" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
