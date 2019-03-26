using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class FixClientProblems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientProblemSTOs_ClientProblem_ClientProblemClientId_ClientProblemProblemId",
                table: "ClientProblemSTOs");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientReplacementSTOs_ClientReplacement_ClientReplacementClientId_ClientReplacementReplacementId",
                table: "ClientReplacementSTOs");

            migrationBuilder.DropTable(
                name: "ClientProblem");

            migrationBuilder.DropTable(
                name: "ClientReplacement");

            migrationBuilder.DropIndex(
                name: "IX_ClientReplacementSTOs_ClientReplacementClientId_ClientReplacementReplacementId",
                table: "ClientReplacementSTOs");

            migrationBuilder.DropIndex(
                name: "IX_ClientProblemSTOs_ClientProblemClientId_ClientProblemProblemId",
                table: "ClientProblemSTOs");

            migrationBuilder.DropColumn(
                name: "ClientReplacementClientId",
                table: "ClientReplacementSTOs");

            migrationBuilder.DropColumn(
                name: "ClientProblemClientId",
                table: "ClientProblemSTOs");

            migrationBuilder.RenameColumn(
                name: "ClientReplacementReplacementId",
                table: "ClientReplacementSTOs",
                newName: "ClientReplacement2Id");

            migrationBuilder.RenameColumn(
                name: "ClientProblemProblemId",
                table: "ClientProblemSTOs",
                newName: "ClientProblem2Id");

            migrationBuilder.CreateTable(
                name: "ClientProblems2",
                columns: table => new
                {
                    ClientProblem2Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    ProblemId = table.Column<int>(nullable: false),
                    BaselineFrom = table.Column<DateTime>(nullable: true),
                    BaselineTo = table.Column<DateTime>(nullable: true),
                    BaselineCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProblems2", x => x.ClientProblem2Id);
                    table.ForeignKey(
                        name: "FK_ClientProblems2_ProblemBehaviors_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "ProblemBehaviors",
                        principalColumn: "ProblemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientReplacements2",
                columns: table => new
                {
                    ClientReplacement2Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    ReplacementId = table.Column<int>(nullable: false),
                    BaselineFrom = table.Column<DateTime>(nullable: true),
                    BaselineTo = table.Column<DateTime>(nullable: true),
                    BaselinePercent = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientReplacements2", x => x.ClientReplacement2Id);
                    table.ForeignKey(
                        name: "FK_ClientReplacements2_ReplacementPrograms_ReplacementId",
                        column: x => x.ReplacementId,
                        principalTable: "ReplacementPrograms",
                        principalColumn: "ReplacementId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ClientReplacementSTOs_ClientReplacement2Id",
                table: "ClientReplacementSTOs",
                column: "ClientReplacement2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProblemSTOs_ClientProblem2Id",
                table: "ClientProblemSTOs",
                column: "ClientProblem2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProblems2_ProblemId",
                table: "ClientProblems2",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientReplacements2_ReplacementId",
                table: "ClientReplacements2",
                column: "ReplacementId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientProblemSTOs_ClientProblems2_ClientProblem2Id",
                table: "ClientProblemSTOs",
                column: "ClientProblem2Id",
                principalTable: "ClientProblems2",
                principalColumn: "ClientProblem2Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReplacementSTOs_ClientReplacements2_ClientReplacement2Id",
                table: "ClientReplacementSTOs",
                column: "ClientReplacement2Id",
                principalTable: "ClientReplacements2",
                principalColumn: "ClientReplacement2Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientProblemSTOs_ClientProblems2_ClientProblem2Id",
                table: "ClientProblemSTOs");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientReplacementSTOs_ClientReplacements2_ClientReplacement2Id",
                table: "ClientReplacementSTOs");

            migrationBuilder.DropTable(
                name: "ClientProblems2");

            migrationBuilder.DropTable(
                name: "ClientReplacements2");

            migrationBuilder.DropIndex(
                name: "IX_ClientReplacementSTOs_ClientReplacement2Id",
                table: "ClientReplacementSTOs");

            migrationBuilder.DropIndex(
                name: "IX_ClientProblemSTOs_ClientProblem2Id",
                table: "ClientProblemSTOs");

            migrationBuilder.RenameColumn(
                name: "ClientReplacement2Id",
                table: "ClientReplacementSTOs",
                newName: "ClientReplacementReplacementId");

            migrationBuilder.RenameColumn(
                name: "ClientProblem2Id",
                table: "ClientProblemSTOs",
                newName: "ClientProblemProblemId");

            migrationBuilder.AddColumn<int>(
                name: "ClientReplacementClientId",
                table: "ClientReplacementSTOs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientProblemClientId",
                table: "ClientProblemSTOs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientProblem",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false),
                    ProblemId = table.Column<int>(nullable: false),
                    BaselineCount = table.Column<int>(nullable: true),
                    BaselineFrom = table.Column<DateTime>(nullable: true),
                    BaselineTo = table.Column<DateTime>(nullable: true),
                    ClientProblemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProblem", x => new { x.ClientId, x.ProblemId });
                    table.ForeignKey(
                        name: "FK_ClientProblem_ProblemBehaviors_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "ProblemBehaviors",
                        principalColumn: "ProblemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientReplacement",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false),
                    ReplacementId = table.Column<int>(nullable: false),
                    BaselineFrom = table.Column<DateTime>(nullable: true),
                    BaselinePercent = table.Column<int>(nullable: true),
                    BaselineTo = table.Column<DateTime>(nullable: true),
                    ClientReplacementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientReplacement", x => new { x.ClientId, x.ReplacementId });
                    table.ForeignKey(
                        name: "FK_ClientReplacement_ReplacementPrograms_ReplacementId",
                        column: x => x.ReplacementId,
                        principalTable: "ReplacementPrograms",
                        principalColumn: "ReplacementId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ClientReplacementSTOs_ClientReplacementClientId_ClientReplacementReplacementId",
                table: "ClientReplacementSTOs",
                columns: new[] { "ClientReplacementClientId", "ClientReplacementReplacementId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProblemSTOs_ClientProblemClientId_ClientProblemProblemId",
                table: "ClientProblemSTOs",
                columns: new[] { "ClientProblemClientId", "ClientProblemProblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProblem_ProblemId",
                table: "ClientProblem",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientReplacement_ReplacementId",
                table: "ClientReplacement",
                column: "ReplacementId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientProblemSTOs_ClientProblem_ClientProblemClientId_ClientProblemProblemId",
                table: "ClientProblemSTOs",
                columns: new[] { "ClientProblemClientId", "ClientProblemProblemId" },
                principalTable: "ClientProblem",
                principalColumns: new[] { "ClientId", "ProblemId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReplacementSTOs_ClientReplacement_ClientReplacementClientId_ClientReplacementReplacementId",
                table: "ClientReplacementSTOs",
                columns: new[] { "ClientReplacementClientId", "ClientReplacementReplacementId" },
                principalTable: "ClientReplacement",
                principalColumns: new[] { "ClientId", "ReplacementId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
