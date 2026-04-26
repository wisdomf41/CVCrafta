using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class SeperatedControllerssoeachmodulehasitsowncontroller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifcations_Resumes_ResumeId",
                table: "Certifcations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certifcations",
                table: "Certifcations");

            migrationBuilder.RenameTable(
                name: "Certifcations",
                newName: "Certifications");

            migrationBuilder.RenameIndex(
                name: "IX_Certifcations_ResumeId",
                table: "Certifications",
                newName: "IX_Certifications_ResumeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certifications",
                table: "Certifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Resumes_ResumeId",
                table: "Certifications",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Resumes_ResumeId",
                table: "Certifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certifications",
                table: "Certifications");

            migrationBuilder.RenameTable(
                name: "Certifications",
                newName: "Certifcations");

            migrationBuilder.RenameIndex(
                name: "IX_Certifications_ResumeId",
                table: "Certifcations",
                newName: "IX_Certifcations_ResumeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certifcations",
                table: "Certifcations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifcations_Resumes_ResumeId",
                table: "Certifcations",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
