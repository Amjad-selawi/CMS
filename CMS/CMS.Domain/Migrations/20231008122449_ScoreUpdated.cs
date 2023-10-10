using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class ScoreUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "Interviews",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "Interviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

        }
    }
}
