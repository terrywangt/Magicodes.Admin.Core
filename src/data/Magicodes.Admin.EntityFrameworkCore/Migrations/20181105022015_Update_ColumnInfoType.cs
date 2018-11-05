using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Update_ColumnInfoType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnTypes",
                table: "ColumnInfos");

            migrationBuilder.AddColumn<int>(
                name: "ColumnType",
                table: "ColumnInfos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnType",
                table: "ColumnInfos");

            migrationBuilder.AddColumn<int>(
                name: "ColumnTypes",
                table: "ColumnInfos",
                nullable: false,
                defaultValue: 0);
        }
    }
}
