using System.Threading.Tasks;

namespace AbaBackend.Infrastructure.StoProcess
{
  public interface IStoProcess
  {
    Task ProcessStos();
    Task ProccessClientBehavior(int clientId);
    Task ProccessClientReplacement(int clientId);
  }
}