using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class modelscorrections1outof2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Countries_SourceCountryId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_SourceCountryId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "SourceCountryId",
                table: "Interviews");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Interviews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "84d1019a-fdaf-412c-93b4-2cf39a3b65ac");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "998a7451-a879-4683-8592-8bda52bf4778");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "d24d732a-367d-4813-ba4a-93a6839d04ef");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "584e60d7-72de-4ec4-97b2-cb90aba06cf5");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a86f0066-ff69-44f8-befc-5ba448d76544", "AQAAAAEAACcQAAAAEEc4RYI1sykd2A6fTL/gcbqmVJddGIat9Pl9+NuA5BzDh1vAMsZiOxTnS1H8/aJf2g==", "848c423a-6bd5-4e8c-8c3c-17d7a9165e7b" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Interviews");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Interviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceCountryId",
                table: "Interviews",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "235fbd96-0bef-4704-b9b9-0d7c70f55ff8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "0336d0e9-611a-41be-bc15-7d1dd212c294");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "a03df5b1-b928-476e-b782-ed4bbe19e1df");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "a16ad11d-ce36-46dd-91fd-f8ceebedc9ba");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "514871e0-20c6-443c-a9cf-43e9fa82eb0c", "AQAAAAEAACcQAAAAEAGsZR1/LL9PAx84TFX+vOqvKENYU6MU06vTaBe4Bdpw1QDNzhFs3WH3fb4WgbXjBQ==", "ce4a225f-7a32-4c5e-bfc2-fdb62570a50b" });

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_SourceCountryId",
                table: "Interviews",
                column: "SourceCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Countries_SourceCountryId",
                table: "Interviews",
                column: "SourceCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
