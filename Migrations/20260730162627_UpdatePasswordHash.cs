using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2b$12$A.3RKBS0s/IlMceFJOGoPOdxdwPhbM.RFs7yHfuXJKFQE1jYmRkgu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$rBnNmMDDqMKRRSSgLBMxuuWfhGXEzGPXcFJQjhIlkVpkHqIQJE5Zy");
        }
    }
}
