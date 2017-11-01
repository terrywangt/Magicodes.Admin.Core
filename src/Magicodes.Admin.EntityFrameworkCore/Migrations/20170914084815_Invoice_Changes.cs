using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Magicodes.Admin.Migrations
{
    public partial class Invoice_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "AppSubscriptionPayments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvoiceDate = table.Column<DateTime>(nullable: false),
                    InvoiceNo = table.Column<string>(nullable: true),
                    TenantAddress = table.Column<string>(nullable: true),
                    TenantLegalName = table.Column<string>(nullable: true),
                    TenantTaxNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInvoices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInvoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "AppSubscriptionPayments");
        }
    }
}
