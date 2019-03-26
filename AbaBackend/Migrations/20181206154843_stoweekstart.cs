using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class stoweekstart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WeekEnd",
                table: "PeriodClientProblemSTOs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WeekStart",
                table: "PeriodClientProblemSTOs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "IX_SessionProblemNotes_SessionId",
                table: "SessionProblemNotes",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodClientProblems_Periods_PeriodId",
                table: "PeriodClientProblems",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "PeriodId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodClientReplacements_Periods_PeriodId",
                table: "PeriodClientReplacements",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "PeriodId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionProblemNotes_Sessions_SessionId",
                table: "SessionProblemNotes",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodClientProblems_Periods_PeriodId",
                table: "PeriodClientProblems");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodClientReplacements_Periods_PeriodId",
                table: "PeriodClientReplacements");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionProblemNotes_Sessions_SessionId",
                table: "SessionProblemNotes");

            migrationBuilder.DropIndex(
                name: "IX_SessionProblemNotes_SessionId",
                table: "SessionProblemNotes");

            migrationBuilder.DropColumn(
                name: "WeekEnd",
                table: "PeriodClientProblemSTOs");

            migrationBuilder.DropColumn(
                name: "WeekStart",
                table: "PeriodClientProblemSTOs");

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
