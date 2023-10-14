using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class IsupdatedToInterviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUpdated",
                table: "Interviews",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "5e29bd78-b596-4d0f-a286-3470397cc290");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "c2122c5f-2478-4ca3-b804-7eaf93002c8a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "101c6ddd-4695-4f5b-9a9e-f3e9665c951a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "26766c75-473c-4220-9a35-49f103ce39cc");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f3874ee-0d01-4898-8f43-736be7ad1eb8", "AQAAAAEAACcQAAAAEGDbFEgCkKB3jFbca4cZExkB56ilDd+Ze64lzW8OpG/U3Kx2A7p8t6PFb0V2FWSWxA==", "4cca6342-38f8-429d-b548-7da5412e4113" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUpdated",
                table: "Interviews");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "14eef19d-0043-4166-b9cd-b041dc8a66ff");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "a5927960-7ada-4f31-99da-2bfebccb8800");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "cd5b17b0-fb63-403a-9e55-20e3828bb7d6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "b1d2d287-824f-4e79-8122-020a351ea488");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6802217b-9ae5-4533-9e6d-a36578c004c9", "AQAAAAEAACcQAAAAEJvJ65+bLQKk9MIWvxLrYamKiMJk/In+ph7jecJtCdVJ9g8OTHaFHXdHybFZwkYqDg==", "ad68ac51-17e8-48f8-85ff-262027fb4779" });
        }
    }
}
