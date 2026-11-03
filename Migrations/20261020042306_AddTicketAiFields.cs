using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuggestedReply",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 10001,
                columns: new[] { "Category", "SuggestedReply" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 10002,
                columns: new[] { "Category", "SuggestedReply" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 10003,
                columns: new[] { "Category", "SuggestedReply" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SuggestedReply",
                table: "Tickets");
        }
    }
}
