using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class SessionAnalyst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionAnalystId",
                table: "Sessions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionAnalystId",
                table: "Sessions",
                column: "SessionAnalystId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_SessionAnalystId",
                table: "Sessions",
                column: "SessionAnalystId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_SessionAnalystId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_SessionAnalystId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SessionAnalystId",
                table: "Sessions");
        }
    }
}
