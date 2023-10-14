using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class removedAddressFromCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                table: "Companies",
                nullable: true);

       
         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonName",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

   
            
        }
    }
}
