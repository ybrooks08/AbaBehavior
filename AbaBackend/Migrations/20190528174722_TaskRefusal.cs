using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class TaskRefusal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPorcent",
                table: "ProblemBehaviors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ProblemBehaviors",
                keyColumn: "ProblemId",
                keyValue: 4,
                column: "isPorcent",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPorcent",
                table: "ProblemBehaviors");
        }
    }
}
