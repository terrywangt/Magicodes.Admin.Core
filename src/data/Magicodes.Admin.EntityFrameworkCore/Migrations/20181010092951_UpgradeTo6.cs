using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class UpgradeTo6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "AbpUserTokens",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "AbpUserTokens");
        }
    }
}
