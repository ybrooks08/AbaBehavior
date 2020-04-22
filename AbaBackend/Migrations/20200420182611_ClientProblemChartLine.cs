using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class ClientProblemChartLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientProblemChartLines",
                columns: table => new
                {
                    ClientProblemChartLineId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientProblemId = table.Column<int>(nullable: false),
                    ChartDate = table.Column<DateTime>(nullable: false),
                    ChartLabel = table.Column<string>(nullable: true),
                    ChartOrientation = table.Column<string>(nullable: true),
                    ChartLineStyle = table.Column<string>(nullable: true),
                    ChartLineColor = table.Column<string>(nullable: true),
                    ChartLabelFontSize = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProblemChartLines", x => x.ClientProblemChartLineId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientProblemChartLines");
        }
    }
}
