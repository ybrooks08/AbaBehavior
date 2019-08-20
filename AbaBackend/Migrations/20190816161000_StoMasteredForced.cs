using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class StoMasteredForced : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MasteredForced",
                table: "ClientReplacementSTOs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MasteredForced",
                table: "ClientProblemSTOs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasteredForced",
                table: "ClientReplacementSTOs");

            migrationBuilder.DropColumn(
                name: "MasteredForced",
                table: "ClientProblemSTOs");
        }
    }
}
