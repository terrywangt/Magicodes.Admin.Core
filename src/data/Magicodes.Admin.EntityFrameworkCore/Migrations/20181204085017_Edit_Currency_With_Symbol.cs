using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Edit_Currency_With_Symbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency_CultureName",
                table: "TransactionLogs");

            migrationBuilder.AddColumn<string>(
                name: "Currency_Symbol",
                table: "TransactionLogs",
                maxLength: 5,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency_Symbol",
                table: "TransactionLogs");

            migrationBuilder.AddColumn<string>(
                name: "Currency_CultureName",
                table: "TransactionLogs",
                maxLength: 10,
                nullable: true);
        }
    }
}
