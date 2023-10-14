using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class removeNullableFromCountryIdInCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Countries_CountryId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Companies",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

      
            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Countries_CountryId",
                table: "Companies",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Countries_CountryId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Companies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

          

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Countries_CountryId",
                table: "Companies",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
