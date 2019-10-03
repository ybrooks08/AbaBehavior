using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class CompetencyCheckTypos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompetencyCheckParams",
                keyColumn: "CompetencyCheckParamId",
                keyValue: 2,
                column: "Description",
                value: "Followed recommended intervention procedures upon occurrence of problem behavior in session");

            migrationBuilder.UpdateData(
                table: "CompetencyCheckParams",
                keyColumn: "CompetencyCheckParamId",
                keyValue: 4,
                column: "Description",
                value: "Implemented the acquisition skills training programs and recorded the data");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompetencyCheckParams",
                keyColumn: "CompetencyCheckParamId",
                keyValue: 2,
                column: "Description",
                value: "Followed recommended intervention procedures upon occurrence of problem behaviore in session");

            migrationBuilder.UpdateData(
                table: "CompetencyCheckParams",
                keyColumn: "CompetencyCheckParamId",
                keyValue: 4,
                column: "Description",
                value: "Implemented the acquisition skills training proramas and recorded the data");
        }
    }
}
