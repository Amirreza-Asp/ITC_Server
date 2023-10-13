using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indicators_IndicatorCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "IndicatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Indicators_IndicatorTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "IndicatorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationalObjectiveIndicators",
                columns: table => new
                {
                    OperationalObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IndicatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalObjectiveIndicators", x => new { x.IndicatorId, x.OperationalObjectiveId });
                    table.ForeignKey(
                        name: "FK_OperationalObjectiveIndicators_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationalObjectiveIndicators_OperationalObjectives_OperationalObjectiveId",
                        column: x => x.OperationalObjectiveId,
                        principalTable: "OperationalObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectIndicators",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IndicatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectIndicators", x => new { x.IndicatorId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectIndicators_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectIndicators_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_CategoryId",
                table: "Indicators",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_TypeId",
                table: "Indicators",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalObjectiveIndicators_OperationalObjectiveId",
                table: "OperationalObjectiveIndicators",
                column: "OperationalObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectIndicators_ProjectId",
                table: "ProjectIndicators",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationalObjectiveIndicators");

            migrationBuilder.DropTable(
                name: "ProjectIndicators");

            migrationBuilder.DropTable(
                name: "Indicators");
        }
    }
}
