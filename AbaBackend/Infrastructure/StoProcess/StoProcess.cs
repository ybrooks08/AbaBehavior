using System;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Collection;
using AbaBackend.Infrastructure.Extensions;
using AbaBackend.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace AbaBackend.Infrastructure.StoProcess
{
  public class StoProcess : IStoProcess
  {
    AbaDbContext _dbContext;
    IUtils _utils;
    ICollection _collection;

    public StoProcess(AbaDbContext dbContext, IUtils utils, ICollection collection)
    {
      _dbContext = dbContext;
      _utils = utils;
      _collection = collection;
    }

    public async Task ProcessStos()
    {
      var clients = await _dbContext.Clients.Where(w => w.Active).ToListAsync();
      foreach (var client in clients)
      {
        await ProccessClientBehavior(client.ClientId);
        await ProccessClientReplacement(client.ClientId);
      }
    }

    public async Task ProccessClientBehavior(int clientId)
    {
      var (_, start, end) = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end;
      while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday) lastWeekEnd = lastWeekEnd.AddDays(1);

      var clientBehaviors = await _utils.GetClientBehaviors(clientId, false);
      if (clientBehaviors.Count == 0) return;
      var clientBehaviorsListId = clientBehaviors.Select(s => s.ProblemId).ToList();
      var allStos = await _dbContext.ClientProblemSTOs.Where(w => !w.MasteredForced && clientBehaviors.Select(s => s.ClientProblemId).Contains(w.ClientProblemId)).ToListAsync();
      allStos.ForEach(f => f.Status = StoStatus.Unknow);

      var allCollection = await _collection.GetCollectionBehaviors(firstWeekStart, lastWeekEnd, clientId, clientBehaviorsListId);
      var allCaregiverCollection = await _collection.GetCollectionBehaviorsCaregiver(firstWeekStart, lastWeekEnd, clientId, clientBehaviorsListId);

      foreach (var clientBehavior in clientBehaviors)
      {
        var valuesByWeek = _collection.GetClientProblemsByWeek(clientBehavior.ProblemId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection, clientBehavior.ProblemBehavior.IsPercent);
        var stos = allStos.Where(w => w.ClientProblemId == clientBehavior.ClientProblemId).OrderBy(o => o.ClientProblemStoId).ToList();
        if (stos.Count == 0) continue;

        foreach (var sto in stos)
        {
          var qty = sto.Quantity;
          var weeks = sto.Weeks;

          var e = false;
          do
          {
            var checkWeeks = valuesByWeek.Take(weeks).Count(w => w.Total <= qty);
            var allWeeks = valuesByWeek.Take(weeks).ToList();
            if (checkWeeks >= weeks)
            {
              sto.WeekStart = allWeeks.First().Start;
              sto.WeekEnd = allWeeks.Last().End;
              sto.Status = StoStatus.Mastered;
              valuesByWeek.RemoveRange(0, weeks);
              e = true;
            }
            else if (valuesByWeek.Count > 0) valuesByWeek.RemoveRange(0, 1);
            if (valuesByWeek.Count <= 0) e = true;
          } while (!e);

          if (valuesByWeek.Count == 0) break;

        }
      }
      await _dbContext.SaveChangesAsync();
    }

    public async Task ProccessClientReplacement(int clientId)
    {
      var (_, start, end) = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end;
      while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday) lastWeekEnd = lastWeekEnd.AddDays(1);

      var clientReplacements = await _utils.GetClientReplacements(clientId, false);
      if (clientReplacements.Count == 0) return;
      var clientReplacementsListId = clientReplacements.Select(s => s.ReplacementId).ToList();
      var allStos = await _dbContext.ClientReplacementSTOs.Where(w => !w.MasteredForced && clientReplacements.Select(s => s.ClientReplacementId).Contains(w.ClientReplacementId)).ToListAsync();
      allStos.ForEach(f => f.Status = StoStatus.Unknow);

      var allCollection = await _collection.GetCollectionReplacements(firstWeekStart, lastWeekEnd, clientId, clientReplacementsListId);
      var allCaregiverCollection = await _collection.GetCollectionReplacementsCaregiver(firstWeekStart, lastWeekEnd, clientId, clientReplacementsListId);

      foreach (var clientReplacement in clientReplacements)
      {
        var valuesByWeek = _collection.GetClientReplacementsByWeek(clientReplacement.ReplacementId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection);
        var stos = allStos.Where(w => w.ClientReplacementId == clientReplacement.ClientReplacementId).OrderBy(o => o.ClientReplacementStoId).ToList();
        if (stos.Count == 0) continue;

        foreach (var sto in stos)
        {
          var qty = sto.Percent;
          var weeks = sto.Weeks;

          var e = false;
          do
          {
            var checkWeeks = valuesByWeek.Take(weeks).Count(w => w.Total >= qty);
            var allWeeks = valuesByWeek.Take(weeks).ToList();
            if (checkWeeks >= weeks)
            {
              sto.WeekStart = allWeeks.First().Start;
              sto.WeekEnd = allWeeks.Last().End;
              sto.Status = StoStatus.Mastered;
              valuesByWeek.RemoveRange(0, weeks);
              e = true;
            }
            else if (valuesByWeek.Count > 0) valuesByWeek.RemoveRange(0, 1);
            if (valuesByWeek.Count <= 0) e = true;
          } while (!e);

          if (valuesByWeek.Count == 0) break;

        }
      }
      await _dbContext.SaveChangesAsync();
    }
  }
}