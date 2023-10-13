using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddIndicatorCategoryAndIndicatorTypeToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersJoinRequests_Company_CompanyId",
                table: "UsersJoinRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "UsersJoinRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "IndicatorCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorCategories_IndicatorCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "IndicatorCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IndicatorTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorCategories_ParentId",
                table: "IndicatorCategories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersJoinRequests_Company_CompanyId",
                table: "UsersJoinRequests",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersJoinRequests_Company_CompanyId",
                table: "UsersJoinRequests");

            migrationBuilder.DropTable(
                name: "IndicatorCategories");

            migrationBuilder.DropTable(
                name: "IndicatorTypes");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "UsersJoinRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersJoinRequests_Company_CompanyId",
                table: "UsersJoinRequests",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
