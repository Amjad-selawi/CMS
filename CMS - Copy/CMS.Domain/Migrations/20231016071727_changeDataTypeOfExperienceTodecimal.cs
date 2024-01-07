using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class changeDataTypeOfExperienceTodecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
           


            migrationBuilder.AlterColumn<decimal>(
                name: "Experience",
                table: "Candidates",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

        
       
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.AlterColumn<string>(
                name: "Experience",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal));

          

         
        }
    }
}
