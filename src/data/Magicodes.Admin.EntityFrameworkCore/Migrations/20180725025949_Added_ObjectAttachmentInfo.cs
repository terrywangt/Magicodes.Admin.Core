using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class Added_ObjectAttachmentInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectAttachmentInfos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ObjectId = table.Column<long>(nullable: false),
                    AttachmentInfoId = table.Column<long>(nullable: false),
                    ObjectType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectAttachmentInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectAttachmentInfos_AttachmentInfos_AttachmentInfoId",
                        column: x => x.AttachmentInfoId,
                        principalTable: "AttachmentInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectAttachmentInfos_AttachmentInfoId",
                table: "ObjectAttachmentInfos",
                column: "AttachmentInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectAttachmentInfos");
        }
    }
}
