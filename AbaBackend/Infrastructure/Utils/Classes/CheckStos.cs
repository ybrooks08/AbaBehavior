using System.Threading.Tasks;

namespace AbaBackend.Infrastructure.Utils.Classes
{
  public class CheckStos
  {
    public DataModel.AbaDbContext _dbContext { get; }

    public CheckStos(DataModel.AbaDbContext dbContext) => _dbContext = dbContext;

    public async Task ProcessStos()
    {
      await Task.Delay(1);
    }
  }
}