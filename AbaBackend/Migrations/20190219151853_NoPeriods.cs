using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class NoPeriods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientsProblems_Clients_ClientId",
                table: "ClientsProblems");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsProblems_ProblemBehaviors_ProblemId",
                table: "ClientsProblems");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsReplacements_Clients_ClientId",
                table: "ClientsReplacements");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsReplacements_ReplacementPrograms_ReplacementId",
                table: "ClientsReplacements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientsReplacements",
                table: "ClientsReplacements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientsProblems",
                table: "ClientsProblems");

            migrationBuilder.RenameTable(
                name: "ClientsReplacements",
                newName: "ClientReplacement");

            migrationBuilder.RenameTable(
                name: "ClientsProblems",
                newName: "ClientProblem");

            migrationBuilder.RenameIndex(
                name: "IX_ClientsReplacements_ReplacementId",
                table: "ClientReplacement",
                newName: "IX_ClientReplacement_ReplacementId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientsProblems_ProblemId",
                table: "ClientProblem",
                newName: "IX_ClientProblem_ProblemId");

            migrationBuilder.AddColumn<int>(
                name: "ClientReplacementId",
                table: "ClientReplacement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClientProblemId",
                table: "ClientProblem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientReplacement",
                table: "ClientReplacement",
                columns: new[] { "ClientId", "ReplacementId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientProblem",
                table: "ClientProblem",
                columns: new[] { "ClientId", "ProblemId" });

            migrationBuilder.CreateTable(
                name: "ClientProblemSTOs",
                columns: table => new
                {
                    ClientProblemStoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientProblemId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Weeks = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    WeekStart = table.Column<DateTime>(nullable: false),
                    WeekEnd = table.Column<DateTime>(nullable: false),
                    ClientProblemClientId = table.Column<int>(nullable: true),
                    ClientProblemProblemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProblemSTOs", x => x.ClientProblemStoId);
                    table.ForeignKey(
                        name: "FK_ClientProblemSTOs_ClientProblem_ClientProblemClientId_ClientProblemProblemId",
                        columns: x => new { x.ClientProblemClientId, x.ClientProblemProblemId },
                        principalTable: "ClientProblem",
                        principalColumns: new[] { "ClientId", "ProblemId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientReplacementSTOs",
                columns: table => new
                {
                    ClientReplacementStoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientReplacementId = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false),
                    Weeks = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    WeekStart = table.Column<DateTime>(nullable: false),
                    WeekEnd = table.Column<DateTime>(nullable: false),
                    ClientReplacementClientId = table.Column<int>(nullable: true),
                    ClientReplacementReplacementId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientReplacementSTOs", x => x.ClientReplacementStoId);
                    table.ForeignKey(
                        name: "FK_ClientReplacementSTOs_ClientReplacement_ClientReplacementClientId_ClientReplacementReplacementId",
                        columns: x => new { x.ClientReplacementClientId, x.ClientReplacementReplacementId },
                        principalTable: "ClientReplacement",
                        principalColumns: new[] { "ClientId", "ReplacementId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProblemSTOs_ClientProblemClientId_ClientProblemProblemId",
                table: "ClientProblemSTOs",
                columns: new[] { "ClientProblemClientId", "ClientProblemProblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientReplacementSTOs_ClientReplacementClientId_ClientReplacementReplacementId",
                table: "ClientReplacementSTOs",
                columns: new[] { "ClientReplacementClientId", "ClientReplacementReplacementId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClientProblem_ProblemBehaviors_ProblemId",
                table: "ClientProblem",
                column: "ProblemId",
                principalTable: "ProblemBehaviors",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReplacement_ReplacementPrograms_ReplacementId",
                table: "ClientReplacement",
                column: "ReplacementId",
                principalTable: "ReplacementPrograms",
                principalColumn: "ReplacementId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientProblem_ProblemBehaviors_ProblemId",
                table: "ClientProblem");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientReplacement_ReplacementPrograms_ReplacementId",
                table: "ClientReplacement");

            migrationBuilder.DropTable(
                name: "ClientProblemSTOs");

            migrationBuilder.DropTable(
                name: "ClientReplacementSTOs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientReplacement",
                table: "ClientReplacement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientProblem",
                table: "ClientProblem");

            migrationBuilder.DropColumn(
                name: "ClientReplacementId",
                table: "ClientReplacement");

            migrationBuilder.DropColumn(
                name: "ClientProblemId",
                table: "ClientProblem");

            migrationBuilder.RenameTable(
                name: "ClientReplacement",
                newName: "ClientsReplacements");

            migrationBuilder.RenameTable(
                name: "ClientProblem",
                newName: "ClientsProblems");

            migrationBuilder.RenameIndex(
                name: "IX_ClientReplacement_ReplacementId",
                table: "ClientsReplacements",
                newName: "IX_ClientsReplacements_ReplacementId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientProblem_ProblemId",
                table: "ClientsProblems",
                newName: "IX_ClientsProblems_ProblemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientsReplacements",
                table: "ClientsReplacements",
                columns: new[] { "ClientId", "ReplacementId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientsProblems",
                table: "ClientsProblems",
                columns: new[] { "ClientId", "ProblemId" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                columns: new[] { "Hash", "Salt" },
                values: new object[] { new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 } });

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsProblems_Clients_ClientId",
                table: "ClientsProblems",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsProblems_ProblemBehaviors_ProblemId",
                table: "ClientsProblems",
                column: "ProblemId",
                principalTable: "ProblemBehaviors",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsReplacements_Clients_ClientId",
                table: "ClientsReplacements",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsReplacements_ReplacementPrograms_ReplacementId",
                table: "ClientsReplacements",
                column: "ReplacementId",
                principalTable: "ReplacementPrograms",
                principalColumn: "ReplacementId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
