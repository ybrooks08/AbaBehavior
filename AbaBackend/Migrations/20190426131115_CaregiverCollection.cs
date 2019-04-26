using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
    public partial class CaregiverCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaregiverDataCollections",
                columns: table => new
                {
                    CaregiverDataCollectionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    CollectDate = table.Column<DateTimeOffset>(nullable: false),
                    CaregiverId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaregiverDataCollections", x => x.CaregiverDataCollectionId);
                });

            migrationBuilder.CreateTable(
                name: "CaregiverDataCollectionProblems",
                columns: table => new
                {
                    CaregiverDataCollectionProblemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaregiverDataCollectionId = table.Column<int>(nullable: false),
                    ProblemId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaregiverDataCollectionProblems", x => x.CaregiverDataCollectionProblemId);
                    table.ForeignKey(
                        name: "FK_CaregiverDataCollectionProblems_CaregiverDataCollections_CaregiverDataCollectionId",
                        column: x => x.CaregiverDataCollectionId,
                        principalTable: "CaregiverDataCollections",
                        principalColumn: "CaregiverDataCollectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaregiverDataCollectionReplacements",
                columns: table => new
                {
                    CaregiverDataCollectionReplacementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaregiverDataCollectionId = table.Column<int>(nullable: false),
                    ReplacementId = table.Column<int>(nullable: false),
                    TotalTrial = table.Column<int>(nullable: true),
                    TotalCompleted = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaregiverDataCollectionReplacements", x => x.CaregiverDataCollectionReplacementId);
                    table.ForeignKey(
                        name: "FK_CaregiverDataCollectionReplacements_CaregiverDataCollections_CaregiverDataCollectionId",
                        column: x => x.CaregiverDataCollectionId,
                        principalTable: "CaregiverDataCollections",
                        principalColumn: "CaregiverDataCollectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaregiverDataCollectionProblems_CaregiverDataCollectionId",
                table: "CaregiverDataCollectionProblems",
                column: "CaregiverDataCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CaregiverDataCollectionReplacements_CaregiverDataCollectionId",
                table: "CaregiverDataCollectionReplacements",
                column: "CaregiverDataCollectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaregiverDataCollectionProblems");

            migrationBuilder.DropTable(
                name: "CaregiverDataCollectionReplacements");

            migrationBuilder.DropTable(
                name: "CaregiverDataCollections");
        }
    }
}
