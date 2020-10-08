using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class supervision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Supervision1",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Supervision2",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Supervision3",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Supervision4",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Supervision5",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Supervision6",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Supervision7",
                table: "SessionNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SupervisionOther",
                table: "SessionNotes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supervision1",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Supervision2",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Supervision3",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Supervision4",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Supervision5",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Supervision6",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Supervision7",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "SupervisionOther",
                table: "SessionNotes");
        }
    }
}
