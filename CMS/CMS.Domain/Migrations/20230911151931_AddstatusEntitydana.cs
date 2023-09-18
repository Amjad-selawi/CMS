using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class AddstatusEntitydana : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Interviews");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Interviews",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Interviews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_StatusId",
                table: "Interviews",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Statuses_StatusId",
                table: "Interviews",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Statuses_StatusId",
                table: "Interviews");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_StatusId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Interviews");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Interviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
