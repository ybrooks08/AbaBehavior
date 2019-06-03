using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class BehaviorIsPercentTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isPorcent",
                table: "ProblemBehaviors",
                newName: "IsPercent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPercent",
                table: "ProblemBehaviors",
                newName: "isPorcent");
        }
    }
}
