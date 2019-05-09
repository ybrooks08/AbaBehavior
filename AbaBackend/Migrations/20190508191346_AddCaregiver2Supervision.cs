using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class AddCaregiver2Supervision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaregiverId",
                table: "SessionSupervisionNotes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaregiverNote",
                table: "SessionSupervisionNotes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionSupervisionNotes_CaregiverId",
                table: "SessionSupervisionNotes",
                column: "CaregiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionSupervisionNotes_Caregivers_CaregiverId",
                table: "SessionSupervisionNotes",
                column: "CaregiverId",
                principalTable: "Caregivers",
                principalColumn: "CaregiverId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionSupervisionNotes_Caregivers_CaregiverId",
                table: "SessionSupervisionNotes");

            migrationBuilder.DropIndex(
                name: "IX_SessionSupervisionNotes_CaregiverId",
                table: "SessionSupervisionNotes");

            migrationBuilder.DropColumn(
                name: "CaregiverId",
                table: "SessionSupervisionNotes");

            migrationBuilder.DropColumn(
                name: "CaregiverNote",
                table: "SessionSupervisionNotes");
        }
    }
}
