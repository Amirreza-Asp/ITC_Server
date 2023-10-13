using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddGoalValueToIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "To",
                table: "Indicators",
                newName: "ToDate");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "Indicators",
                newName: "FromDate");

            migrationBuilder.AddColumn<long>(
                name: "GoalValue",
                table: "Indicators",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalValue",
                table: "Indicators");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "Indicators",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "FromDate",
                table: "Indicators",
                newName: "From");
        }
    }
}
