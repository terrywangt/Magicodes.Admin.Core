using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Added_TransactionLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ClientIpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(maxLength: 128, nullable: true),
                    IsFreeze = table.Column<bool>(nullable: false),
                    PayChannel = table.Column<int>(nullable: false),
                    Terminal = table.Column<int>(nullable: false),
                    TransactionState = table.Column<int>(nullable: false),
                    CustomData = table.Column<string>(maxLength: 500, nullable: true),
                    OutTradeNo = table.Column<string>(maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(maxLength: 50, nullable: true),
                    PayTime = table.Column<DateTime>(nullable: true),
                    Exception = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionLogs");
        }
    }
}
