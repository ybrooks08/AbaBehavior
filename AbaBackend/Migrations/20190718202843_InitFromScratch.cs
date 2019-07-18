using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbaBackend.Migrations
{
  public partial class InitFromScratch : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "BehaviorAnalysisCodes",
          columns: table => new
          {
            BehaviorAnalysisCodeId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Hcpcs = table.Column<string>(maxLength: 10, nullable: true),
            Description = table.Column<string>(maxLength: 60, nullable: true),
            Checkable = table.Column<bool>(nullable: false),
            Color = table.Column<string>(maxLength: 60, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_BehaviorAnalysisCodes", x => x.BehaviorAnalysisCodeId);
          });

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
          name: "CaregiversType",
          columns: table => new
          {
            CaregiverTypeId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Description = table.Column<string>(maxLength: 60, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CaregiversType", x => x.CaregiverTypeId);
          });

      migrationBuilder.CreateTable(
          name: "ClientChartNotes",
          columns: table => new
          {
            ClientChartNoteId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            ChartNoteType = table.Column<int>(nullable: false),
            ChartNoteDate = table.Column<DateTime>(nullable: false),
            Title = table.Column<string>(maxLength: 20, nullable: true),
            Note = table.Column<string>(maxLength: 200, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClientChartNotes", x => x.ClientChartNoteId);
          });

      migrationBuilder.CreateTable(
          name: "Clients",
          columns: table => new
          {
            ClientId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Code = table.Column<string>(maxLength: 10, nullable: true),
            Firstname = table.Column<string>(maxLength: 60, nullable: false),
            Lastname = table.Column<string>(maxLength: 100, nullable: false),
            Nickname = table.Column<string>(maxLength: 30, nullable: true),
            Dob = table.Column<DateTime>(nullable: false),
            Phone = table.Column<string>(maxLength: 15, nullable: true),
            Email = table.Column<string>(maxLength: 60, nullable: true),
            Address = table.Column<string>(maxLength: 100, nullable: false),
            Apt = table.Column<string>(maxLength: 10, nullable: true),
            City = table.Column<string>(maxLength: 50, nullable: false),
            State = table.Column<string>(maxLength: 50, nullable: false),
            Zipcode = table.Column<string>(maxLength: 10, nullable: false),
            Gender = table.Column<string>(maxLength: 15, nullable: true),
            Race = table.Column<string>(maxLength: 60, nullable: true),
            PrimaryLanguage = table.Column<string>(maxLength: 20, nullable: true),
            EmergencyContact = table.Column<string>(maxLength: 150, nullable: true),
            EmergencyPhone = table.Column<string>(maxLength: 15, nullable: true),
            EmergencyEmail = table.Column<string>(maxLength: 60, nullable: true),
            Notes = table.Column<string>(nullable: true),
            SocialSecurity = table.Column<string>(maxLength: 11, nullable: true),
            Insurance = table.Column<string>(maxLength: 50, nullable: true),
            MemberNo = table.Column<string>(maxLength: 50, nullable: true),
            MmaPlan = table.Column<string>(maxLength: 50, nullable: true),
            MmaIdNo = table.Column<string>(maxLength: 50, nullable: true),
            Active = table.Column<bool>(nullable: false),
            Created = table.Column<DateTime>(nullable: false),
            Modified = table.Column<DateTime>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Clients", x => x.ClientId);
          });

      migrationBuilder.CreateTable(
          name: "CompetencyCheckParams",
          columns: table => new
          {
            CompetencyCheckParamId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            CompetencyCheckType = table.Column<int>(nullable: false),
            Description = table.Column<string>(maxLength: 250, nullable: true),
            Comment = table.Column<string>(maxLength: 100, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CompetencyCheckParams", x => x.CompetencyCheckParamId);
          });

      migrationBuilder.CreateTable(
          name: "Diagnostics",
          columns: table => new
          {
            DiagnosisId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Code = table.Column<string>(maxLength: 20, nullable: true),
            Description = table.Column<string>(maxLength: 100, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Diagnostics", x => x.DiagnosisId);
          });

      migrationBuilder.CreateTable(
          name: "DocumentGroups",
          columns: table => new
          {
            DocumentGroupId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            GroupName = table.Column<string>(maxLength: 100, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_DocumentGroups", x => x.DocumentGroupId);
          });

      migrationBuilder.CreateTable(
          name: "Emails",
          columns: table => new
          {
            EmailId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            To = table.Column<string>(maxLength: 100, nullable: false),
            Subject = table.Column<string>(maxLength: 100, nullable: false),
            Body = table.Column<string>(nullable: true),
            MesssageType = table.Column<int>(nullable: false),
            Created = table.Column<DateTime>(nullable: false),
            Sent = table.Column<DateTime>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Emails", x => x.EmailId);
          });

      migrationBuilder.CreateTable(
          name: "ProblemBehaviors",
          columns: table => new
          {
            ProblemId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ProblemBehaviorDescription = table.Column<string>(maxLength: 100, nullable: false),
            Active = table.Column<bool>(nullable: false),
            IsPercent = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ProblemBehaviors", x => x.ProblemId);
          });

      migrationBuilder.CreateTable(
          name: "ReplacementPrograms",
          columns: table => new
          {
            ReplacementId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ReplacementProgramDescription = table.Column<string>(maxLength: 100, nullable: false),
            Active = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ReplacementPrograms", x => x.ReplacementId);
          });

      migrationBuilder.CreateTable(
          name: "Roles",
          columns: table => new
          {
            RolId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            RolName = table.Column<string>(maxLength: 100, nullable: false),
            RolShortName = table.Column<string>(maxLength: 20, nullable: false),
            CanCreateSession = table.Column<bool>(nullable: false),
            CanEditAllClientSession = table.Column<bool>(nullable: false),
            HasDocuments = table.Column<bool>(nullable: false),
            BehaviorAnalysisCodeId = table.Column<int>(nullable: true),
            TemplateName = table.Column<string>(maxLength: 20, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Roles", x => x.RolId);
            table.ForeignKey(
                      name: "FK_Roles_BehaviorAnalysisCodes_BehaviorAnalysisCodeId",
                      column: x => x.BehaviorAnalysisCodeId,
                      principalTable: "BehaviorAnalysisCodes",
                      principalColumn: "BehaviorAnalysisCodeId",
                      onDelete: ReferentialAction.Restrict);
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

      migrationBuilder.CreateTable(
          name: "Assessments",
          columns: table => new
          {
            AssessmentId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            BehaviorAnalysisCodeId = table.Column<int>(nullable: false),
            TotalUnits = table.Column<int>(nullable: false),
            StartDate = table.Column<DateTime>(nullable: false),
            EndDate = table.Column<DateTime>(nullable: false),
            PaNumber = table.Column<string>(maxLength: 12, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Assessments", x => x.AssessmentId);
            table.ForeignKey(
                      name: "FK_Assessments_BehaviorAnalysisCodes_BehaviorAnalysisCodeId",
                      column: x => x.BehaviorAnalysisCodeId,
                      principalTable: "BehaviorAnalysisCodes",
                      principalColumn: "BehaviorAnalysisCodeId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Assessments_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Caregivers",
          columns: table => new
          {
            CaregiverId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            CaregiverFullname = table.Column<string>(maxLength: 60, nullable: false),
            Phone = table.Column<string>(maxLength: 15, nullable: true),
            Email = table.Column<string>(maxLength: 60, nullable: true),
            CaregiverTypeId = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Caregivers", x => x.CaregiverId);
            table.ForeignKey(
                      name: "FK_Caregivers_CaregiversType_CaregiverTypeId",
                      column: x => x.CaregiverTypeId,
                      principalTable: "CaregiversType",
                      principalColumn: "CaregiverTypeId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Caregivers_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "MonthlyNotes",
          columns: table => new
          {
            MonthlyNoteId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            MonthlySummary = table.Column<string>(nullable: true),
            CommentsAboutCaregiver = table.Column<string>(nullable: true),
            Services2ProvideNextMonth = table.Column<string>(nullable: true),
            RecipientHealthIssues = table.Column<string>(nullable: true),
            Medication = table.Column<string>(nullable: true),
            FamilyChanges = table.Column<string>(nullable: true),
            HomeChanges = table.Column<string>(nullable: true),
            ProviverChanges = table.Column<string>(nullable: true),
            Barriers2Treatment = table.Column<string>(nullable: true),
            ContinueNextMonth = table.Column<bool>(nullable: false),
            ReassessmentNextMonth = table.Column<bool>(nullable: false),
            Refer2OtherServices = table.Column<bool>(nullable: false),
            ChangesCurrentPlan = table.Column<bool>(nullable: false),
            ExtraNotes = table.Column<string>(nullable: true),
            ClientId = table.Column<int>(nullable: false),
            MonthlyNoteDate = table.Column<DateTime>(nullable: false),
            Year = table.Column<int>(nullable: false),
            Month = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_MonthlyNotes", x => x.MonthlyNoteId);
            table.ForeignKey(
                      name: "FK_MonthlyNotes_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Referrals",
          columns: table => new
          {
            ReferralId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            ReferralFullname = table.Column<string>(maxLength: 60, nullable: false),
            Specialty = table.Column<string>(maxLength: 60, nullable: false),
            License = table.Column<string>(maxLength: 20, nullable: true),
            Provider = table.Column<string>(maxLength: 60, nullable: true),
            Npi = table.Column<string>(maxLength: 20, nullable: true),
            FullAddress = table.Column<string>(maxLength: 120, nullable: true),
            Phone = table.Column<string>(maxLength: 15, nullable: true),
            Fax = table.Column<string>(maxLength: 15, nullable: true),
            Email = table.Column<string>(maxLength: 60, nullable: true),
            DateReferral = table.Column<DateTime>(nullable: false),
            DateExpires = table.Column<DateTime>(nullable: false),
            Active = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Referrals", x => x.ReferralId);
            table.ForeignKey(
                      name: "FK_Referrals_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ClientDiagnostics",
          columns: table => new
          {
            ClientDiagnosisId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            DiagnosisId = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClientDiagnostics", x => x.ClientDiagnosisId);
            table.ForeignKey(
                      name: "FK_ClientDiagnostics_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_ClientDiagnostics_Diagnostics_DiagnosisId",
                      column: x => x.DiagnosisId,
                      principalTable: "Diagnostics",
                      principalColumn: "DiagnosisId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Documents",
          columns: table => new
          {
            DocumentId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            DocumentGroupId = table.Column<int>(nullable: false),
            DocumentName = table.Column<string>(maxLength: 200, nullable: false),
            DocumentExpires = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Documents", x => x.DocumentId);
            table.ForeignKey(
                      name: "FK_Documents_DocumentGroups_DocumentGroupId",
                      column: x => x.DocumentGroupId,
                      principalTable: "DocumentGroups",
                      principalColumn: "DocumentGroupId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ClientProblems",
          columns: table => new
          {
            ClientProblemId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            ProblemId = table.Column<int>(nullable: false),
            BaselineFrom = table.Column<DateTime>(nullable: true),
            BaselineTo = table.Column<DateTime>(nullable: true),
            BaselineCount = table.Column<int>(nullable: true),
            Active = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClientProblems", x => x.ClientProblemId);
            table.ForeignKey(
                      name: "FK_ClientProblems_ProblemBehaviors_ProblemId",
                      column: x => x.ProblemId,
                      principalTable: "ProblemBehaviors",
                      principalColumn: "ProblemId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SessionCollectBehaviors",
          columns: table => new
          {
            SessionCollectBehaviorId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionId = table.Column<int>(nullable: false),
            ProblemId = table.Column<int>(nullable: false),
            ClientId = table.Column<int>(nullable: false),
            Entry = table.Column<DateTime>(nullable: false),
            Notes = table.Column<string>(nullable: true),
            Duration = table.Column<int>(nullable: false),
            Completed = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionCollectBehaviors", x => x.SessionCollectBehaviorId);
            table.ForeignKey(
                      name: "FK_SessionCollectBehaviors_ProblemBehaviors_ProblemId",
                      column: x => x.ProblemId,
                      principalTable: "ProblemBehaviors",
                      principalColumn: "ProblemId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ClientReplacements",
          columns: table => new
          {
            ClientReplacementId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            ReplacementId = table.Column<int>(nullable: false),
            BaselineFrom = table.Column<DateTime>(nullable: true),
            BaselineTo = table.Column<DateTime>(nullable: true),
            BaselinePercent = table.Column<int>(nullable: true),
            Active = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClientReplacements", x => x.ClientReplacementId);
            table.ForeignKey(
                      name: "FK_ClientReplacements_ReplacementPrograms_ReplacementId",
                      column: x => x.ReplacementId,
                      principalTable: "ReplacementPrograms",
                      principalColumn: "ReplacementId",
                      onDelete: ReferentialAction.Cascade);
          });

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
          });

      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            UserId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Username = table.Column<string>(maxLength: 50, nullable: false),
            Firstname = table.Column<string>(maxLength: 60, nullable: false),
            Lastname = table.Column<string>(maxLength: 100, nullable: false),
            Email = table.Column<string>(maxLength: 100, nullable: false),
            RolId = table.Column<int>(nullable: false),
            Created = table.Column<DateTime>(nullable: false),
            Active = table.Column<bool>(nullable: false),
            Hash = table.Column<byte[]>(nullable: true),
            Salt = table.Column<byte[]>(nullable: true),
            Npi = table.Column<string>(maxLength: 20, nullable: true),
            Mpi = table.Column<string>(maxLength: 20, nullable: true),
            LicenseNo = table.Column<string>(maxLength: 20, nullable: true),
            SocialSecurity = table.Column<string>(maxLength: 11, nullable: true),
            Phone = table.Column<string>(maxLength: 15, nullable: true),
            Address = table.Column<string>(maxLength: 100, nullable: true),
            Apt = table.Column<string>(maxLength: 10, nullable: true),
            City = table.Column<string>(maxLength: 50, nullable: true),
            State = table.Column<string>(maxLength: 50, nullable: true),
            Zipcode = table.Column<string>(maxLength: 10, nullable: true),
            BankName = table.Column<string>(maxLength: 70, nullable: true),
            BankAddress = table.Column<string>(maxLength: 100, nullable: true),
            BankRoutingNumber = table.Column<string>(maxLength: 20, nullable: true),
            BankAccountNumber = table.Column<string>(maxLength: 20, nullable: true),
            PayRate = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
            DriveTimePayRate = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.UserId);
            table.ForeignKey(
                      name: "FK_Users_Roles_RolId",
                      column: x => x.RolId,
                      principalTable: "Roles",
                      principalColumn: "RolId",
                      onDelete: ReferentialAction.Cascade);
          });

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
            WeekEnd = table.Column<DateTime>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClientProblemSTOs", x => x.ClientProblemStoId);
            table.ForeignKey(
                      name: "FK_ClientProblemSTOs_ClientProblems_ClientProblemId",
                      column: x => x.ClientProblemId,
                      principalTable: "ClientProblems",
                      principalColumn: "ClientProblemId",
                      onDelete: ReferentialAction.Cascade);
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
            WeekEnd = table.Column<DateTime>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClientReplacementSTOs", x => x.ClientReplacementStoId);
            table.ForeignKey(
                      name: "FK_ClientReplacementSTOs_ClientReplacements_ClientReplacementId",
                      column: x => x.ClientReplacementId,
                      principalTable: "ClientReplacements",
                      principalColumn: "ClientReplacementId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Assignments",
          columns: table => new
          {
            AssignmentId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            ClientId = table.Column<int>(nullable: false),
            UserId = table.Column<int>(nullable: false),
            Active = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Assignments", x => x.AssignmentId);
            table.ForeignKey(
                      name: "FK_Assignments_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Assignments_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "CompetencyChecks",
          columns: table => new
          {
            CompetencyCheckId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            CompetencyCheckType = table.Column<int>(nullable: false),
            ClientId = table.Column<int>(nullable: false),
            CaregiverId = table.Column<int>(nullable: true),
            UserId = table.Column<int>(nullable: true),
            TotalDuration = table.Column<int>(nullable: false),
            Date = table.Column<DateTime>(nullable: false),
            EvaluationById = table.Column<int>(nullable: false),
            TotalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CompetencyChecks", x => x.CompetencyCheckId);
            table.ForeignKey(
                      name: "FK_CompetencyChecks_Caregivers_CaregiverId",
                      column: x => x.CaregiverId,
                      principalTable: "Caregivers",
                      principalColumn: "CaregiverId",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_CompetencyChecks_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_CompetencyChecks_Users_EvaluationById",
                      column: x => x.EvaluationById,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_CompetencyChecks_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "DocumentsUsers",
          columns: table => new
          {
            Id = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            UserId = table.Column<int>(nullable: false),
            DocumentId = table.Column<int>(nullable: false),
            Active = table.Column<bool>(nullable: false),
            Expires = table.Column<DateTime>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_DocumentsUsers", x => x.Id);
            table.ForeignKey(
                      name: "FK_DocumentsUsers_Documents_DocumentId",
                      column: x => x.DocumentId,
                      principalTable: "Documents",
                      principalColumn: "DocumentId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_DocumentsUsers_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Sessions",
          columns: table => new
          {
            SessionId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            UserId = table.Column<int>(nullable: false),
            ClientId = table.Column<int>(nullable: false),
            SessionStart = table.Column<DateTime>(nullable: false),
            SessionEnd = table.Column<DateTime>(nullable: false),
            TotalUnits = table.Column<int>(nullable: false),
            DriveTime = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
            SessionType = table.Column<int>(nullable: false),
            Pos = table.Column<int>(nullable: false),
            Created = table.Column<DateTime>(nullable: false),
            BehaviorAnalysisCodeId = table.Column<int>(nullable: false),
            SessionStatus = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Sessions", x => x.SessionId);
            table.ForeignKey(
                      name: "FK_Sessions_BehaviorAnalysisCodes_BehaviorAnalysisCodeId",
                      column: x => x.BehaviorAnalysisCodeId,
                      principalTable: "BehaviorAnalysisCodes",
                      principalColumn: "BehaviorAnalysisCodeId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Sessions_Clients_ClientId",
                      column: x => x.ClientId,
                      principalTable: "Clients",
                      principalColumn: "ClientId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Sessions_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SystemLogs",
          columns: table => new
          {
            SystemLogId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Entry = table.Column<DateTimeOffset>(nullable: false),
            Title = table.Column<string>(nullable: true),
            Description = table.Column<string>(nullable: true),
            SystemLogType = table.Column<int>(nullable: false),
            Module = table.Column<int>(nullable: false),
            ModuleValue = table.Column<string>(nullable: true),
            UserId = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SystemLogs", x => x.SystemLogId);
            table.ForeignKey(
                      name: "FK_SystemLogs_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "UserSigns",
          columns: table => new
          {
            UserSignId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            UserId = table.Column<int>(nullable: false),
            Sign = table.Column<string>(nullable: true),
            SignedDate = table.Column<DateTime>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UserSigns", x => x.UserSignId);
            table.ForeignKey(
                      name: "FK_UserSigns_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "CompetencyCheckClientParams",
          columns: table => new
          {
            CompetencyCheckClientParamId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            CompetencyCheckId = table.Column<int>(nullable: false),
            CompetencyCheckParamId = table.Column<int>(nullable: false),
            Comment = table.Column<string>(nullable: true),
            Score = table.Column<byte>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CompetencyCheckClientParams", x => x.CompetencyCheckClientParamId);
            table.ForeignKey(
                      name: "FK_CompetencyCheckClientParams_CompetencyChecks_CompetencyCheckId",
                      column: x => x.CompetencyCheckId,
                      principalTable: "CompetencyChecks",
                      principalColumn: "CompetencyCheckId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_CompetencyCheckClientParams_CompetencyCheckParams_CompetencyCheckParamId",
                      column: x => x.CompetencyCheckParamId,
                      principalTable: "CompetencyCheckParams",
                      principalColumn: "CompetencyCheckParamId",
                      onDelete: ReferentialAction.Cascade);
          });

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
            table.ForeignKey(
                      name: "FK_SessionCollectBehaviorsV2_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
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
            table.ForeignKey(
                      name: "FK_SessionCollectReplacementsV2_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SessionLogs",
          columns: table => new
          {
            SessionLogId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionId = table.Column<int>(nullable: false),
            Entry = table.Column<DateTimeOffset>(nullable: false),
            Title = table.Column<string>(nullable: true),
            Description = table.Column<string>(nullable: true),
            Icon = table.Column<string>(nullable: true),
            IconColor = table.Column<string>(nullable: true),
            UserId = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionLogs", x => x.SessionLogId);
            table.ForeignKey(
                      name: "FK_SessionLogs_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_SessionLogs_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "SessionNotes",
          columns: table => new
          {
            SessionNoteId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionId = table.Column<int>(nullable: false),
            CaregiverId = table.Column<int>(nullable: true),
            CaregiverNote = table.Column<string>(nullable: true),
            RiskBehavior = table.Column<int>(nullable: false),
            RiskBehaviorCrisisInvolved = table.Column<bool>(nullable: false),
            RiskBehaviorExplain = table.Column<string>(nullable: true),
            ReinforcersEdibles = table.Column<string>(nullable: true),
            ReinforcersNonEdibles = table.Column<string>(nullable: true),
            ReinforcersOthers = table.Column<string>(nullable: true),
            ReinforcersResult = table.Column<string>(nullable: true),
            ParticipationLevel = table.Column<int>(nullable: false),
            ProgressNotes = table.Column<string>(nullable: true),
            FeedbackCaregiver = table.Column<bool>(nullable: false),
            FeedbackCaregiverExplain = table.Column<string>(nullable: true),
            FeedbackOtherServices = table.Column<bool>(nullable: false),
            FeedbackOtherServicesExplain = table.Column<string>(nullable: true),
            CaregiverTrainingObservationFeedback = table.Column<bool>(nullable: false),
            CaregiverTrainingParentCaregiverTraining = table.Column<bool>(nullable: false),
            CaregiverTrainingCompetencyCheck = table.Column<bool>(nullable: false),
            CaregiverTrainingOther = table.Column<string>(nullable: true),
            CaregiverTrainingSummary = table.Column<string>(nullable: true),
            SummaryDirectObservation = table.Column<bool>(nullable: false),
            SummaryObservationFeedback = table.Column<bool>(nullable: false),
            SummaryImplementedReduction = table.Column<bool>(nullable: false),
            SummaryImplementedReplacement = table.Column<bool>(nullable: false),
            SummaryGeneralization = table.Column<bool>(nullable: false),
            SummaryCommunication = table.Column<bool>(nullable: false),
            SummaryOther = table.Column<string>(nullable: true),
            RejectNotes = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionNotes", x => x.SessionNoteId);
            table.ForeignKey(
                      name: "FK_SessionNotes_Caregivers_CaregiverId",
                      column: x => x.CaregiverId,
                      principalTable: "Caregivers",
                      principalColumn: "CaregiverId",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_SessionNotes_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SessionProblemNotes",
          columns: table => new
          {
            SessionProblemNoteId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionId = table.Column<int>(nullable: false),
            ProblemId = table.Column<int>(nullable: false),
            DuringWichActivities = table.Column<string>(nullable: true),
            ReplacementInterventionsUsed = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionProblemNotes", x => x.SessionProblemNoteId);
            table.ForeignKey(
                      name: "FK_SessionProblemNotes_ProblemBehaviors_ProblemId",
                      column: x => x.ProblemId,
                      principalTable: "ProblemBehaviors",
                      principalColumn: "ProblemId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_SessionProblemNotes_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SessionSigns",
          columns: table => new
          {
            SessionSignId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionId = table.Column<int>(nullable: false),
            Sign = table.Column<string>(nullable: true),
            SignedDate = table.Column<DateTime>(nullable: false),
            Auth = table.Column<Guid>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionSigns", x => x.SessionSignId);
            table.ForeignKey(
                      name: "FK_SessionSigns_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SessionSupervisionNotes",
          columns: table => new
          {
            SessionSupervisionNoteId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionId = table.Column<int>(nullable: false),
            CaregiverId = table.Column<int>(nullable: true),
            CaregiverNote = table.Column<string>(nullable: true),
            WorkWith = table.Column<int>(nullable: false),
            isDirectSession = table.Column<bool>(nullable: false),
            BriefObservation = table.Column<bool>(nullable: false),
            BriefReplacement = table.Column<bool>(nullable: false),
            BriefGeneralization = table.Column<bool>(nullable: false),
            BriefBCaBaTraining = table.Column<bool>(nullable: false),
            BriefInService = table.Column<bool>(nullable: false),
            BriefInServiceSubject = table.Column<string>(nullable: true),
            BriefOther = table.Column<bool>(nullable: false),
            BriefOtherDescription = table.Column<string>(nullable: true),
            OversightFollowUp = table.Column<int>(nullable: false),
            OversightDesigning = table.Column<int>(nullable: false),
            OversightContributing = table.Column<int>(nullable: false),
            OversightAnalyzing = table.Column<int>(nullable: false),
            OversightGoals = table.Column<int>(nullable: false),
            OversightMakingDecisions = table.Column<int>(nullable: false),
            OversightModeling = table.Column<int>(nullable: false),
            OversightResponse = table.Column<int>(nullable: false),
            OversightOverall = table.Column<int>(nullable: false),
            OversightFollowUpBool = table.Column<bool>(nullable: false),
            OversightDesigningBool = table.Column<bool>(nullable: false),
            OversightContributingBool = table.Column<bool>(nullable: false),
            OversightAnalyzingBool = table.Column<bool>(nullable: false),
            OversightGoalsBool = table.Column<bool>(nullable: false),
            OversightMakingDecisionsBool = table.Column<bool>(nullable: false),
            OversightModelingBool = table.Column<bool>(nullable: false),
            OversightResponseBool = table.Column<bool>(nullable: false),
            OversightOverallBool = table.Column<bool>(nullable: false),
            CommentsRelated = table.Column<string>(nullable: true),
            Recommendations = table.Column<string>(nullable: true),
            Validation = table.Column<bool>(nullable: false),
            NextScheduledDate = table.Column<DateTime>(nullable: true),
            RejectNotes = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionSupervisionNotes", x => x.SessionSupervisionNoteId);
            table.ForeignKey(
                      name: "FK_SessionSupervisionNotes_Caregivers_CaregiverId",
                      column: x => x.CaregiverId,
                      principalTable: "Caregivers",
                      principalColumn: "CaregiverId",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_SessionSupervisionNotes_Sessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "Sessions",
                      principalColumn: "SessionId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "SessionProblemNoteReplacements",
          columns: table => new
          {
            SessionProblemNoteReplacementId = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            SessionProblemNoteId = table.Column<int>(nullable: false),
            ReplacementId = table.Column<int>(nullable: false),
            Active = table.Column<bool>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SessionProblemNoteReplacements", x => x.SessionProblemNoteReplacementId);
            table.ForeignKey(
                      name: "FK_SessionProblemNoteReplacements_ReplacementPrograms_ReplacementId",
                      column: x => x.ReplacementId,
                      principalTable: "ReplacementPrograms",
                      principalColumn: "ReplacementId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_SessionProblemNoteReplacements_SessionProblemNotes_SessionProblemNoteId",
                      column: x => x.SessionProblemNoteId,
                      principalTable: "SessionProblemNotes",
                      principalColumn: "SessionProblemNoteId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.InsertData(
          table: "BehaviorAnalysisCodes",
          columns: new[] { "BehaviorAnalysisCodeId", "Checkable", "Color", "Description", "Hcpcs" },
          values: new object[,]
          {
                    { 1, false, "red", "Behavioral Assesment", "H0031" },
                    { 2, false, "pink", "Behavior Reassesment", "H0032" },
                    { 3, true, "brown", "Lead Analyst", "H2019" },
                    { 4, true, "indigo", "Assistant Behavior Analyst", "H2012" },
                    { 5, true, "blue-grey", "Technician", "H2014" }
          });

      migrationBuilder.InsertData(
          table: "CaregiversType",
          columns: new[] { "CaregiverTypeId", "Description" },
          values: new object[,]
          {
                    { 5, "Other" },
                    { 3, "Related" },
                    { 4, "Friend" },
                    { 1, "Parent" },
                    { 2, "Teacher" }
          });

      migrationBuilder.InsertData(
          table: "CompetencyCheckParams",
          columns: new[] { "CompetencyCheckParamId", "Comment", "CompetencyCheckType", "Description" },
          values: new object[,]
          {
                    { 10, "", 1, "Followed recommended intervention procedures upon occurrence of problem behaviore in session" },
                    { 16, "", 1, "Daily progress notes reviewed" },
                    { 15, "", 1, "Data collected to date permit to be graphed" },
                    { 13, "", 1, "Modeled to caregiver how to implement a portion of the behavior program" },
                    { 12, "", 1, "Implemented the replacement skills training programs and recorded the data" },
                    { 11, "", 1, "Provided reinforcement in accordance with the program" },
                    { 9, "", 1, "Collected data on problem behaviors which ocurred in session" },
                    { 14, "", 1, "Can indicate when the behavior occurred in another setting or with another person and the program was effectively implemented (Programming forgeneralization)" },
                    { 7, "List new setting/person. Describe generalization example during month", 2, "Can indicate when the behavior occurred in another setting or with another person and the program was effectively implemented (Programming forgeneralization)" },
                    { 6, "BASP Portion selected to describe and review", 2, "Demostrated to Behavior assistant how to implement a portion of the behavior program" },
                    { 5, "Reinforcer(s) used for replacement/acquisition training", 2, "Provided reinforcement in accordance with the program" },
                    { 4, "Acquisition(s) skill", 2, "Implemented the acquisition skills training proramas and recorded the data" },
                    { 3, "Replacement(s) skill specific for problem behavior", 2, "Implemented the replacement skills training programs and recorded the data" },
                    { 8, "Medical status/Environmental changes/New Problem Behavior/Severity of problem behavior", 2, "Verbally reported about major changes in the environment, daily activity schedule, medical status, or severity of problem behaviors, along with new behaviors occurring, and difficulty implementing behavior plan as is" },
                    { 2, "Interventions(s) used for problem behaviors", 2, "Followed recommended intervention procedures upon occurrence of problem behaviore in session" },
                    { 1, "Behavior(s) occurrences", 2, "Collected data on problem behaviors which ocurred in session" }
          });

      migrationBuilder.InsertData(
          table: "DocumentGroups",
          columns: new[] { "DocumentGroupId", "GroupName" },
          values: new object[,]
          {
                    { 6, "Inservices" },
                    { 5, "Licenses and Certifications" },
                    { 4, "Current work Documents" },
                    { 7, "Other and miscellaneous" },
                    { 2, "Personal Documents" },
                    { 1, "Legal" },
                    { 3, "Clearinghouse" }
          });

      migrationBuilder.InsertData(
          table: "ProblemBehaviors",
          columns: new[] { "ProblemId", "Active", "IsPercent", "ProblemBehaviorDescription" },
          values: new object[,]
          {
                    { 17, true, false, "Property Destructions" },
                    { 18, true, false, "Lying" },
                    { 19, true, false, "Cheating" },
                    { 20, true, false, "Impulsive Behavior" },
                    { 22, true, false, "Echolalia" },
                    { 23, true, false, "Hyper-reactivity to sensory input" },
                    { 24, true, false, "Hypo-reactivity to sensory input" },
                    { 26, true, false, "Difficulty with receptive language" },
                    { 27, true, false, "Difficulty with expressive language" },
                    { 28, true, false, "Bolting" },
                    { 29, true, false, "Saliva play or smearing" },
                    { 16, true, false, "Bruxism" },
                    { 25, true, false, "Insomnia" },
                    { 15, true, false, "Climbing-presenting risk of fall" },
                    { 21, true, false, "Vocal or Auditory stereotypy" },
                    { 13, true, false, "Biting Self" },
                    { 14, true, false, "Outburst Behavior" },
                    { 1, true, false, "Temper Tantrum" },
                    { 2, true, false, "Elopement" },
                    { 4, true, true, "Task Refusal" },
                    { 5, true, false, "Physical Aggression " },
                    { 6, true, false, "Verbal aggression" },
                    { 3, true, false, "Defiant behavior" },
                    { 8, true, false, "Hyperactive Behavior/Out of sit behavior" },
                    { 9, true, false, "Inattentive behavior/Off task behavior" },
                    { 10, true, false, "Disruptive behavior" },
                    { 11, true, false, "Mouthing unsafe objects" },
                    { 12, true, false, "Pica" },
                    { 7, true, false, "Self Injury Behavior" }
          });

      migrationBuilder.InsertData(
          table: "ReplacementPrograms",
          columns: new[] { "ReplacementId", "Active", "ReplacementProgramDescription" },
          values: new object[,]
          {
                    { 24, true, "Decision making" },
                    { 31, true, "Social skills" },
                    { 25, true, "Expressive communication skills" },
                    { 26, true, "Functional communication" },
                    { 27, true, "Responding to name (Eyes Contact)" },
                    { 28, true, "Responding to name (going)" },
                    { 29, true, "Safety skills" },
                    { 30, true, "Sensory skills" },
                    { 32, true, "Self-care skills" },
                    { 38, true, "Dressing pant" },
                    { 34, true, "Safety Crossing Street" },
                    { 35, true, "Brushing teeth" },
                    { 36, true, "Washing and Drying face" },
                    { 37, true, "Follow Night time routing" },
                    { 23, true, "Calming strategies/Breathing exercises" },
                    { 39, true, "Dressing Pullover" },
                    { 40, true, "Eating with spoon" },
                    { 41, true, "Keep eye contact" },
                    { 42, true, "Toilet training" },
                    { 33, true, "Showering Program" },
                    { 22, true, "Compliance skills" },
                    { 16, true, "Take a turns" },
                    { 20, true, "Collaborative play skills" },
                    { 1, true, "Attention seeking skills" },
                    { 2, true, "Ask For breaks" },
                    { 3, true, "Tangibles and activity request" },
                    { 4, true, "Interrupt conversation politely" },
                    { 5, true, "Following Instructions" },
                    { 6, true, "Following a schedule of activities" },
                    { 7, true, "Escape from demand program" },
                    { 21, true, "Communication skills" },
                    { 9, true, "Come here" },
                    { 8, true, "Working independently" },
                    { 11, true, "Accept “No response”" },
                    { 12, true, "Time on task" },
                    { 13, true, "Decreasing Distractibility program" },
                    { 14, true, "Initiate and sustain peers in an activity and play" },
                    { 15, true, "Initiation of physical interactions" },
                    { 17, true, "Greetings" },
                    { 18, true, "Sharing" },
                    { 19, true, "Requesting skills" },
                    { 10, true, "Delay of reinforcement" }
          });

      migrationBuilder.InsertData(
          table: "Roles",
          columns: new[] { "RolId", "BehaviorAnalysisCodeId", "CanCreateSession", "CanEditAllClientSession", "HasDocuments", "RolName", "RolShortName", "TemplateName" },
          values: new object[,]
          {
                    { 6, null, false, false, false, "Client management", "management", "clientmanagement" },
                    { 1, null, false, true, false, "Admin", "admin", "admin" },
                    { 5, null, false, false, false, "HR", "hr", "hr" },
                    { 7, null, false, false, false, "Billing", "billing", "billing" }
          });

      migrationBuilder.InsertData(
          table: "Documents",
          columns: new[] { "DocumentId", "DocumentExpires", "DocumentGroupId", "DocumentName" },
          values: new object[,]
          {
                    { 34, false, 4, "Policy on Jobs" },
                    { 33, false, 4, "Code of Professional Conduct" },
                    { 64, true, 6, "Med Error Prevention" },
                    { 35, false, 4, "Employment Agreement / Independent/Agreement" },
                    { 36, false, 4, "Employee Acknowledgement of Probationary Period Form" },
                    { 37, false, 4, "Disclosures" },
                    { 38, false, 4, "Employee Health release for denial TB" },
                    { 39, false, 4, "Telephone reference check form" },
                    { 40, true, 5, "Provider" },
                    { 41, false, 5, "ATN" },
                    { 42, false, 5, "NPI" },
                    { 43, true, 5, "Credentials" },
                    { 44, false, 5, "RBT" },
                    { 45, false, 5, "BCaBA" },
                    { 46, false, 5, "Analyst" },
                    { 47, false, 5, "Licenses" },
                    { 48, false, 5, "Diplomas" },
                    { 49, false, 5, "Transcripts" },
                    { 50, false, 5, "Attestation Form for Behavior Assistant" },
                    { 51, true, 6, "DCC (Direct Care Core Competences) (course ID No: 1060050)" },
                    { 52, true, 6, "Zero Tolerance (course ID No: 1058718)" },
                    { 53, true, 6, "Requirements for all Waiver Providers & Incident Reporting (course ID No: 1060049)" },
                    { 54, true, 6, "Learner Orientation Provider/Customer (course ID No: 1060049)" },
                    { 55, true, 6, "AIDs/HIV and Blood borne Pathogens (course ID No: 1059884)" },
                    { 56, true, 6, "CPR/AED/BASIC FIRST AIDS" },
                    { 57, true, 6, "HIPAA Basics (course ID No: 1058936)" },
                    { 58, true, 6, "Security Awareness" },
                    { 59, true, 6, "Access Civil Rights or Patient´s Rights" },
                    { 60, true, 6, "OSHA" },
                    { 61, true, 6, "Infection Control" },
                    { 32, false, 4, "Employment Statement of Commitment" },
                    { 31, false, 4, "Job Description" },
                    { 30, false, 4, "Workplace Expectation" },
                    { 29, false, 4, "Non-Institutional Medicaid Provider Agreements" },
                    { 66, true, 6, "Abuse/Neglect/Exploitation" },
                    { 65, true, 6, "Med Record Documentation" },
                    { 1, false, 1, "Social security" },
                    { 2, true, 1, "Proof Legal status (US citizen, Lawful Permanent Resident, Work Permit, etc.)" },
                    { 3, true, 1, "Driver License" },
                    { 4, true, 1, "I-9 Form: Employment Eligibility Verification" },
                    { 5, false, 1, "E-Verify" },
                    { 6, false, 1, "W9" },
                    { 7, true, 1, "Attestation of Compliance with Background Screening Requirements" },
                    { 8, false, 1, "Privacy Policy Acknowledgment Form" },
                    { 9, true, 1, "Liability" },
                    { 10, false, 1, "Confidentiality Agreement" },
                    { 11, false, 1, "HIPPA Privacy & Confidentiality Statement" },
                    { 12, false, 1, "Abuse Reporting Policies" },
                    { 62, true, 6, "Patient´s Rights OR Access Civil Rights" },
                    { 13, false, 1, "Commitment to educate about report abuse/neglect/explo/mis conduct" },
                    { 15, false, 1, "Managed care FRAUD and ABUSE" },
                    { 16, false, 1, "Affidavit Good Moral Charter (Notarized)" },
                    { 17, false, 2, "Employee General Information Form and Emergency Information" },
                    { 18, false, 2, "Resume" },
                    { 19, false, 2, "References 2 letter (previous employ)" },
                    { 20, true, 2, "Physician’s Health Certification TB test" },
                    { 21, true, 2, "Alcohol and drugs test" },
                    { 22, true, 2, "Car registration" },
                    { 23, true, 2, "Car insurance" },
                    { 24, true, 3, "Police record Footprints ACHA Level II" },
                    { 25, true, 3, "Local Police Background Check" },
                    { 26, true, 3, "OIG (results)" },
                    { 27, false, 4, "Application Form" },
                    { 28, false, 4, "Group Membership Authorization" },
                    { 14, false, 1, "Bill of Rights" },
                    { 63, true, 6, "Domestic Violence" }
          });

      migrationBuilder.InsertData(
          table: "Roles",
          columns: new[] { "RolId", "BehaviorAnalysisCodeId", "CanCreateSession", "CanEditAllClientSession", "HasDocuments", "RolName", "RolShortName", "TemplateName" },
          values: new object[,]
          {
                    { 2, 3, true, true, true, "Lead Analyst", "analyst", "specialist" },
                    { 4, 5, true, false, true, "Technician", "tech", "specialist" },
                    { 3, 4, true, false, true, "Assistant Behavior Analyst", "assistant", "specialist" }
          });

      migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "UserId", "Active", "Address", "Apt", "BankAccountNumber", "BankAddress", "BankName", "BankRoutingNumber", "City", "Created", "DriveTimePayRate", "Email", "Firstname", "Hash", "Lastname", "LicenseNo", "Mpi", "Npi", "PayRate", "Phone", "RolId", "Salt", "SocialSecurity", "State", "Username", "Zipcode" },
          values: new object[,]
          {
                    { 1, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "Yuri", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Morales", null, null, null, 0m, null, 1, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "admin", null },
                    { 5, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "John", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Jackson", null, null, null, 0m, null, 5, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "hr", null },
                    { 6, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "Roberto", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Perez", null, null, null, 0m, null, 6, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "clientmanagement", null },
                    { 7, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "Paco", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Rivera", null, null, null, 0m, null, 7, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "billing", null }
          });

      migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "UserId", "Active", "Address", "Apt", "BankAccountNumber", "BankAddress", "BankName", "BankRoutingNumber", "City", "Created", "DriveTimePayRate", "Email", "Firstname", "Hash", "Lastname", "LicenseNo", "Mpi", "Npi", "PayRate", "Phone", "RolId", "Salt", "SocialSecurity", "State", "Username", "Zipcode" },
          values: new object[] { 2, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "Jhon", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Doe", null, null, null, 0m, null, 2, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "analyst", null });

      migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "UserId", "Active", "Address", "Apt", "BankAccountNumber", "BankAddress", "BankName", "BankRoutingNumber", "City", "Created", "DriveTimePayRate", "Email", "Firstname", "Hash", "Lastname", "LicenseNo", "Mpi", "Npi", "PayRate", "Phone", "RolId", "Salt", "SocialSecurity", "State", "Username", "Zipcode" },
          values: new object[] { 3, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "Peter", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Smith", null, null, null, 0m, null, 3, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "analystassistant", null });

      migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "UserId", "Active", "Address", "Apt", "BankAccountNumber", "BankAddress", "BankName", "BankRoutingNumber", "City", "Created", "DriveTimePayRate", "Email", "Firstname", "Hash", "Lastname", "LicenseNo", "Mpi", "Npi", "PayRate", "Phone", "RolId", "Salt", "SocialSecurity", "State", "Username", "Zipcode" },
          values: new object[] { 4, true, null, null, null, null, null, null, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ymorales@redient.com", "Jason", new byte[] { 38, 71, 169, 96, 147, 143, 219, 73, 62, 228, 252, 185, 59, 102, 208, 177, 200, 43, 108, 98, 133, 236, 42, 143, 117, 206, 133, 158, 73, 239, 190, 89, 180, 69, 209, 113, 243, 227, 183, 246, 62, 122, 97, 162, 19, 151, 20, 6, 251, 16, 224, 0, 214, 163, 86, 188, 211, 148, 233, 104, 46, 52, 234, 141 }, "Bourne", null, null, null, 0m, null, 4, new byte[] { 73, 78, 73, 84, 73, 65, 76, 45, 83, 65, 76, 84, 45, 72, 69, 82, 69 }, null, null, "rbt", null });

      migrationBuilder.CreateIndex(
          name: "IX_Assessments_BehaviorAnalysisCodeId",
          table: "Assessments",
          column: "BehaviorAnalysisCodeId");

      migrationBuilder.CreateIndex(
          name: "IX_Assessments_ClientId",
          table: "Assessments",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_Assignments_UserId",
          table: "Assignments",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Assignments_ClientId_UserId",
          table: "Assignments",
          columns: new[] { "ClientId", "UserId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_BehaviorAnalysisCodes_Hcpcs",
          table: "BehaviorAnalysisCodes",
          column: "Hcpcs",
          unique: true,
          filter: "[Hcpcs] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_CaregiverDataCollectionProblems_CaregiverDataCollectionId",
          table: "CaregiverDataCollectionProblems",
          column: "CaregiverDataCollectionId");

      migrationBuilder.CreateIndex(
          name: "IX_CaregiverDataCollectionReplacements_CaregiverDataCollectionId",
          table: "CaregiverDataCollectionReplacements",
          column: "CaregiverDataCollectionId");

      migrationBuilder.CreateIndex(
          name: "IX_Caregivers_CaregiverTypeId",
          table: "Caregivers",
          column: "CaregiverTypeId");

      migrationBuilder.CreateIndex(
          name: "IX_Caregivers_ClientId",
          table: "Caregivers",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_ClientDiagnostics_DiagnosisId",
          table: "ClientDiagnostics",
          column: "DiagnosisId");

      migrationBuilder.CreateIndex(
          name: "IX_ClientDiagnostics_ClientId_DiagnosisId",
          table: "ClientDiagnostics",
          columns: new[] { "ClientId", "DiagnosisId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_ClientProblems_ProblemId",
          table: "ClientProblems",
          column: "ProblemId");

      migrationBuilder.CreateIndex(
          name: "IX_ClientProblemSTOs_ClientProblemId",
          table: "ClientProblemSTOs",
          column: "ClientProblemId");

      migrationBuilder.CreateIndex(
          name: "IX_ClientReplacements_ReplacementId",
          table: "ClientReplacements",
          column: "ReplacementId");

      migrationBuilder.CreateIndex(
          name: "IX_ClientReplacementSTOs_ClientReplacementId",
          table: "ClientReplacementSTOs",
          column: "ClientReplacementId");

      migrationBuilder.CreateIndex(
          name: "IX_CompetencyCheckClientParams_CompetencyCheckId",
          table: "CompetencyCheckClientParams",
          column: "CompetencyCheckId");

      migrationBuilder.CreateIndex(
          name: "IX_CompetencyCheckClientParams_CompetencyCheckParamId",
          table: "CompetencyCheckClientParams",
          column: "CompetencyCheckParamId");

      migrationBuilder.CreateIndex(
          name: "IX_CompetencyChecks_CaregiverId",
          table: "CompetencyChecks",
          column: "CaregiverId");

      migrationBuilder.CreateIndex(
          name: "IX_CompetencyChecks_ClientId",
          table: "CompetencyChecks",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_CompetencyChecks_EvaluationById",
          table: "CompetencyChecks",
          column: "EvaluationById");

      migrationBuilder.CreateIndex(
          name: "IX_CompetencyChecks_UserId",
          table: "CompetencyChecks",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Diagnostics_Code",
          table: "Diagnostics",
          column: "Code",
          unique: true,
          filter: "[Code] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_Documents_DocumentGroupId",
          table: "Documents",
          column: "DocumentGroupId");

      migrationBuilder.CreateIndex(
          name: "IX_DocumentsUsers_DocumentId",
          table: "DocumentsUsers",
          column: "DocumentId");

      migrationBuilder.CreateIndex(
          name: "IX_DocumentsUsers_UserId_DocumentId",
          table: "DocumentsUsers",
          columns: new[] { "UserId", "DocumentId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_MonthlyNotes_ClientId",
          table: "MonthlyNotes",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_ProblemBehaviors_ProblemBehaviorDescription",
          table: "ProblemBehaviors",
          column: "ProblemBehaviorDescription",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Referrals_ClientId",
          table: "Referrals",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_ReplacementPrograms_ReplacementProgramDescription",
          table: "ReplacementPrograms",
          column: "ReplacementProgramDescription",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Roles_BehaviorAnalysisCodeId",
          table: "Roles",
          column: "BehaviorAnalysisCodeId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionCollectBehaviors_ProblemId",
          table: "SessionCollectBehaviors",
          column: "ProblemId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionCollectBehaviorsV2_ProblemId",
          table: "SessionCollectBehaviorsV2",
          column: "ProblemId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionCollectBehaviorsV2_SessionId",
          table: "SessionCollectBehaviorsV2",
          column: "SessionId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionCollectReplacements_ReplacementId",
          table: "SessionCollectReplacements",
          column: "ReplacementId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionCollectReplacementsV2_ReplacementId",
          table: "SessionCollectReplacementsV2",
          column: "ReplacementId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionCollectReplacementsV2_SessionId",
          table: "SessionCollectReplacementsV2",
          column: "SessionId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionLogs_SessionId",
          table: "SessionLogs",
          column: "SessionId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionLogs_UserId",
          table: "SessionLogs",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionNotes_CaregiverId",
          table: "SessionNotes",
          column: "CaregiverId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionNotes_SessionId",
          table: "SessionNotes",
          column: "SessionId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_SessionProblemNoteReplacements_ReplacementId",
          table: "SessionProblemNoteReplacements",
          column: "ReplacementId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionProblemNoteReplacements_SessionProblemNoteId",
          table: "SessionProblemNoteReplacements",
          column: "SessionProblemNoteId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionProblemNotes_ProblemId",
          table: "SessionProblemNotes",
          column: "ProblemId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionProblemNotes_SessionId",
          table: "SessionProblemNotes",
          column: "SessionId");

      migrationBuilder.CreateIndex(
          name: "IX_Sessions_BehaviorAnalysisCodeId",
          table: "Sessions",
          column: "BehaviorAnalysisCodeId");

      migrationBuilder.CreateIndex(
          name: "IX_Sessions_ClientId",
          table: "Sessions",
          column: "ClientId");

      migrationBuilder.CreateIndex(
          name: "IX_Sessions_UserId",
          table: "Sessions",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionSigns_SessionId",
          table: "SessionSigns",
          column: "SessionId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_SessionSupervisionNotes_CaregiverId",
          table: "SessionSupervisionNotes",
          column: "CaregiverId");

      migrationBuilder.CreateIndex(
          name: "IX_SessionSupervisionNotes_SessionId",
          table: "SessionSupervisionNotes",
          column: "SessionId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_SystemLogs_UserId",
          table: "SystemLogs",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Users_RolId",
          table: "Users",
          column: "RolId");

      migrationBuilder.CreateIndex(
          name: "IX_Users_Username",
          table: "Users",
          column: "Username",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_UserSigns_UserId",
          table: "UserSigns",
          column: "UserId",
          unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Assessments");

      migrationBuilder.DropTable(
          name: "Assignments");

      migrationBuilder.DropTable(
          name: "CaregiverDataCollectionProblems");

      migrationBuilder.DropTable(
          name: "CaregiverDataCollectionReplacements");

      migrationBuilder.DropTable(
          name: "ClientChartNotes");

      migrationBuilder.DropTable(
          name: "ClientDiagnostics");

      migrationBuilder.DropTable(
          name: "ClientProblemSTOs");

      migrationBuilder.DropTable(
          name: "ClientReplacementSTOs");

      migrationBuilder.DropTable(
          name: "CompetencyCheckClientParams");

      migrationBuilder.DropTable(
          name: "DocumentsUsers");

      migrationBuilder.DropTable(
          name: "Emails");

      migrationBuilder.DropTable(
          name: "MonthlyNotes");

      migrationBuilder.DropTable(
          name: "Referrals");

      migrationBuilder.DropTable(
          name: "SessionCollectBehaviors");

      migrationBuilder.DropTable(
          name: "SessionCollectBehaviorsV2");

      migrationBuilder.DropTable(
          name: "SessionCollectReplacements");

      migrationBuilder.DropTable(
          name: "SessionCollectReplacementsV2");

      migrationBuilder.DropTable(
          name: "SessionLogs");

      migrationBuilder.DropTable(
          name: "SessionNotes");

      migrationBuilder.DropTable(
          name: "SessionProblemNoteReplacements");

      migrationBuilder.DropTable(
          name: "SessionSigns");

      migrationBuilder.DropTable(
          name: "SessionSupervisionNotes");

      migrationBuilder.DropTable(
          name: "SystemLogs");

      migrationBuilder.DropTable(
          name: "UserSigns");

      migrationBuilder.DropTable(
          name: "CaregiverDataCollections");

      migrationBuilder.DropTable(
          name: "Diagnostics");

      migrationBuilder.DropTable(
          name: "ClientProblems");

      migrationBuilder.DropTable(
          name: "ClientReplacements");

      migrationBuilder.DropTable(
          name: "CompetencyChecks");

      migrationBuilder.DropTable(
          name: "CompetencyCheckParams");

      migrationBuilder.DropTable(
          name: "Documents");

      migrationBuilder.DropTable(
          name: "SessionProblemNotes");

      migrationBuilder.DropTable(
          name: "ReplacementPrograms");

      migrationBuilder.DropTable(
          name: "Caregivers");

      migrationBuilder.DropTable(
          name: "DocumentGroups");

      migrationBuilder.DropTable(
          name: "ProblemBehaviors");

      migrationBuilder.DropTable(
          name: "Sessions");

      migrationBuilder.DropTable(
          name: "CaregiversType");

      migrationBuilder.DropTable(
          name: "Clients");

      migrationBuilder.DropTable(
          name: "Users");

      migrationBuilder.DropTable(
          name: "Roles");

      migrationBuilder.DropTable(
          name: "BehaviorAnalysisCodes");
    }
  }
}
