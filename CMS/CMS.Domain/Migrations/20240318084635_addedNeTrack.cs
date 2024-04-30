using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class addedNeTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
   

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Name" },
                values: new object[] { 7, "UI/UX" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 7);

        
        }
    }
}
