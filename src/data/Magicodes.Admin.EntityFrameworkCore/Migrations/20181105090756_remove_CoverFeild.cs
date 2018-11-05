using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class remove_CoverFeild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cover",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "Cover",
                table: "ArticleInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Cover",
                table: "ColumnInfos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Cover",
                table: "ArticleInfos",
                nullable: true);
        }
    }
}
