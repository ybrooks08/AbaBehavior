namespace AbaBackend.Model.Client
{
  public class AddRemoveClientReplacement
  {
    public int ClientId { get; set; }
    public int ReplacementId { get; set; }
    public char Action { get; set; }
  }
}