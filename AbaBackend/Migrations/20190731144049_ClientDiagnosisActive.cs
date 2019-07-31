using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class ClientDiagnosisActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ClientDiagnostics",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "ClientDiagnostics");
        }
    }
}
