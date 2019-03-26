using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AbaBackend.DataModel
{
  // [JsonObject(IsReference = true)]
  public class Client
  {
    public int ClientId { get; set; }
    [MaxLength(10)]
    public string Code { get; set; }
    [Required]
    [MaxLength(60)]
    public string Firstname { get; set; }
    [Required]
    [MaxLength(100)]
    public string Lastname { get; set; }
    [MaxLength(30)]
    public string Nickname { get; set; }
    [Required]
    public DateTime Dob { get; set; }
    [MaxLength(15)]
    public string Phone { get; set; }
    [MaxLength(60)]
    public string Email { get; set; }
    [Required]
    [MaxLength(100)]
    public string Address { get; set; }
    [MaxLength(10)]
    public string Apt { get; set; }
    [Required]
    [MaxLength(50)]
    public string City { get; set; }
    [Required]
    [MaxLength(50)]
    public string State { get; set; }
    [Required]
    [MaxLength(10)]
    public string Zipcode { get; set; }
    [MaxLength(15)]
    public string Gender { get; set; }
    [MaxLength(60)]
    public string Race { get; set; }
    [MaxLength(20)]
    public string PrimaryLanguage { get; set; }
    [MaxLength(150)]
    public string EmergencyContact { get; set; }
    [MaxLength(15)]
    public string EmergencyPhone { get; set; }
    [MaxLength(60)]
    public string EmergencyEmail { get; set; }
    public string Notes { get; set; }
    [MaxLength(11)]
    public string SocialSecurity { get; set; }
    [MaxLength(50)]
    public string Insurance { get; set; }
    [MaxLength(50)]
    public string MemberNo { get; set; }
    [MaxLength(50)]
    public string MmaPlan { get; set; }
    [MaxLength(50)]
    public string MmaIdNo { get; set; }
    public bool Active { get; set; } = true;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public List<Caregiver> Caregivers { get; set; }
    // [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public List<Referral> Referrals { get; set; }
    public List<ClientDiagnosis> ClientDiagnostics { get; set; }
    public List<Assignment> Assignments { get; set; }
    public List<Assessment> Assessments { get; set; }
    public List<MonthlyNote> MonthlyNotes { get; set; }

  }
}
