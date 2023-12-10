using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class NewSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3F476A40-97F4-42C6-A226-602AED74A4BC", "fdc04d8c-1afc-4568-a4b5-4de3cdcc0b38", "Solution Architecture", "SOLUTION ARCHITECTURE" });

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3F476A40-97F4-42C6-A226-602AED74A4BC");

       
        }
    }
}
