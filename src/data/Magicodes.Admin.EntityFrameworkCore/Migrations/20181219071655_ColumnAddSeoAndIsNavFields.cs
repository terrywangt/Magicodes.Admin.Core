using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class ColumnAddSeoAndIsNavFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNav",
                table: "ColumnInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StaticPageUrl",
                table: "ColumnInfos",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNav",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "StaticPageUrl",
                table: "ColumnInfos");
        }
    }
}
