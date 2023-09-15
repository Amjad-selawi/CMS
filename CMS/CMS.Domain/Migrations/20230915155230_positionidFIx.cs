using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class positionidFIx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Positions_PositionId",
                table: "Candidates"
                );
            migrationBuilder.DropIndex(
                name: "IX_Candidates_PositionId",
                table: "Candidates"
                );
            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Candidates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
