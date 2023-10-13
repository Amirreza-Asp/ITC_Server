using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddPracticalActionIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PracticalActionIndicators",
                columns: table => new
                {
                    PracticalActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IndicatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalActionIndicators", x => new { x.IndicatorId, x.PracticalActionId });
                    table.ForeignKey(
                        name: "FK_PracticalActionIndicators_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticalActionIndicators_PracticalActions_PracticalActionId",
                        column: x => x.PracticalActionId,
                        principalTable: "PracticalActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticalActionIndicators_PracticalActionId",
                table: "PracticalActionIndicators",
                column: "PracticalActionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticalActionIndicators");
        }
    }
}
