using System;

namespace AbaBackend.DataModel
{
  public class DocumentUser
  {
    public int Id { get; set; }

    public int UserId { get; set; }

    public int DocumentId { get; set; }

    public bool Active { get; set; }

    public DateTime? Expires { get; set; }

    public User User { get; set; }

    public Document Document { get; set; }
  }
}