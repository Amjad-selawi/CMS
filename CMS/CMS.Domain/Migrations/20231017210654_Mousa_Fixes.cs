using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class Mousa_Fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       
            migrationBuilder.AlterColumn<double>(
                name: "Experience",
                table: "Candidates",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

       
         

       

      

        
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<decimal>(
                name: "Experience",
                table: "Candidates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double));

      
    
    


        }
    }
}
