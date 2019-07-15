using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
  public partial class remove_default_active : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<bool>(
        name: "Active",
        table: "ClientReplacements",
        nullable: false,
        oldClrType: typeof(bool),
        oldDefaultValue: true);

      migrationBuilder.AlterColumn<bool>(
        name: "Active",
        table: "ClientProblems",
        nullable: false,
        oldClrType: typeof(bool),
        oldDefaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<bool>(
        name: "Active",
        table: "ClientReplacements",
        nullable: false,
        defaultValue: true,
        oldClrType: typeof(bool));

      migrationBuilder.AlterColumn<bool>(
        name: "Active",
        table: "ClientProblems",
        nullable: false,
        defaultValue: true,
        oldClrType: typeof(bool));
    }
  }
}