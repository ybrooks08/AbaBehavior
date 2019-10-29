using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class PayrateNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PayRate",
                table: "Users",
                type: "decimal(6,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DriveTimePayRate",
                table: "Users",
                type: "decimal(6,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PayRate",
                table: "Users",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DriveTimePayRate",
                table: "Users",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                columns: new[] { "DriveTimePayRate", "PayRate" },
                values: new object[] { 0m, 0m });
        }
    }
}
