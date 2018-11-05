using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Update_ColumnInfoAndArticleInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ColumnInfos",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ColumnTypes",
                table: "ColumnInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxItemCount",
                table: "ColumnInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ArticleInfos",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "ColumnTypes",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "MaxItemCount",
                table: "ColumnInfos");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ArticleInfos");
        }
    }
}
