using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class ColumnInfoAddIsFooterNavField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsNav",
                table: "ColumnInfos",
                newName: "IsHeaderNav");

            migrationBuilder.AddColumn<bool>(
                name: "IsFooterNav",
                table: "ColumnInfos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFooterNav",
                table: "ColumnInfos");

            migrationBuilder.RenameColumn(
                name: "IsHeaderNav",
                table: "ColumnInfos",
                newName: "IsNav");
        }
    }
}
