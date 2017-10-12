using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Magicodes.Admin.Migrations
{
    public partial class AspNetZero_V4_1_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpRoleClaims_AbpRoles_UserId",
                table: "AbpRoleClaims");

            migrationBuilder.DropIndex(
                name: "IX_AbpRoleClaims_UserId",
                table: "AbpRoleClaims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AbpRoleClaims");

            migrationBuilder.AddColumn<bool>(
                name: "IsInTrialPeriod",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndDateUtc",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignInToken",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignInTokenExpireTimeUtc",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AbpLanguages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AbpEditions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualPrice",
                table: "AbpEditions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpiringEditionId",
                table: "AbpEditions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPrice",
                table: "AbpEditions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrialDayCount",
                table: "AbpEditions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaitingDayAfterExpire",
                table: "AbpEditions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AbpPersistedGrants",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 200, nullable: false),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 50000, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPersistedGrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DayCount = table.Column<int>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    EditionId = table.Column<int>(nullable: false),
                    Gateway = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true),
                    PaymentPeriodType = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSubscriptionPayments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CreationTime",
                table: "AbpTenants",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_SubscriptionEndDateUtc",
                table: "AbpTenants",
                column: "SubscriptionEndDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPersistedGrants_SubjectId_ClientId_Type",
                table: "AbpPersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPayments_PaymentId_Gateway",
                table: "AppSubscriptionPayments",
                columns: new[] { "PaymentId", "Gateway" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPayments_Status_CreationTime",
                table: "AppSubscriptionPayments",
                columns: new[] { "Status", "CreationTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId",
                principalTable: "AbpRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("UPDATE AbpEditions SET Discriminator = 'SubscribableEdition'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                table: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpPersistedGrants");

            migrationBuilder.DropTable(
                name: "AppSubscriptionPayments");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_CreationTime",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_SubscriptionEndDateUtc",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsInTrialPeriod",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndDateUtc",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SignInToken",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "SignInTokenExpireTimeUtc",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AbpLanguages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "AnnualPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "ExpiringEditionId",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "TrialDayCount",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "WaitingDayAfterExpire",
                table: "AbpEditions");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AbpRoleClaims",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_UserId",
                table: "AbpRoleClaims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpRoleClaims_AbpRoles_UserId",
                table: "AbpRoleClaims",
                column: "UserId",
                principalTable: "AbpRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
