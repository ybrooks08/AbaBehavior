using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class DriveTimePayRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DriveTimePayRate",
                table: "Users",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveTimePayRate",
                table: "Users");
        }
    }
}
