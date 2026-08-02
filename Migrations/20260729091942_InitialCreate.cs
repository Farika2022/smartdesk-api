using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartDesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Customer = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Urgency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "CreatedAt", "Customer", "Email", "Status", "Subject", "Urgency" },
                values: new object[,]
                {
                    { 10001, new DateTime(2026, 7, 10, 8, 23, 0, 0, DateTimeKind.Utc), "Lars Nielsen", "lars@example.com", "open", "Front wheel is making a grinding noise", "HIGH" },
                    { 10002, new DateTime(2026, 7, 11, 10, 5, 0, 0, DateTimeKind.Utc), "Sofia Berg", "sofia@example.com", "open", "Invoice shows wrong VAT amount", "LOW" },
                    { 10003, new DateTime(2026, 7, 12, 14, 30, 0, 0, DateTimeKind.Utc), "Mikkel Holm", "mikkel@example.com", "resolved", "Battery not charging past 40 percent", "MEDIUM" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
