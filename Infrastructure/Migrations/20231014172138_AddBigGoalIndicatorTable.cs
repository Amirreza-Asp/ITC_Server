using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddBigGoalIndicatorTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Progress",
                table: "BigGoals");

            migrationBuilder.CreateTable(
                name: "BigGoalIndicators",
                columns: table => new
                {
                    IndicatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BigGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BigGoalIndicators", x => new { x.IndicatorId, x.BigGoalId });
                    table.ForeignKey(
                        name: "FK_BigGoalIndicators_BigGoals_BigGoalId",
                        column: x => x.BigGoalId,
                        principalTable: "BigGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BigGoalIndicators_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BigGoalIndicators_BigGoalId",
                table: "BigGoalIndicators",
                column: "BigGoalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BigGoalIndicators");

            migrationBuilder.AddColumn<short>(
                name: "Progress",
                table: "BigGoals",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
