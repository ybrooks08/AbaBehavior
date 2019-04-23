using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class SessionSigns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionSign_Sessions_SessionId",
                table: "SessionSign");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionSign",
                table: "SessionSign");

            migrationBuilder.RenameTable(
                name: "SessionSign",
                newName: "SessionSigns");

            migrationBuilder.RenameIndex(
                name: "IX_SessionSign_SessionId",
                table: "SessionSigns",
                newName: "IX_SessionSigns_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionSigns",
                table: "SessionSigns",
                column: "SessionSignId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionSigns_Sessions_SessionId",
                table: "SessionSigns",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionSigns_Sessions_SessionId",
                table: "SessionSigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionSigns",
                table: "SessionSigns");

            migrationBuilder.RenameTable(
                name: "SessionSigns",
                newName: "SessionSign");

            migrationBuilder.RenameIndex(
                name: "IX_SessionSigns_SessionId",
                table: "SessionSign",
                newName: "IX_SessionSign_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionSign",
                table: "SessionSign",
                column: "SessionSignId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionSign_Sessions_SessionId",
                table: "SessionSign",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
