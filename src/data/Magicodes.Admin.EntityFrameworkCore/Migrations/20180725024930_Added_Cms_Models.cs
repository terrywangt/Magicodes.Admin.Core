using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Added_Cms_Models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleSourceInfos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSourceInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColumnInfos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    SortNo = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Introduction = table.Column<string>(maxLength: 200, nullable: true),
                    ParentId = table.Column<long>(nullable: false),
                    IsNeedAuthorizeAccess = table.Column<bool>(nullable: false),
                    IconCls = table.Column<string>(maxLength: 20, nullable: true),
                    Cover = table.Column<long>(nullable: true),
                    Url = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleInfos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Publisher = table.Column<string>(maxLength: 20, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsNeedAuthorizeAccess = table.Column<bool>(nullable: false),
                    ColumnInfoId = table.Column<long>(nullable: false),
                    ArticleSourceInfoId = table.Column<long>(nullable: true),
                    ReleaseTime = table.Column<DateTime>(nullable: true),
                    SeoTitle = table.Column<string>(maxLength: 50, nullable: true),
                    KeyWords = table.Column<string>(maxLength: 200, nullable: true),
                    Introduction = table.Column<string>(maxLength: 200, nullable: true),
                    StaticPageUrl = table.Column<string>(maxLength: 200, nullable: true),
                    Cover = table.Column<long>(nullable: true),
                    Url = table.Column<string>(maxLength: 255, nullable: true),
                    RecommendedType = table.Column<int>(nullable: false),
                    ViewCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleInfos_ArticleSourceInfos_ArticleSourceInfoId",
                        column: x => x.ArticleSourceInfoId,
                        principalTable: "ArticleSourceInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticleInfos_ColumnInfos_ColumnInfoId",
                        column: x => x.ColumnInfoId,
                        principalTable: "ColumnInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTagInfos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ArticleInfoId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTagInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleTagInfos_ArticleInfos_ArticleInfoId",
                        column: x => x.ArticleInfoId,
                        principalTable: "ArticleInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleInfos_ArticleSourceInfoId",
                table: "ArticleInfos",
                column: "ArticleSourceInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleInfos_ColumnInfoId",
                table: "ArticleInfos",
                column: "ColumnInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTagInfos_ArticleInfoId",
                table: "ArticleTagInfos",
                column: "ArticleInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTagInfos");

            migrationBuilder.DropTable(
                name: "ArticleInfos");

            migrationBuilder.DropTable(
                name: "ArticleSourceInfos");

            migrationBuilder.DropTable(
                name: "ColumnInfos");
        }
    }
}
