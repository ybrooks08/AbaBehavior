using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
  public partial class SessionCollectV2 : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "SessionCollectBehaviorsV2",
        columns: table => new
        {
          SessionCollectBehaviorV2Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
          SessionId = table.Column<int>(nullable: false),
          ProblemId = table.Column<int>(nullable: false),
          ClientId = table.Column<int>(nullable: false),
          Total = table.Column<int>(nullable: false),
          Completed = table.Column<int>(nullable: false),
          NoData = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_SessionCollectBehaviorsV2", x => x.SessionCollectBehaviorV2Id);
          table.ForeignKey(
            name: "FK_SessionCollectBehaviorsV2_ProblemBehaviors_ProblemId",
            column: x => x.ProblemId,
            principalTable: "ProblemBehaviors",
            principalColumn: "ProblemId",
            onDelete: ReferentialAction.Cascade);
        });

      migrationBuilder.CreateTable(
        name: "SessionCollectReplacementsV2",
        columns: table => new
        {
          SessionCollectReplacementV2Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
          SessionId = table.Column<int>(nullable: false),
          ReplacementId = table.Column<int>(nullable: false),
          ClientId = table.Column<int>(nullable: false),
          Total = table.Column<int>(nullable: false),
          Completed = table.Column<int>(nullable: false),
          NoData = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_SessionCollectReplacementsV2", x => x.SessionCollectReplacementV2Id);
          table.ForeignKey(
            name: "FK_SessionCollectReplacementsV2_ReplacementPrograms_ReplacementId",
            column: x => x.ReplacementId,
            principalTable: "ReplacementPrograms",
            principalColumn: "ReplacementId",
            onDelete: ReferentialAction.Cascade);
        });

      migrationBuilder.CreateIndex(
        name: "IX_SessionCollectBehaviorsV2_ProblemId",
        table: "SessionCollectBehaviorsV2",
        column: "ProblemId");

      migrationBuilder.CreateIndex(
        name: "IX_SessionCollectReplacementsV2_ReplacementId",
        table: "SessionCollectReplacementsV2",
        column: "ReplacementId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
        name: "SessionCollectBehaviorsV2");

      migrationBuilder.DropTable(
        name: "SessionCollectReplacementsV2");
    }
  }
}