using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class SessionDriveTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DriveTime",
                table: "Sessions",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveTime",
                table: "Sessions");
        }
    }
}
