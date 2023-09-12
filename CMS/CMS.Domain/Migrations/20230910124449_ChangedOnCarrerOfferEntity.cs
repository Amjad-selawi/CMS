using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class ChangedOnCarrerOfferEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "CarrerOffers");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "CarrerOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarrerOffers_PositionId",
                table: "CarrerOffers",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrerOffers_Positions_PositionId",
                table: "CarrerOffers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrerOffers_Positions_PositionId",
                table: "CarrerOffers");

            migrationBuilder.DropIndex(
                name: "IX_CarrerOffers_PositionId",
                table: "CarrerOffers");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "CarrerOffers");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "CarrerOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
