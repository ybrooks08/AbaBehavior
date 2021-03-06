using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AbaBackend.Infrastructure.Reporting.Sessions
{
  public interface ICompetencyCheckReport
  {
    Task<ExcelPackage> CreateReport(int competencyCheckId, FileInfo file = null);
  }

  public class CompetencyCheckReport : ICompetencyCheckReport
  {
    private AbaDbContext _dbContext { get; }
    private ExcelPackage _excelPackage;
    private ExcelWorksheet _wsMain;
    private readonly IUtils _utils;

    public CompetencyCheckReport(AbaDbContext dbContext, IUtils utils)
    {
      _utils = utils;
      _dbContext = dbContext;
    }

    public async Task<ExcelPackage> CreateReport(int competencyCheckId, FileInfo file = null)
    {
      PrepareReport();

      var compCheck = await _dbContext.CompetencyChecks
                            .Include(i => i.Client)
                            .Include(i => i.CompetencyCheckClientParams).ThenInclude(i => i.CompetencyCheckParam)
                            .Include(i => i.User).ThenInclude(i => i.UserSign)
                            .Include(i => i.Caregiver)
                            .Include(i => i.EvaluationBy).ThenInclude(i => i.UserSign)
                            .FirstOrDefaultAsync(a => a.CompetencyCheckId.Equals(competencyCheckId));

      var dx = await _utils.GetClientDiagnosisByDate(compCheck.Date, compCheck.ClientId);

      FixedCells();
      var row = ParametersCells();
      row = ScoreCell(row);
      row = await SignCell(row);
      FinalizeReport(row);

      if (file != null) _excelPackage.SaveAs(file);
      return _excelPackage;

      void PrepareReport()
      {
        _excelPackage = new ExcelPackage();
        _wsMain = _excelPackage.Workbook.Worksheets.Add("CompCheck");
        _wsMain.Cells.Style.Font.Size = 11;
        _wsMain.PrinterSettings.TopMargin = 0.5m;
        _wsMain.PrinterSettings.BottomMargin = 0.5m;
        _wsMain.PrinterSettings.LeftMargin = 0.7m;
        _wsMain.PrinterSettings.RightMargin = 0.3m;
        _wsMain.Column(1).Width = 14;//ReportingTools.GetTrueColumnWidth(23);
        _wsMain.Column(2).Width = 30;//ReportingTools.GetTrueColumnWidth(21);
        _wsMain.Column(3).Width = 43;//ReportingTools.GetTrueColumnWidth(40);
      }

      void FixedCells()
      {
        ReportingTools.MergeCells(_wsMain, 1, 1, 1, 3);
        ReportingTools.PutDataInSheet(_wsMain, 1, 1, $"{compCheck.CompetencyCheckType} Competency Check (By Lead Analyst)", ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center, Color.Black, Color.White);
        _wsMain.Cells[1, 1].Style.Font.Size = 13;
        _wsMain.Cells[1, 1].Style.Font.Bold = true;

        ReportingTools.MergeCells(_wsMain, 2, 1, 2, 2);
        _wsMain.Row(2).Height = 65;
        var clientCell = _wsMain.Cells[2, 1];
        clientCell.Style.WrapText = true;
        clientCell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        clientCell.RichText.Add($"Client: {compCheck.Client.Firstname} {compCheck.Client.Lastname}\r\n");
        clientCell.RichText.Add($"Dob: {compCheck.Client.Dob.ToShortDateString()}\r\n");
        clientCell.RichText.Add($"Medicaid: {compCheck.Client.MemberNo}\r\n");
        clientCell.RichText.Add($"Code: {compCheck.Client.Code}\r\n");

        var clientCell2 = _wsMain.Cells[2, 3];
        clientCell2.Style.WrapText = true;
        clientCell2.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        clientCell2.RichText.Add($"Total duration (this month): {compCheck.TotalDuration} hrs\r\n");
        foreach (var (Code, Description) in dx)
        {
          clientCell2.RichText.Add($"{Code}-{Description}\r\n");
        }

        // ReportingTools.PutDataInSheet(_wsMain, 2, 1, $"Client: {compCheck.Client.Firstname} {compCheck.Client.Lastname}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center);
        // ReportingTools.PutDataInSheet(_wsMain, 2, 3, $"Total duration (this month): {compCheck.TotalDuration} hrs", ExcelHorizontalAlignment.Right, ExcelVerticalAlignment.Center);

        ReportingTools.PutDataInSheet(_wsMain, 3, 1, $"Date", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center, Color.Gray, Color.White);
        ReportingTools.PutDataInSheet(_wsMain, 3, 2, $"Competency to ({compCheck.CompetencyCheckType} name)", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center, Color.Gray, Color.White);
        ReportingTools.PutDataInSheet(_wsMain, 3, 3, $"Evaluation by", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center, Color.Gray, Color.White);

        ReportingTools.PutDataInSheet(_wsMain, 4, 1, compCheck.Date.ToShortDateString(), ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center);
        ReportingTools.PutDataInSheet(_wsMain, 4, 2, compCheck.CompetencyCheckType == CompetencyCheckType.Rbt ? $"{compCheck.User.Firstname} {compCheck.User.Lastname}" : $"{compCheck.Caregiver.CaregiverFullname}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center);
        ReportingTools.PutDataInSheet(_wsMain, 4, 3, $"{compCheck.EvaluationBy.Firstname} {compCheck.EvaluationBy.Lastname}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center);
      }

      int ParametersCells()
      {
        var parameters = compCheck.CompetencyCheckClientParams.Where(w => w.CompetencyCheckParam.CompetencyCheckType.Equals(compCheck.CompetencyCheckType)).OrderBy(o => o.CompetencyCheckParam.CompetencyCheckParamId).ToList();
        int rowParameter = 5;
        foreach (var p in parameters)
        {
          ReportingTools.MergeCells(_wsMain, rowParameter, 1, rowParameter + 1, 2);

          _wsMain.Cells[rowParameter, 1].Style.WrapText = true;
          ReportingTools.PutDataInSheet(_wsMain, rowParameter, 1, p.CompetencyCheckParam.Description, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Top);

          ReportingTools.PutDataInSheet(_wsMain, rowParameter, 3, $"Score: {p.Score}", ExcelHorizontalAlignment.Right, ExcelVerticalAlignment.Center);

          var commentCell = _wsMain.Cells[rowParameter + 1, 3];
          commentCell.Style.WrapText = true;
          commentCell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
          var r1 = commentCell.RichText.Add($"Comment - {p.CompetencyCheckParam.Comment.AsNullIfEmpty() ?? "N/A"} \r\n");
          r1.Size = 8;
          r1.Color = Color.Gray;
          var r2 = commentCell.RichText.Add(p.Comment.AsNullIfEmpty() ?? "N/A");
          r2.Size = 11;
          r2.Color = Color.Black;
          _wsMain.Row(rowParameter + 1).Height = 58;
          rowParameter += 2;
        }
        return rowParameter;
      }

      int ScoreCell(int rowScore)
      {
        _wsMain.Row(rowScore).Height = 47;
        ReportingTools.PutDataInSheet(_wsMain, rowScore, 1, $"Score: {compCheck.TotalScore:p0}", ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center, Color.Black, Color.White);
        _wsMain.Cells[rowScore, 1].Style.Font.Bold = true;

        ReportingTools.MergeCells(_wsMain, rowScore, 2, rowScore, 3);
        var scoreInstructions = "Score instructions: Score each item above using scale of 0-1 \r\n" +
        "N/A = Problem behavior did not ocurr - no opportunity to observe data collection and recommend intervention. \r\n" +
        $"0 = {compCheck.CompetencyCheckType} was unable to articulate/demostrate what was requested or needed prompts to do it. \r\n" +
        $"1 = {compCheck.CompetencyCheckType} was able to articulate/demostrate what was requested independently. \r\n";
        _wsMain.Cells[rowScore, 2].Style.Font.Size = 8;
        _wsMain.Cells[rowScore, 2].Style.WrapText = true;
        ReportingTools.PutDataInSheet(_wsMain, rowScore, 2, scoreInstructions, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Top, fg: Color.Gray);
        return ++rowScore;
      }

      async Task<int> SignCell(int rowSign)
      {
        ReportingTools.MergeCells(_wsMain, rowSign, 1, rowSign, 3);
        rowSign++;
        _wsMain.Row(rowSign).Height = 60;

        var analyst = compCheck.EvaluationBy;
        var secondaryName = compCheck.CompetencyCheckType == CompetencyCheckType.Caregiver ? $"Caregiver {compCheck.Caregiver.CaregiverFullname}" : $"{compCheck.User.Firstname} {compCheck.User.Lastname} - {compCheck.User.LicenseNo}";

        ReportingTools.PutDataInSheet(_wsMain, rowSign, 1, compCheck.Date.ToShortDateString(), ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Bottom);
        ReportingTools.PutDataInSheet(_wsMain, rowSign, 2, secondaryName, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Bottom);
        ReportingTools.PutDataInSheet(_wsMain, rowSign, 3, $"{analyst.Firstname} {analyst.Lastname} - {analyst.LicenseNo}", ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Bottom);

        var analystSign = compCheck.EvaluationBy.UserSign.Sign;

        var caregiverSign = await _dbContext.Sessions
        .Where(w => w.ClientId == compCheck.ClientId && w.SessionStatus == SessionStatus.Billed)
        .OrderByDescending(o => o.SessionStart)
        .Select(s => s.Sign.Sign)
        .FirstOrDefaultAsync();

        // var caregiverSign = await _dbContext.Sessions.Where(w => w.ClientId == compCheck.ClientId)
        //   .Where(s => s.Sign.Sign != null)
        //   .OrderByDescending(o => o.SessionStart)
        //   .Select(s => s.Sign.Sign)
        //   .FirstOrDefaultAsync();
        var userSign = compCheck.User?.UserSign?.Sign;

        var sign = compCheck.CompetencyCheckType == CompetencyCheckType.Caregiver ? caregiverSign : userSign;

        var imgSignAnalyst = LoadImage(analystSign);
        var picture = _wsMain.Drawings.AddPicture("AnalystSign", imgSignAnalyst);
        picture.SetSize(150, 50);
        picture.SetPosition(rowSign - 1, 10, 2, 50);

        var imgSignSecondary = LoadImage(sign);
        var picture2 = _wsMain.Drawings.AddPicture("SecondarySign", imgSignSecondary);
        picture2.SetSize(150, 50);
        picture2.SetPosition(rowSign - 1, 10, 1, 50);


        return rowSign;
      }

      void FinalizeReport(int rowFinalize)
      {
        ReportingTools.DrawBorders(_wsMain, 1, 1, rowFinalize, 3);
      }

      Image LoadImage(string st)
      {
        var base64Data = Regex.Match(st, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        byte[] bytes = Convert.FromBase64String(base64Data);

        Image image;
        using (MemoryStream ms = new MemoryStream(bytes))
        {
          image = Image.FromStream(ms);
        }

        return image;
      }

    }


  }
}