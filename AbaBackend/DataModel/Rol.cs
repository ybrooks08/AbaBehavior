using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class Rol
  {
    public int RolId { get; set; }

    [Required]
    [MaxLength(100)]
    public string RolName { get; set; }

    [Required]
    [MaxLength(20)]
    public string RolShortName { get; set; }

    public bool CanCreateSession { get; set; }

    public bool CanEditAllClientSession { get; set; }

    public bool HasDocuments { get; set; }

    public int? BehaviorAnalysisCodeId { get; set; } = null;

    [MaxLength(20)]
    public string TemplateName { get; set; }

    public BehaviorAnalysisCode BehaviorAnalysisCode { get; set; }

    //public List<User> Users { get; set; }
  }
}