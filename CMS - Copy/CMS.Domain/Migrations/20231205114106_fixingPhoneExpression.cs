using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class fixingPhoneExpression : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Candidates",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

         


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Phone",
                table: "Candidates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

           
        }
    }
}
