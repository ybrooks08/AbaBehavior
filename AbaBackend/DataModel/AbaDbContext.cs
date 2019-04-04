using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AbaBackend.Infrastructure.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AbaBackend.DataModel
{
  public class AbaDbContext : DbContext
  {
    private IHostingEnvironment _env;

    public AbaDbContext(DbContextOptions<AbaDbContext> options, IHostingEnvironment env) : base(options)
    {
      _env = env;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
                  .HasIndex(b => b.Username)
                  .IsUnique();

      modelBuilder.Entity<ProblemBehavior>()
                  .HasIndex(b => b.ProblemBehaviorDescription)
                  .IsUnique();

      modelBuilder.Entity<ReplacementProgram>()
                  .HasIndex(b => b.ReplacementProgramDescription)
                  .IsUnique();

      // modelBuilder.Entity<ClientProblem>()
      //             .HasKey(x => new { x.ClientProblemId });

      // modelBuilder.Entity<ClientReplacement>()
      //             .HasKey(x => new { x.ClientReplacementId });

      modelBuilder.Entity<Diagnosis>()
                .HasIndex(x => x.Code)
                .IsUnique();

      modelBuilder.Entity<ClientDiagnosis>()
                  .HasIndex(x => new { x.ClientId, x.DiagnosisId })
                  .IsUnique();

      modelBuilder.Entity<DocumentUser>()
                  .HasIndex(x => new { x.UserId, x.DocumentId })
                  .IsUnique();

      modelBuilder.Entity<BehaviorAnalysisCode>()
                  .HasIndex(x => x.Hcpcs)
                  .IsUnique();

      modelBuilder.Entity<Assignment>()
                  .HasIndex(x => new { x.ClientId, x.UserId })
                  .IsUnique();

      modelBuilder.Entity<SessionLog>()
                  .HasOne(s => s.User)
                  .WithMany()
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<SystemLog>()
                  .HasOne(s => s.User)
                  .WithMany()
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

      //Seed
      var salt = Encoding.ASCII.GetBytes("INITIAL-SALT-HERE");//Guid.NewGuid().ToByteArray();
      var hash = new PasswordHasher();
      modelBuilder.Entity<BehaviorAnalysisCode>().HasData(
        new BehaviorAnalysisCode { BehaviorAnalysisCodeId = 1, Hcpcs = "H0031", Description = "Behavioral Assesment", Checkable = false, Color = "red" },
        new BehaviorAnalysisCode { BehaviorAnalysisCodeId = 2, Hcpcs = "H0032", Description = "Behavior Reassesment", Checkable = false, Color = "pink" },
        new BehaviorAnalysisCode { BehaviorAnalysisCodeId = 3, Hcpcs = "H2019", Description = "Lead Analyst", Checkable = true, Color = "brown" },
        new BehaviorAnalysisCode { BehaviorAnalysisCodeId = 4, Hcpcs = "H2012", Description = "Assistant Behavior Analyst", Checkable = true, Color = "indigo" },
        new BehaviorAnalysisCode { BehaviorAnalysisCodeId = 5, Hcpcs = "H2014", Description = "Technician", Checkable = true, Color = "blue-grey" });
      modelBuilder.Entity<Rol>().HasData(
        new Rol { RolId = 1, RolName = "Admin", RolShortName = "admin", CanCreateSession = false, CanEditAllClientSession = true, HasDocuments = false, TemplateName = "admin" },
        new Rol { RolId = 2, RolName = "Lead Analyst", RolShortName = "analyst", CanCreateSession = true, CanEditAllClientSession = true, HasDocuments = true, BehaviorAnalysisCodeId = 3, TemplateName = "specialist" },
        new Rol { RolId = 3, RolName = "Assistant Behavior Analyst", RolShortName = "assistant", CanCreateSession = true, CanEditAllClientSession = false, HasDocuments = true, BehaviorAnalysisCodeId = 4, TemplateName = "specialist" },
        new Rol { RolId = 4, RolName = "Technician", RolShortName = "tech", CanCreateSession = true, CanEditAllClientSession = false, HasDocuments = true, BehaviorAnalysisCodeId = 5, TemplateName = "specialist" },
        new Rol { RolId = 5, RolName = "HR", RolShortName = "hr", CanCreateSession = false, CanEditAllClientSession = false, HasDocuments = false, TemplateName = "hr" },
        new Rol { RolId = 6, RolName = "Client management", RolShortName = "management", CanCreateSession = false, CanEditAllClientSession = false, HasDocuments = false, TemplateName = "clientmanagement" },
        new Rol { RolId = 7, RolName = "Billing", RolShortName = "billing", CanCreateSession = false, CanEditAllClientSession = false, HasDocuments = false, TemplateName = "billing" });
      modelBuilder.Entity<User>().HasData(
        new User { UserId = 1, Username = "admin", Firstname = "Yuri", Lastname = "Morales", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 1, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) },
        new User { UserId = 2, Username = "analyst", Firstname = "Jhon", Lastname = "Doe", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 2, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) },
        new User { UserId = 3, Username = "analystassistant", Firstname = "Peter", Lastname = "Smith", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 3, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) },
        new User { UserId = 4, Username = "rbt", Firstname = "Jason", Lastname = "Bourne", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 4, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) },
        new User { UserId = 5, Username = "hr", Firstname = "John", Lastname = "Jackson", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 5, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) },
        new User { UserId = 6, Username = "clientmanagement", Firstname = "Roberto", Lastname = "Perez", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 6, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) },
        new User { UserId = 7, Username = "billing", Firstname = "Paco", Lastname = "Rivera", Salt = salt, Hash = hash.Hash("Passw0rd", salt), RolId = 7, Email = "ymorales@redient.com", Created = new DateTime(2018, 1, 1) });
      modelBuilder.Entity<ProblemBehavior>().HasData(
        new ProblemBehavior { ProblemId = 1, ProblemBehaviorDescription = "Temper Tantrum" },
        new ProblemBehavior { ProblemId = 2, ProblemBehaviorDescription = "Elopement" },
        new ProblemBehavior { ProblemId = 3, ProblemBehaviorDescription = "Defiant behavior" },
        new ProblemBehavior { ProblemId = 4, ProblemBehaviorDescription = "Task Refusal" },
        new ProblemBehavior { ProblemId = 5, ProblemBehaviorDescription = "Physical Aggression " },
        new ProblemBehavior { ProblemId = 6, ProblemBehaviorDescription = "Verbal aggression" },
        new ProblemBehavior { ProblemId = 7, ProblemBehaviorDescription = "Self Injury Behavior" },
        new ProblemBehavior { ProblemId = 8, ProblemBehaviorDescription = "Hyperactive Behavior/Out of sit behavior" },
        new ProblemBehavior { ProblemId = 9, ProblemBehaviorDescription = "Inattentive behavior/Off task behavior" },
        new ProblemBehavior { ProblemId = 10, ProblemBehaviorDescription = "Disruptive behavior" },
        new ProblemBehavior { ProblemId = 11, ProblemBehaviorDescription = "Mouthing unsafe objects" },
        new ProblemBehavior { ProblemId = 12, ProblemBehaviorDescription = "Pica" },
        new ProblemBehavior { ProblemId = 13, ProblemBehaviorDescription = "Biting Self" },
        new ProblemBehavior { ProblemId = 14, ProblemBehaviorDescription = "Outburst Behavior" },
        new ProblemBehavior { ProblemId = 15, ProblemBehaviorDescription = "Climbing-presenting risk of fall" },
        new ProblemBehavior { ProblemId = 16, ProblemBehaviorDescription = "Bruxism" },
        new ProblemBehavior { ProblemId = 17, ProblemBehaviorDescription = "Property Destructions" },
        new ProblemBehavior { ProblemId = 18, ProblemBehaviorDescription = "Lying" },
        new ProblemBehavior { ProblemId = 19, ProblemBehaviorDescription = "Cheating" },
        new ProblemBehavior { ProblemId = 20, ProblemBehaviorDescription = "Impulsive Behavior" },
        new ProblemBehavior { ProblemId = 21, ProblemBehaviorDescription = "Vocal or Auditory stereotypy" },
        new ProblemBehavior { ProblemId = 22, ProblemBehaviorDescription = "Echolalia" },
        new ProblemBehavior { ProblemId = 23, ProblemBehaviorDescription = "Hyper-reactivity to sensory input" },
        new ProblemBehavior { ProblemId = 24, ProblemBehaviorDescription = "Hypo-reactivity to sensory input" },
        new ProblemBehavior { ProblemId = 25, ProblemBehaviorDescription = "Insomnia" },
        new ProblemBehavior { ProblemId = 26, ProblemBehaviorDescription = "Difficulty with receptive language" },
        new ProblemBehavior { ProblemId = 27, ProblemBehaviorDescription = "Difficulty with expressive language" },
        new ProblemBehavior { ProblemId = 28, ProblemBehaviorDescription = "Bolting" },
        new ProblemBehavior { ProblemId = 29, ProblemBehaviorDescription = "Saliva play or smearing" });

      modelBuilder.Entity<ReplacementProgram>().HasData(
        new ReplacementProgram { ReplacementId = 1, ReplacementProgramDescription = "Attention seeking skills" },
        new ReplacementProgram { ReplacementId = 2, ReplacementProgramDescription = "Ask For breaks" },
        new ReplacementProgram { ReplacementId = 3, ReplacementProgramDescription = "Tangibles and activity request" },
        new ReplacementProgram { ReplacementId = 4, ReplacementProgramDescription = "Interrupt conversation politely" },
        new ReplacementProgram { ReplacementId = 5, ReplacementProgramDescription = "Following Instructions" },
        new ReplacementProgram { ReplacementId = 6, ReplacementProgramDescription = "Following a schedule of activities" },
        new ReplacementProgram { ReplacementId = 7, ReplacementProgramDescription = "Escape from demand program" },
        new ReplacementProgram { ReplacementId = 8, ReplacementProgramDescription = "Working independently" },
        new ReplacementProgram { ReplacementId = 9, ReplacementProgramDescription = "Come here" },
        new ReplacementProgram { ReplacementId = 10, ReplacementProgramDescription = "Delay of reinforcement" },
        new ReplacementProgram { ReplacementId = 11, ReplacementProgramDescription = "Accept “No response”" },
        new ReplacementProgram { ReplacementId = 12, ReplacementProgramDescription = "Time on task" },
        new ReplacementProgram { ReplacementId = 13, ReplacementProgramDescription = "Decreasing Distractibility program" },
        new ReplacementProgram { ReplacementId = 14, ReplacementProgramDescription = "Initiate and sustain peers in an activity and play" },
        new ReplacementProgram { ReplacementId = 15, ReplacementProgramDescription = "Initiation of physical interactions" },
        new ReplacementProgram { ReplacementId = 16, ReplacementProgramDescription = "Take a turns" },
        new ReplacementProgram { ReplacementId = 17, ReplacementProgramDescription = "Greetings" },
        new ReplacementProgram { ReplacementId = 18, ReplacementProgramDescription = "Sharing" },
        new ReplacementProgram { ReplacementId = 19, ReplacementProgramDescription = "Requesting skills" },
        new ReplacementProgram { ReplacementId = 20, ReplacementProgramDescription = "Collaborative play skills" },
        new ReplacementProgram { ReplacementId = 21, ReplacementProgramDescription = "Communication skills" },
        new ReplacementProgram { ReplacementId = 22, ReplacementProgramDescription = "Compliance skills" },
        new ReplacementProgram { ReplacementId = 23, ReplacementProgramDescription = "Calming strategies/Breathing exercises" },
        new ReplacementProgram { ReplacementId = 24, ReplacementProgramDescription = "Decision making" },
        new ReplacementProgram { ReplacementId = 25, ReplacementProgramDescription = "Expressive communication skills" },
        new ReplacementProgram { ReplacementId = 26, ReplacementProgramDescription = "Functional communication" },
        new ReplacementProgram { ReplacementId = 27, ReplacementProgramDescription = "Responding to name (Eyes Contact)" },
        new ReplacementProgram { ReplacementId = 28, ReplacementProgramDescription = "Responding to name (going)" },
        new ReplacementProgram { ReplacementId = 29, ReplacementProgramDescription = "Safety skills" },
        new ReplacementProgram { ReplacementId = 30, ReplacementProgramDescription = "Sensory skills" },
        new ReplacementProgram { ReplacementId = 31, ReplacementProgramDescription = "Social skills" },
        new ReplacementProgram { ReplacementId = 32, ReplacementProgramDescription = "Self-care skills" },
        new ReplacementProgram { ReplacementId = 33, ReplacementProgramDescription = "Showering Program" },
        new ReplacementProgram { ReplacementId = 34, ReplacementProgramDescription = "Safety Crossing Street" },
        new ReplacementProgram { ReplacementId = 35, ReplacementProgramDescription = "Brushing teeth" },
        new ReplacementProgram { ReplacementId = 36, ReplacementProgramDescription = "Washing and Drying face" },
        new ReplacementProgram { ReplacementId = 37, ReplacementProgramDescription = "Follow Night time routing" },
        new ReplacementProgram { ReplacementId = 38, ReplacementProgramDescription = "Dressing pant" },
        new ReplacementProgram { ReplacementId = 39, ReplacementProgramDescription = "Dressing Pullover" },
        new ReplacementProgram { ReplacementId = 40, ReplacementProgramDescription = "Eating with spoon" },
        new ReplacementProgram { ReplacementId = 41, ReplacementProgramDescription = "Keep eye contact" },
        new ReplacementProgram { ReplacementId = 42, ReplacementProgramDescription = "Toilet training" });

      modelBuilder.Entity<CaregiverType>().HasData(
        new CaregiverType { CaregiverTypeId = 1, Description = "Parent" },
        new CaregiverType { CaregiverTypeId = 2, Description = "Teacher" },
        new CaregiverType { CaregiverTypeId = 3, Description = "Related" },
        new CaregiverType { CaregiverTypeId = 4, Description = "Friend" },
        new CaregiverType { CaregiverTypeId = 5, Description = "Other" });
      modelBuilder.Entity<DocumentGroup>().HasData(
        new DocumentGroup { DocumentGroupId = 1, GroupName = "Legal" },
        new DocumentGroup { DocumentGroupId = 2, GroupName = "Personal Documents" },
        new DocumentGroup { DocumentGroupId = 3, GroupName = "Clearinghouse" },
        new DocumentGroup { DocumentGroupId = 4, GroupName = "Current work Documents" },
        new DocumentGroup { DocumentGroupId = 5, GroupName = "Licenses and Certifications" },
        new DocumentGroup { DocumentGroupId = 6, GroupName = "Inservices" },
        new DocumentGroup { DocumentGroupId = 7, GroupName = "Other and miscellaneous" });
      modelBuilder.Entity<Document>().HasData(
          new Document { DocumentId = 1, DocumentGroupId = 1, DocumentName = "Social security", DocumentExpires = false },
          new Document { DocumentId = 2, DocumentGroupId = 1, DocumentName = "Proof Legal status (US citizen, Lawful Permanent Resident, Work Permit, etc.)", DocumentExpires = true },
          new Document { DocumentId = 3, DocumentGroupId = 1, DocumentName = "Driver License", DocumentExpires = true },
          new Document { DocumentId = 4, DocumentGroupId = 1, DocumentName = "I-9 Form: Employment Eligibility Verification", DocumentExpires = true },
          new Document { DocumentId = 5, DocumentGroupId = 1, DocumentName = "E-Verify", DocumentExpires = false },
          new Document { DocumentId = 6, DocumentGroupId = 1, DocumentName = "W9", DocumentExpires = false },
          new Document { DocumentId = 7, DocumentGroupId = 1, DocumentName = "Attestation of Compliance with Background Screening Requirements", DocumentExpires = true },
          new Document { DocumentId = 8, DocumentGroupId = 1, DocumentName = "Privacy Policy Acknowledgment Form", DocumentExpires = false },
          new Document { DocumentId = 9, DocumentGroupId = 1, DocumentName = "Liability", DocumentExpires = true },
          new Document { DocumentId = 10, DocumentGroupId = 1, DocumentName = "Confidentiality Agreement", DocumentExpires = false },
          new Document { DocumentId = 11, DocumentGroupId = 1, DocumentName = "HIPPA Privacy & Confidentiality Statement", DocumentExpires = false },
          new Document { DocumentId = 12, DocumentGroupId = 1, DocumentName = "Abuse Reporting Policies", DocumentExpires = false },
          new Document { DocumentId = 13, DocumentGroupId = 1, DocumentName = "Commitment to educate about report abuse/neglect/explo/mis conduct", DocumentExpires = false },
          new Document { DocumentId = 14, DocumentGroupId = 1, DocumentName = "Bill of Rights", DocumentExpires = false },
          new Document { DocumentId = 15, DocumentGroupId = 1, DocumentName = "Managed care FRAUD and ABUSE", DocumentExpires = false },
          new Document { DocumentId = 16, DocumentGroupId = 1, DocumentName = "Affidavit Good Moral Charter (Notarized)", DocumentExpires = false },

          new Document { DocumentId = 17, DocumentGroupId = 2, DocumentName = "Employee General Information Form and Emergency Information", DocumentExpires = false },
          new Document { DocumentId = 18, DocumentGroupId = 2, DocumentName = "Resume", DocumentExpires = false },
          new Document { DocumentId = 19, DocumentGroupId = 2, DocumentName = "References 2 letter (previous employ)", DocumentExpires = false },
          new Document { DocumentId = 20, DocumentGroupId = 2, DocumentName = "Physician’s Health Certification TB test", DocumentExpires = true },
          new Document { DocumentId = 21, DocumentGroupId = 2, DocumentName = "Alcohol and drugs test", DocumentExpires = true },
          new Document { DocumentId = 22, DocumentGroupId = 2, DocumentName = "Car registration", DocumentExpires = true },
          new Document { DocumentId = 23, DocumentGroupId = 2, DocumentName = "Car insurance", DocumentExpires = true },

          new Document { DocumentId = 24, DocumentGroupId = 3, DocumentName = "Police record Footprints ACHA Level II", DocumentExpires = true },
          new Document { DocumentId = 25, DocumentGroupId = 3, DocumentName = "Local Police Background Check", DocumentExpires = true },
          new Document { DocumentId = 26, DocumentGroupId = 3, DocumentName = "OIG (results)", DocumentExpires = true },

          new Document { DocumentId = 27, DocumentGroupId = 4, DocumentName = "Application Form", DocumentExpires = false },
          new Document { DocumentId = 28, DocumentGroupId = 4, DocumentName = "Group Membership Authorization", DocumentExpires = false },
          new Document { DocumentId = 29, DocumentGroupId = 4, DocumentName = "Non-Institutional Medicaid Provider Agreements", DocumentExpires = false },
          new Document { DocumentId = 30, DocumentGroupId = 4, DocumentName = "Workplace Expectation", DocumentExpires = false },
          new Document { DocumentId = 31, DocumentGroupId = 4, DocumentName = "Job Description", DocumentExpires = false },
          new Document { DocumentId = 32, DocumentGroupId = 4, DocumentName = "Employment Statement of Commitment", DocumentExpires = false },
          new Document { DocumentId = 33, DocumentGroupId = 4, DocumentName = "Code of Professional Conduct", DocumentExpires = false },
          new Document { DocumentId = 34, DocumentGroupId = 4, DocumentName = "Policy on Jobs", DocumentExpires = false },
          new Document { DocumentId = 35, DocumentGroupId = 4, DocumentName = "Employment Agreement / Independent/Agreement", DocumentExpires = false },
          new Document { DocumentId = 36, DocumentGroupId = 4, DocumentName = "Employee Acknowledgement of Probationary Period Form", DocumentExpires = false },
          new Document { DocumentId = 37, DocumentGroupId = 4, DocumentName = "Disclosures", DocumentExpires = false },
          new Document { DocumentId = 38, DocumentGroupId = 4, DocumentName = "Employee Health release for denial TB", DocumentExpires = false },
          new Document { DocumentId = 39, DocumentGroupId = 4, DocumentName = "Telephone reference check form", DocumentExpires = false },

          new Document { DocumentId = 40, DocumentGroupId = 5, DocumentName = "Provider", DocumentExpires = true },
          new Document { DocumentId = 41, DocumentGroupId = 5, DocumentName = "ATN", DocumentExpires = false },
          new Document { DocumentId = 42, DocumentGroupId = 5, DocumentName = "NPI", DocumentExpires = false },
          new Document { DocumentId = 43, DocumentGroupId = 5, DocumentName = "Credentials", DocumentExpires = true },
          new Document { DocumentId = 44, DocumentGroupId = 5, DocumentName = "RBT", DocumentExpires = false },
          new Document { DocumentId = 45, DocumentGroupId = 5, DocumentName = "BCaBA", DocumentExpires = false },
          new Document { DocumentId = 46, DocumentGroupId = 5, DocumentName = "Analyst", DocumentExpires = false },
          new Document { DocumentId = 47, DocumentGroupId = 5, DocumentName = "Licenses", DocumentExpires = false },
          new Document { DocumentId = 48, DocumentGroupId = 5, DocumentName = "Diplomas", DocumentExpires = false },
          new Document { DocumentId = 49, DocumentGroupId = 5, DocumentName = "Transcripts", DocumentExpires = false },
          new Document { DocumentId = 50, DocumentGroupId = 5, DocumentName = "Attestation Form for Behavior Assistant", DocumentExpires = false },

          new Document { DocumentId = 51, DocumentGroupId = 6, DocumentName = "DCC (Direct Care Core Competences) (course ID No: 1060050)", DocumentExpires = true },
          new Document { DocumentId = 52, DocumentGroupId = 6, DocumentName = "Zero Tolerance (course ID No: 1058718)", DocumentExpires = true },
          new Document { DocumentId = 53, DocumentGroupId = 6, DocumentName = "Requirements for all Waiver Providers & Incident Reporting (course ID No: 1060049)", DocumentExpires = true },
          new Document { DocumentId = 54, DocumentGroupId = 6, DocumentName = "Learner Orientation Provider/Customer (course ID No: 1060049)", DocumentExpires = true },
          new Document { DocumentId = 55, DocumentGroupId = 6, DocumentName = "AIDs/HIV and Blood borne Pathogens (course ID No: 1059884)", DocumentExpires = true },
          new Document { DocumentId = 56, DocumentGroupId = 6, DocumentName = "CPR/AED/BASIC FIRST AIDS", DocumentExpires = true },
          new Document { DocumentId = 57, DocumentGroupId = 6, DocumentName = "HIPAA Basics (course ID No: 1058936)", DocumentExpires = true },
          new Document { DocumentId = 58, DocumentGroupId = 6, DocumentName = "Security Awareness", DocumentExpires = true },
          new Document { DocumentId = 59, DocumentGroupId = 6, DocumentName = "Access Civil Rights or Patient´s Rights", DocumentExpires = true },
          new Document { DocumentId = 60, DocumentGroupId = 6, DocumentName = "OSHA", DocumentExpires = true },
          new Document { DocumentId = 61, DocumentGroupId = 6, DocumentName = "Infection Control", DocumentExpires = true },
          new Document { DocumentId = 62, DocumentGroupId = 6, DocumentName = "Patient´s Rights OR Access Civil Rights", DocumentExpires = true },
          new Document { DocumentId = 63, DocumentGroupId = 6, DocumentName = "Domestic Violence", DocumentExpires = true },
          new Document { DocumentId = 64, DocumentGroupId = 6, DocumentName = "Med Error Prevention", DocumentExpires = true },
          new Document { DocumentId = 65, DocumentGroupId = 6, DocumentName = "Med Record Documentation", DocumentExpires = true },
          new Document { DocumentId = 66, DocumentGroupId = 6, DocumentName = "Abuse/Neglect/Exploitation", DocumentExpires = true });

      modelBuilder.Entity<CompetencyCheckParam>().HasData(
          new CompetencyCheckParam { CompetencyCheckParamId = 1, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Collected data on problem behaviors which ocurred in session", Comment = "Behavior(s) occurrences" },
          new CompetencyCheckParam { CompetencyCheckParamId = 2, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Followed recommended intervention procedures upon occurrence of problem behaviore in session", Comment = "Interventions(s) used for problem behaviors" },
          new CompetencyCheckParam { CompetencyCheckParamId = 3, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Implemented the replacement skills training programs and recorded the data", Comment = "Replacement(s) skill specific for problem behavior" },
          new CompetencyCheckParam { CompetencyCheckParamId = 4, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Implemented the acquisition skills training proramas and recorded the data", Comment = "Acquisition(s) skill" },
          new CompetencyCheckParam { CompetencyCheckParamId = 5, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Provided reinforcement in accordance with the program", Comment = "Reinforcer(s) used for replacement/acquisition training" },
          new CompetencyCheckParam { CompetencyCheckParamId = 6, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Demostrated to Behavior assistant how to implement a portion of the behavior program", Comment = "BASP Portion selected to describe and review" },
          new CompetencyCheckParam { CompetencyCheckParamId = 7, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Can indicate when the behavior occurred in another setting or with another person and the program was effectively implemented (Programming forgeneralization)", Comment = "List new setting/person. Describe generalization example during month" },
          new CompetencyCheckParam { CompetencyCheckParamId = 8, CompetencyCheckType = CompetencyCheckType.Caregiver, Description = "Verbally reported about major changes in the environment, daily activity schedule, medical status, or severity of problem behaviors, along with new behaviors occurring, and difficulty implementing behavior plan as is", Comment = "Medical status/Environmental changes/New Problem Behavior/Severity of problem behavior" },

          new CompetencyCheckParam { CompetencyCheckParamId = 9, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Collected data on problem behaviors which ocurred in session", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 10, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Followed recommended intervention procedures upon occurrence of problem behaviore in session", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 11, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Provided reinforcement in accordance with the program", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 12, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Implemented the replacement skills training programs and recorded the data", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 13, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Modeled to caregiver how to implement a portion of the behavior program", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 14, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Can indicate when the behavior occurred in another setting or with another person and the program was effectively implemented (Programming forgeneralization)", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 15, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Data collected to date permit to be graphed", Comment = "" },
          new CompetencyCheckParam { CompetencyCheckParamId = 16, CompetencyCheckType = CompetencyCheckType.Rbt, Description = "Daily progress notes reviewed", Comment = "" });
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<ProblemBehavior> ProblemBehaviors { get; set; }
    public DbSet<ReplacementProgram> ReplacementPrograms { get; set; }
    // public DbSet<ClientProblem> ClientsProblems { get; set; }
    // public DbSet<ClientReplacement> ClientsReplacements { get; set; }
    public DbSet<CaregiverType> CaregiversType { get; set; }
    public DbSet<Caregiver> Caregivers { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Diagnosis> Diagnostics { get; set; }
    public DbSet<ClientDiagnosis> ClientDiagnostics { get; set; }
    public DbSet<DocumentGroup> DocumentGroups { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentUser> DocumentsUsers { get; set; }
    public DbSet<BehaviorAnalysisCode> BehaviorAnalysisCodes { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<SessionNote> SessionNotes { get; set; }
    // public DbSet<SessionProblem> SessionsProblems { get; set; }
    // public DbSet<ReplacementByProblem> ReplacementsByProblems { get; set; }
    public DbSet<MonthlyNote> MonthlyNotes { get; set; }
    public DbSet<Email> Emails { get; set; }
    //public DbSet<MetricSessionProblem> MetricSessionProblems { get; set; }
    // public DbSet<MetricSessionReplacement> MetricSessionReplacements { get; set; }
    public DbSet<ClientChartNote> ClientChartNotes { get; set; }
    public DbSet<CompetencyCheckParam> CompetencyCheckParams { get; set; }
    public DbSet<CompetencyCheck> CompetencyChecks { get; set; }
    public DbSet<CompetencyCheckClientParam> CompetencyCheckClientParams { get; set; }
    public DbSet<SessionSupervisionNote> SessionSupervisionNotes { get; set; }
    public DbSet<UserSign> UserSigns { get; set; }
    public DbSet<SessionCollectBehavior> SessionCollectBehaviors { get; set; }
    public DbSet<SessionCollectReplacement> SessionCollectReplacements { get; set; }
    public DbSet<SessionProblemNote> SessionProblemNotes { get; set; }
    public DbSet<SessionProblemNoteReplacement> SessionProblemNoteReplacements { get; set; }

    public DbSet<ClientProblem> ClientProblems { get; set; }
    public DbSet<ClientReplacement> ClientReplacements { get; set; }
    public DbSet<ClientProblemSto> ClientProblemSTOs { get; set; }
    public DbSet<ClientReplacementSto> ClientReplacementSTOs { get; set; }
    public DbSet<SessionLog> SessionLogs { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }

  }
}