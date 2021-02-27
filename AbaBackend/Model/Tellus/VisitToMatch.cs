
using AbaBackend.DataModel;

namespace AbaBackend.Model.Tellus
{
public class VisitToMatch
  {
    public string SessionId { get; set; }
    public string UserId { get; set; }
    public string ClientId { get; set; }
    public string ClientFullname { get; set; }
    //Client code
    public string Code { get; set; }
    public string SessionStart { get; set; }
    public string SessionEnd { get; set; }
    public int TotalUnits { get; set; }
    public string SessionType { get; set; }
    public string SessionStatus { get; set; }
    //public SessionStatus SessionStatusCode { get; set; }
    public string SessionStatusCode { get; set; }
    public string SessionStatusColor { get; set; }
    public string Pos { get; set; }
    //public Pos PosCode { get; set; }
    public string PosCode { get; set; }
    public string UserFullname { get; set; }
    public string Rol { get; set; }
    public bool Edit { get; set; }
    public bool Difference { get; set; }
    public string MedicaidId { get; set; }
    //Provider medicaid or provider id
    public string Mpi { get; set; }
  }
}
