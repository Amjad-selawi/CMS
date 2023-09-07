using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class nn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CandidateId",
                table: "Interviews",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_PositionId",
                table: "Interviews",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Candidates_CandidateId",
                table: "Interviews",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Positions_PositionId",
                table: "Interviews",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Candidates_CandidateId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Positions_PositionId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_CandidateId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_PositionId",
                table: "Interviews");
        }
    }
}
