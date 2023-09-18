using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class addCompanyToCandidate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Candidates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_CompanyId",
                table: "Candidates",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Companies_CompanyId",
                table: "Candidates",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Companies_CompanyId",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_CompanyId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Candidates");
        }
    }
}
