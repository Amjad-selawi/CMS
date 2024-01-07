using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Domain.Migrations
{
    public partial class addAttatchmentToPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EvaluationId",
                table: "Positions",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "de8a4a88-070c-4a86-a6d0-4786c3578495");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "3c21083d-3fbd-4785-8221-350a1dd64453");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "f3813b2b-1fbe-42fa-819c-aa9a24f1c723");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "d4486aa6-e533-4ec1-af32-75c885eaa075");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4041d78f-eab6-4cc8-a256-42efa90f05d7", "AQAAAAEAACcQAAAAENnFK7l4lpbJgl/scYzV2Pm+fszzdDW7vBZ2iAGghJc8SaGW9K6QgkDa/JuvJ8Upgg==", "9d987d67-ba95-4aeb-b031-abe1bf1ec1de" });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_EvaluationId",
                table: "Positions",
                column: "EvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Attachments_EvaluationId",
                table: "Positions",
                column: "EvaluationId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Attachments_EvaluationId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_EvaluationId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "EvaluationId",
                table: "Positions");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1eecb40c-c701-4445-89d4-d1aa7d70460d",
                column: "ConcurrencyStamp",
                value: "0c10956a-1aaa-425f-ae92-3f8c27e2d9be");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "226cca69-f046-4d15-8b81-9b9ba34f2214",
                column: "ConcurrencyStamp",
                value: "b8297c47-c467-43a3-89b9-830cd1253a37");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91c3461a-7da3-4033-b907-b104b903d793",
                column: "ConcurrencyStamp",
                value: "decfba96-1c6e-4f58-9682-c61c17bfd1c4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b024cbbe-f64e-4d1b-9c6e-05ac0f0e3ebb",
                column: "ConcurrencyStamp",
                value: "e1a2e464-8213-4749-b174-cea71ded44c2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c6585ab9-8b5f-4332-a174-92429db8add2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7c2d6ce3-e6ac-497f-941b-8ae7bef85afd", "AQAAAAEAACcQAAAAEMEF0/F9QayDnXRfDNHSxVTitqMECe+IaysZI1KA8NqIk1VwzMRY0rxIHjFmJS6SRg==", "85dbeb4f-db24-42dc-b60e-f2dc27bceb00" });
        }
    }
}
