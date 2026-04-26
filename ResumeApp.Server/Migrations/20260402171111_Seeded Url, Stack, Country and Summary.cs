using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class SeededUrlStackCountryandSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Resumes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Country", "Email", "Stack", "Summary", "Url" },
                values: new object[] { "Nigeria", "wisdomf41@gmail.com", "React | SQL | ASP.NET Core | Rest API", "Full-Stack .NET Developer with experience in building full-stack applications using C#, ASP.NET Core Web API, and React. Skilled in developing RESTful APIs, SQL Server, and Entity Framework Core, with a focus on performance and scalability. Experienced in implementing CI/CD pipelines (GitHub Actions/Azure DevOps) to automate builds and deployments. Strong collaborator in Agile and remote teams, passionate about building efficient, maintainable systems.", "GitHub / LinkedIn (@wisdomf41)" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Resumes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Country", "Email", "Stack", "Summary", "Url" },
                values: new object[] { null, "wisdom41@gmail.com", "", null, "" });
        }
    }
}
