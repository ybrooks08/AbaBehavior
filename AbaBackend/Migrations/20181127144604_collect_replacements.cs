using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class collect_replacements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionCollectReplacements",
                columns: table => new
                {
                    SessionCollectReplacementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SessionId = table.Column<int>(nullable: false),
                    ReplacementId = table.Column<int>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    Entry = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Completed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionCollectReplacements", x => x.SessionCollectReplacementId);
                    table.ForeignKey(
                        name: "FK_SessionCollectReplacements_ReplacementPrograms_ReplacementId",
                        column: x => x.ReplacementId,
                        principalTable: "ReplacementPrograms",
                        principalColumn: "ReplacementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionCollectReplacements_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
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
                name: "IX_SessionCollectBehaviors_ProblemId",
                table: "SessionCollectBehaviors",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionCollectReplacements_ReplacementId",
                table: "SessionCollectReplacements",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionCollectReplacements_SessionId",
                table: "SessionCollectReplacements",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionCollectBehaviors_ProblemBehaviors_ProblemId",
                table: "SessionCollectBehaviors",
                column: "ProblemId",
                principalTable: "ProblemBehaviors",
                principalColumn: "ProblemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionCollectBehaviors_ProblemBehaviors_ProblemId",
                table: "SessionCollectBehaviors");

            migrationBuilder.DropTable(
                name: "SessionCollectReplacements");

            migrationBuilder.DropIndex(
                name: "IX_SessionCollectBehaviors_ProblemId",
                table: "SessionCollectBehaviors");

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
        }
    }
}
