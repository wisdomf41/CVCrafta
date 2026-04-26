using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedtheCertificationssection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certifcations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearObtained = table.Column<int>(type: "int", nullable: false),
                    CredentialUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ResumeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifcations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certifcations_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Certifcations",
                columns: new[] { "Id", "CredentialUrl", "DisplayOrder", "Issuer", "Name", "ResumeId", "YearObtained" },
                values: new object[,]
                {
                    { 1, "", 1, "Udemy", ".NET Core Developer Certification", 1, 2025 },
                    { 2, "", 2, "Udemy", ".NET Core API Developer Certification", 1, 2025 },
                    { 3, "", 3, "Coursera", "Google IT Support Professional Certificate", 1, 2024 },
                    { 4, "", 4, "Udacity", "SQL for Data Analysis", 1, 2022 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certifcations_ResumeId",
                table: "Certifcations",
                column: "ResumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certifcations");
        }
    }
}
