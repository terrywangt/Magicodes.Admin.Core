using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class updateTableInvoiceAddField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "AppInvoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "AppInvoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "AppInvoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bank",
                table: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "Contact",
                table: "AppInvoices");
        }
    }
}
