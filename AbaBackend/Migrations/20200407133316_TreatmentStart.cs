using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class TreatmentStart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TreatmentStart",
                table: "ClientReplacements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TreatmentStart",
                table: "ClientProblems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreatmentStart",
                table: "ClientReplacements");

            migrationBuilder.DropColumn(
                name: "TreatmentStart",
                table: "ClientProblems");
        }
    }
}
