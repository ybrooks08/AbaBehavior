using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace AbaBackend.DataModel
{
  public class DbInitializer
  {
    public static void CustomSeed(IHostingEnvironment env, AbaDbContext context, ILogger _logger)
    {
      if (!context.Diagnostics.Any())
      {
        try
        {
          var file = Path.Combine(env.WebRootPath, "seed/diagnosis.txt");
          var lines = File.ReadAllLines(file);
          _logger.LogWarning($"Seeding {lines.Length} diagnostics...");
          foreach (var l in lines)
          {
            var obj = l.Replace("\"", "").Split('\t');
            var diag = new Diagnosis
            {
              Code = obj[0],
              Description = obj[1]
            };
            context.Add(diag);
          }
          context.SaveChanges();
          _logger.LogWarning($"Done!");
        }
        catch (System.Exception e)
        {
          _logger.LogCritical("ERROR SEEDING: " + e.Message);
        }

      }

    }

  }
}