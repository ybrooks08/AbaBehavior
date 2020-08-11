using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class ClientDiagnosisNewFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientDiagnostics_ClientId_DiagnosisId",
                table: "ClientDiagnostics");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "ClientDiagnostics",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ClientDiagnostics",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ClientDiagnostics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientDiagnostics_ClientId",
                table: "ClientDiagnostics",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientDiagnostics_ClientId",
                table: "ClientDiagnostics");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "ClientDiagnostics");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ClientDiagnostics");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ClientDiagnostics");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDiagnostics_ClientId_DiagnosisId",
                table: "ClientDiagnostics",
                columns: new[] { "ClientId", "DiagnosisId" },
                unique: true);
        }
    }
}
