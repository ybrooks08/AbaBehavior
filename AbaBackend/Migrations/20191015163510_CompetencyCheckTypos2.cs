using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class CompetencyCheckTypos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompetencyCheckParams",
                keyColumn: "CompetencyCheckParamId",
                keyValue: 10,
                column: "Description",
                value: "Followed recommended intervention procedures upon occurrence of problem behavior in session");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompetencyCheckParams",
                keyColumn: "CompetencyCheckParamId",
                keyValue: 10,
                column: "Description",
                value: "Followed recommended intervention procedures upon occurrence of problem behaviore in session");
        }
    }
}
