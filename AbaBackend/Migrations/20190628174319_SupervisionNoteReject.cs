using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
  public partial class SupervisionNoteReject : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<string>(
        name: "RejectNotes",
        table: "SessionSupervisionNotes",
        nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
        name: "RejectNotes",
        table: "SessionSupervisionNotes");
    }
  }
}