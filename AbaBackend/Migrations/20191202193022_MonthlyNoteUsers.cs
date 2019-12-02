using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class MonthlyNoteUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyAnalystId",
                table: "MonthlyNotes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyAssistantId",
                table: "MonthlyNotes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyRbtId",
                table: "MonthlyNotes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyNotes_MonthlyAnalystId",
                table: "MonthlyNotes",
                column: "MonthlyAnalystId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyNotes_MonthlyAssistantId",
                table: "MonthlyNotes",
                column: "MonthlyAssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyNotes_MonthlyRbtId",
                table: "MonthlyNotes",
                column: "MonthlyRbtId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyNotes_Users_MonthlyAnalystId",
                table: "MonthlyNotes",
                column: "MonthlyAnalystId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyNotes_Users_MonthlyAssistantId",
                table: "MonthlyNotes",
                column: "MonthlyAssistantId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyNotes_Users_MonthlyRbtId",
                table: "MonthlyNotes",
                column: "MonthlyRbtId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyNotes_Users_MonthlyAnalystId",
                table: "MonthlyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyNotes_Users_MonthlyAssistantId",
                table: "MonthlyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyNotes_Users_MonthlyRbtId",
                table: "MonthlyNotes");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyNotes_MonthlyAnalystId",
                table: "MonthlyNotes");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyNotes_MonthlyAssistantId",
                table: "MonthlyNotes");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyNotes_MonthlyRbtId",
                table: "MonthlyNotes");

            migrationBuilder.DropColumn(
                name: "MonthlyAnalystId",
                table: "MonthlyNotes");

            migrationBuilder.DropColumn(
                name: "MonthlyAssistantId",
                table: "MonthlyNotes");

            migrationBuilder.DropColumn(
                name: "MonthlyRbtId",
                table: "MonthlyNotes");
        }
    }
}
