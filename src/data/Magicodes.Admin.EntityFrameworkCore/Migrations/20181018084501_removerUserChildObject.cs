using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class removerUserChildObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Wallet_Balance",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Wallet_FrozenAmount",
                table: "AbpUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Wallet_Balance",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wallet_FrozenAmount",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
