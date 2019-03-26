using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class Document
  {
    public int DocumentId { get; set; }

    public int DocumentGroupId { get; set; }

    [Required]
    [MaxLength(200)]
    public string DocumentName { get; set; }

    public bool DocumentExpires { get; set; }

    public DocumentGroup DocumentGroup { get; set; }
  }
}