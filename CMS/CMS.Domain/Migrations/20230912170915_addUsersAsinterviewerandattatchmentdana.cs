using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class addUsersAsinterviewerandattatchmentdana : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InterviewerId",
                table: "Interviews",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AttachmentId",
                table: "Interviews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_AttachmentId",
                table: "Interviews",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewerId",
                table: "Interviews",
                column: "InterviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Attachments",
                table: "Interviews",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_AspNetUsers_InterviewerId",
                table: "Interviews",
                column: "InterviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Attachments",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_AspNetUsers_InterviewerId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_AttachmentId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_InterviewerId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "InterviewerId",
                table: "Interviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
