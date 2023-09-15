using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class candidateCountryRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Candidates",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_Candidates_CountryId",
                table: "Candidates",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Countries_CountryId",
                table: "Candidates",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Countries_CountryId",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_CountryId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Candidates");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "efb814dc-ce2e-4544-be42-58753edb47e2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "ee7cdba7-2174-4736-8dc3-1ad9faa7f691");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "afa32351-9d26-48c3-b824-f7b184734e39");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "499c99e3-6d06-4571-9e04-1467b86d7a90");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f1cc9a6-3c85-4821-b804-b331e90c75a4", "AQAAAAEAACcQAAAAEFn3tyiZHnmOpjwbg49AIoO1n9gT95NEHT2I+SMFf+1YBeU0SHDaY/opJiM2y5+a1w==", "166484ea-15a5-4859-9510-8fe68c10ce42" });
        }
    }
}
