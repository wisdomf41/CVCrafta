using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedExperienceandchangedfromfetchthenthentoasyncawait : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experiences_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Experiences",
                columns: new[] { "Id", "Company", "Description", "Duration", "ResumeId", "Role" },
                values: new object[,]
                {
                    { 1, "Metclan Technologies", "Developed 3+ full-stack applications using ASP.NET Core Web API and React.|Built reusable React components, reducing development time by 30%.|Integrated REST APIs and optimized queries with Entity Framework Core, improving performance by 25%.|Implemented CI/CD pipelines using GitHub Actions and Azure DevOps.|Collaborated in an Agile team using Git for version control.", "2025 - 2026", 1, "Full-Stack .NET Developer" },
                    { 2, "Loctech Training Institute", "Developed applications using .NET, C#, HTML, CSS, JavaScript, and jQuery.|Contributed to the development of a healthcare management system, improving patient record handling and data accessibility.|Diagnosed and resolved 20+ software bugs, enhancing system reliability and user satisfaction.|Assisted in application testing, debugging, and deployment processes.", "2024 - 2025", 1, "Software Developer" },
                    { 3, "FirstBank Limited", "Installed, customized, and supported desktop and laptop workstations plus software systems for 100+ in-office and remote users.|Refurbished 100+ Windows 8 machines to Windows 10 in preparation for an Office 365 rollout across eight regional offices.|Mentored junior team members and supported IT systems operations.|Managed cloud migration for storage and backup systems.|Handled 20 to 40 onsite IT support calls daily.", "2021 - 2024", 1, "IT/Network Specialist" }
                });

            migrationBuilder.UpdateData(
                table: "Resumes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Stack",
                value: "React | SQL | ASP.NET Core | REST API");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_ResumeId",
                table: "Experiences",
                column: "ResumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.UpdateData(
                table: "Resumes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Stack",
                value: "React | SQL | ASP.NET Core | Rest API");
        }
    }
}
