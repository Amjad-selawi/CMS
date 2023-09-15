using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class ssm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "CarrerOffers");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Interviews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceCountryId",
                table: "Interviews",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "CarrerOffers",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "CarrerOffers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "CarrerOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "Candidates",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb", "499c99e3-6d06-4571-9e04-1467b86d7a90", "Admin", "ADMIN" },
                    { "1eecb40c-c701-4445-89d4-d1aa7d70460d", "efb814dc-ce2e-4544-be42-58753edb47e2", "General Manager", "GENERAL MANAGER" },
                    { "226cca69-f046-4d15-8b81-9b9ba34f2214", "ee7cdba7-2174-4736-8dc3-1ad9faa7f691", "HR Manager", "HR MANAGER" },
                    { "91c3461a-7da3-4033-b907-b104b903d793", "afa32351-9d26-48c3-b824-f7b184734e39", "Interviewer", "INTERVIEWER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c6585ab9-8b5f-4332-a174-92429db8add2", 0, "5f1cc9a6-3c85-4821-b804-b331e90c75a4", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAEAACcQAAAAEFn3tyiZHnmOpjwbg49AIoO1n9gT95NEHT2I+SMFf+1YBeU0SHDaY/opJiM2y5+a1w==", null, false, "166484ea-15a5-4859-9510-8fe68c10ce42", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "c6585ab9-8b5f-4332-a174-92429db8add2", "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb" });

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_SourceCountryId",
                table: "Interviews",
                column: "SourceCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrerOffers_CreatorId",
                table: "CarrerOffers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrerOffers_PositionId",
                table: "CarrerOffers",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_PositionId",
                table: "Candidates",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Positions_PositionId",
                table: "Candidates",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarrerOffers_AspNetUsers_CreatorId",
                table: "CarrerOffers",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarrerOffers_Positions_PositionId",
                table: "CarrerOffers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Countries_SourceCountryId",
                table: "Interviews",
                column: "SourceCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Positions_PositionId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_CarrerOffers_AspNetUsers_CreatorId",
                table: "CarrerOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_CarrerOffers_Positions_PositionId",
                table: "CarrerOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Countries_SourceCountryId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_SourceCountryId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_CarrerOffers_CreatorId",
                table: "CarrerOffers");

            migrationBuilder.DropIndex(
                name: "IX_CarrerOffers_PositionId",
                table: "CarrerOffers");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_PositionId",
                table: "Candidates");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "c6585ab9-8b5f-4332-a174-92429db8add2", "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "SourceCountryId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "CarrerOffers");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "CarrerOffers");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Candidates");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CarrerOffers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "CarrerOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
