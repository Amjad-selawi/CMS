using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class fixcarreroffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrerOffers_AspNetUsers_CreatorId",
                table: "CarrerOffers");

            migrationBuilder.DropIndex(
                name: "IX_CarrerOffers_CreatorId",
                table: "CarrerOffers");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "CarrerOffers");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CarrerOffers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "bade4b7d-0f7d-410e-a084-3aa0a758491c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "3ecc2a5e-b3c8-4876-b379-7923e971b702");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "77e95ae5-b216-474f-8f88-26608eda51cf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "ded42a35-78b3-45a8-a670-e1f3a076e682");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8a1244a2-8679-425f-b717-e4ed6b009968", "AQAAAAEAACcQAAAAEC2bTBRX/l3wAs3D5/OnLKtFDqmW40k2SmHEeJOsGgGSvTsoeds6wOj9vhqMeB45ig==", "396ce0cf-8db3-4459-b27a-9c7f7d9acbd3" });

            migrationBuilder.CreateIndex(
                name: "IX_CarrerOffers_CreatedBy",
                table: "CarrerOffers",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrerOffers_AspNetUsers_CreatedBy",
                table: "CarrerOffers",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrerOffers_AspNetUsers_CreatedBy",
                table: "CarrerOffers");

            migrationBuilder.DropIndex(
                name: "IX_CarrerOffers_CreatedBy",
                table: "CarrerOffers");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "CarrerOffers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "CarrerOffers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "746a7f29-4f73-4a76-9702-e65b1eb7f76a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "25bdf6be-921e-4991-93b7-e397f0bef7ea");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "61050574-0f22-4fe3-b841-8739547c67ce");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "57749260-16a4-4c21-ba27-d1c377a54dc5");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffe418a7-39ba-4400-b1e1-937e6f325f76", "AQAAAAEAACcQAAAAEAPiRBBmOYEc4HhVOyxoc3KkFO8W0UefR6XKRUWLfhWckoCpBMjvSnIk+iThljjULQ==", "cc963135-5106-4a7c-a4bf-0bf82b7e7290" });

            migrationBuilder.CreateIndex(
                name: "IX_CarrerOffers_CreatorId",
                table: "CarrerOffers",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrerOffers_AspNetUsers_CreatorId",
                table: "CarrerOffers",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
