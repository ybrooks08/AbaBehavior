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
    public static void CustomSeed(IHostingEnvironment env, AbaDbContext context)
    {
      if (!context.Diagnostics.Any())
      {
        try
        {
          var file = Path.Combine(env.WebRootPath, "seed/diagnosis.txt");
          var lines = File.ReadAllLines(file);
          Console.WriteLine($"Seeding {lines.Length} diagnostics...");
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
          Console.WriteLine("Done!");
        }
        catch (System.Exception e)
        {
          Console.WriteLine("ERROR SEEDING: " + e.Message);
        }

      }

    }

  }
}