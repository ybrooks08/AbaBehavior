using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class DocumentGroup
  {
    public int DocumentGroupId { get; set; }

    [Required]
    [MaxLength(100)]
    public string GroupName { get; set; }

    public List<Document> Documents { get; set; }
  }
}