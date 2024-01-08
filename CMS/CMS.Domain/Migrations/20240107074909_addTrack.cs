using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class addTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Interviews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Candidates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, ".NET" },
                    { 2, "QC" },
                    { 3, "BA" },
                    { 4, "PM" },
                    { 5, "IT" },
                    { 6, "Frontend" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_TrackId",
                table: "Interviews",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_TrackId",
                table: "Candidates",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Tracks_TrackId",
                table: "Candidates",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Interviews_Tracks_TrackId",
            //    table: "Interviews",
            //    column: "TrackId",
            //    principalTable: "Tracks",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Tracks_TrackId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Tracks_TrackId",
                table: "Interviews");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_TrackId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_TrackId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Candidates");

        }
    }
}
