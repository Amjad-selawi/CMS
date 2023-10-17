using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class ActualExperienceNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ActualExperience",
                table: "Interviews",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "3cb3fcd2-fc7a-498e-ae78-c6806c2d9490");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "b8bfaf09-fca6-4cf4-a71a-c38491005616");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "c52f9554-777a-472c-9091-fd050e9536d5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "c59a719f-319c-40e8-a2f7-7befdcc6b429");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc692c44-b1ad-4f12-9d32-bbb99b3cfed1", "AQAAAAEAACcQAAAAEFuB7H8Nmh326nlgjq1G07V2yc+VBn5nnigd4SHy1LAm2NDgLJSqGan7KEGhkooSlQ==", "61847830-def9-4a68-b994-5f6fb3178847" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ActualExperience",
                table: "Interviews",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "4f37d089-14d6-4db7-95fd-c6fa95f76fd2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "08ba4c33-d96e-43fd-bc3a-95bde92fc6fd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "ad6307dd-e014-4d12-b622-07f4d0ca36dd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "4e26b759-c1e8-46fb-a8fb-14f0f0c0bf2c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c518178-7095-40ef-b9d0-b7f9ee15a503", "AQAAAAEAACcQAAAAELsfVkRNFVfkNthCjbnhDSMiBnc5i3fTLor5doU047dADXwep1Ee8RQaN8uQR6kxfA==", "be08cf16-5a31-4e58-9b11-2ce986e8f435" });
        }
    }
}
