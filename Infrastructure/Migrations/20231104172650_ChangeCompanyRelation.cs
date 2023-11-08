using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ChangeCompanyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "SingleWindowUrl",
                table: "Company",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ProvinceName",
                table: "Company",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "NameUniversity",
                table: "Company",
                newName: "Logo");

            migrationBuilder.RenameColumn(
                name: "LogoUniversity",
                table: "Company",
                newName: "City");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Company",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_ParentId",
                table: "Company",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Company_ParentId",
                table: "Company",
                column: "ParentId",
                principalTable: "Company",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Company_ParentId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_ParentId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Company",
                newName: "SingleWindowUrl");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Company",
                newName: "ProvinceName");

            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "Company",
                newName: "NameUniversity");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Company",
                newName: "LogoUniversity");

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
