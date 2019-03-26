using System;

namespace AbaBackend.Model.MasterTables
{
  public class ChangeDate
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }
  }
  
  public class ChangeDateNull
  {
    public int Id { get; set; }
    public DateTime? Date { get; set; }
  }
}