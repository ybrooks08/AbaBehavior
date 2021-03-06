using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbaBackend.DataModel
{
  public enum SessionType
  {
    BA_Service = 1,
    Caregiver_Trainer = 2,
    Training_BCABA = 3
  }

  public enum Pos
  {
    Telemedicine = 2,
    School = 3,
    Office = 11,
    Home = 12,
    Assistance_Living_Facility = 13,
    Group_Home = 14,
    Out_Patient_Hospital = 22,
    Inpatient_Clinic = 49,
    Community_Mental_Health_Center = 53,
    Comprensive_Outpatient_Rehabilitation_Facility = 62,
    Other = 99
  }

  public enum SessionStatus
  {
    Created, //when create new session
    Edited, //edited the session
    Rejected, //only from service log
    Cancelled, // N/A for now
    Reopen, // maybe when delete the whole service log all session with checked or billed go to waiting
    Checked, //ready to be billed or signed
    Billed, //last stage of the session. CAN NOT UNDO
    Reviewed,
    RbtReady
  }

  public enum SessionStatusColors
  {
    purple,
    blue,
    red,
    orange,
    cyan,
    teal,
    brown,
    lime,
    amber
  }

  public class Session
  {
    public int SessionId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
    public int TotalUnits { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal DriveTime { get; set; }

    public SessionType SessionType { get; set; }
    public Pos Pos { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public int BehaviorAnalysisCodeId { get; set; }
    public User User { get; set; }
    public Client Client { get; set; }
    public SessionStatus SessionStatus { get; set; } = SessionStatus.Created;
    public BehaviorAnalysisCode BehaviorAnalysisCode { get; set; }
    public SessionNote SessionNote { get; set; }
    public SessionSupervisionNote SessionSupervisionNote { get; set; }
    public List<SessionProblemNote> SessionProblemNotes { get; set; }
    public List<SessionLog> SessionLogs { get; set; }
    public SessionSign Sign { get; set; }
    public List<SessionCollectBehaviorV2> SessionCollectBehaviorsV2 { get; set; }
    public List<SessionCollectReplacementV2> SessionCollectReplacementsV2 { get; set; }
    public int? SessionAnalystId { get; set; }
    [ForeignKey("SessionAnalystId")]
    public User SessionAnalyst { get; set; }
    /// <summary>
    /// Tellus matched property
    /// </summary>
    public bool Matched { get; set; } = false;

  }
}