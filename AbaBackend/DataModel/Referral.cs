using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AbaBackend.DataModel
{
  public class Referral
  {
    public int ReferralId { get; set; }

    public int ClientId { get; set; }

    // [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public Client Client { get; set; }

    [Required]
    [MaxLength(60)]
    public string ReferralFullname { get; set; }

    [Required]
    [MaxLength(60)]
    public string Specialty { get; set; }

    [MaxLength(20)]
    public string License { get; set; }

    [MaxLength(60)]
    public string Provider { get; set; }

    [MaxLength(20)]
    public string Npi { get; set; }

    [MaxLength(120)]
    public string FullAddress { get; set; }

    [MaxLength(15)]
    public string Phone { get; set; }

    [MaxLength(15)]
    public string Fax { get; set; }

    [MaxLength(60)]
    public string Email { get; set; }

    public DateTime DateReferral { get; set; } = DateTime.Today;

    public DateTime DateExpires { get; set; } = DateTime.Today.AddYears(1);

    public bool Active { get; set; } = true;
  }
}