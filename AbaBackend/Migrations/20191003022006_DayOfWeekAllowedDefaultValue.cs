using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class DayOfWeekAllowedDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SessionsDateAllowed",
                table: "Users",
                nullable: false,
                defaultValue: 62,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SessionsDateAllowed",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 62);
        }
    }
}
