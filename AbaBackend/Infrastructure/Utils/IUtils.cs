using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Model.Session;

namespace AbaBackend.Infrastructure.Utils
{
  public interface IUtils
  {
    Task<User> GetUserByUsername(string username);
    Task<User> GetCurrentUser();
    Task<Client> GetClientById(int clientId);
    decimal ConvertTimeToNumber(DateTime time);
    Task<bool> CheckAssignmentClientActive(int userId, int clientId);
    Task<List<Assessment>> GetClientValidAssessmentsForUser(DateTime date, int clientId, User user);
    Task<string> CheckMaxHoursClientsInDay(DateTime date, int clientId, int totalUnits);
    Task<string> CheckMaxHoursUserInDay(DateTime date, int userId, int totalUnits);
    Task<string> CheckMaxHoursUserByClientInDay(DateTime date, int userId, int clientId, int totalUnits);
    Task<string> CheckMaxHoursByClientInSchool(DateTime date, int userId, int clientId, int totalUnits);
    Task<int> GetUnitsAvailable(DateTime? date, int clientId, User user);
    Task<bool> CreateEmail(string to, string subject, string message, MessageType messageType);
    Task MidNightProcess();
    Task<bool> SendEmailsAsync(bool sendGlobal = true);
    // Task<ChartNoteData> GetNotesForChart(int clientId, int lastWeeks, ChartNoteType chartNoteType);
    Task<string> CheckUserSessionOverlap(DateTime dateStart, DateTime dateEnd, int userId);
    Task<string> CheckSessionOverlapSameDayClient(DateTime dateStart, DateTime dateEnd, int clientId);
    Task<(string Color, string Text)> ChangeSessionStatus(ChangeSessionStatus changeSessionStatus);
    Task<int> GetUserExpiredDocumentsCount(int userId = 0);
    Task UpdateClientProblemStos(int clientProblemId);
    Task UpdateClientReplacementStos(int periodClientReplacementId);
    Task<(bool isValid, DateTime start, DateTime end)> GetClientWholePeriod(int clientId);
    Task<List<ClientProblem>> GetClientBehaviors(int clientId);
    Task<List<ClientReplacement>> GetClientReplacements(int clientId);
    Task AddSessionProblemNotes(int sessionId, int clientId);
    Task NewEntryLog(int sessionId, string title, string description, string icon = "fa-info-circle", string iconColor = "blue");
    Task<Object> GetCompetencyCheckChart(int clientId, CompetencyCheckType competencyCheckType, int userOrCaregiverId);
    Task NewSystemLog(SystemLogType logType, Module module, int moduleId, string title, string description);
    Task<string> GetFullDataForSystemLog(Module who, int valueId);
    Task<User> GetUserById(int userId);
  }
}