using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Added_ColumnInfo_SEO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "ColumnInfos",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyWords",
                table: "ColumnInfos",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoTitle",
                table: "ColumnInfos",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "KeyWords",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "SeoTitle",
                table: "ColumnInfos");
        }
    }
}
