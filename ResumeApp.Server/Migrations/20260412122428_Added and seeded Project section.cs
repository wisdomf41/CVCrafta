using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedandseededProjectsection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Technologies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GithubUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ResumeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Description", "DisplayOrder", "GithubUrl", "Name", "ProjectUrl", "ResumeId", "Technologies" },
                values: new object[,]
                {
                    { 1, "Full-stack resume application with responsive design, deployed to Azure cloud.", 1, "", "Resume Website (This Project)", "", 1, "React + Vite frontend, ASP.NET Core backend, Azure App Service" },
                    { 2, "Full-stack task management system with CRUD operations, authentication, and responsive UI.", 2, "", "Task Management API with React Frontend", "", 1, "ASP.NET Core Web API, React, SQL Server, Entity Framework" },
                    { 3, "Complete e-commerce solution with product management, shopping cart, and order processing.", 3, "", "E-Commerce MVC Application", "", 1, "ASP.NET Core MVC, HTML 5, CSS 3, JavaScript, SQL Server" },
                    { 4, "RESTful API for hotel room booking with customer management and reservation tracking.", 4, "", "Hotel Booking API", "", 1, "ASP.NET Core Web API, SQL Server, Entity Framework" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ResumeId",
                table: "Projects",
                column: "ResumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
