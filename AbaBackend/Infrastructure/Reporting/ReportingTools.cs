using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AbaBackend.Infrastructure.Reporting
{
  public static class ReportingTools
  {

    public static async Task<Client> GetClientById(this IQueryable<Client> clients, int id)
    {
      var client = await clients.Where(s => s.ClientId.Equals(id)).FirstOrDefaultAsync();
      return client;
    }

    public static void PutDataInSheet(ExcelWorksheet ws, int row, int col, object value, ExcelHorizontalAlignment hor, ExcelVerticalAlignment ver, Color? bg = null, Color? fg = null, string format = "@")
    {
      ws.Cells[row, col].Value = value;
      ws.Cells[row, col].Style.HorizontalAlignment = hor;
      ws.Cells[row, col].Style.VerticalAlignment = ver;
      if (bg != null) ws.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
      if (bg != null) ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor((Color)bg);
      if (fg != null) ws.Cells[row, col].Style.Font.Color.SetColor((Color)fg);
      ws.Cells[row, col].Style.Numberformat.Format = format;
    }

    public static void MergeCells(ExcelWorksheet ws, int row1, int col1, int row2, int col2)
    {
      ws.Cells[row1, col1, row2, col2].Merge = true;
    }

    public static void DrawBorders(ExcelWorksheet ws, int row1, int col1, int row2, int col2)
    {
      ws.Cells[row1, col1, row2, col2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
      ws.Cells[row1, col1, row2, col2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
      ws.Cells[row1, col1, row2, col2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
      ws.Cells[row1, col1, row2, col2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }

    public static double GetTrueColumnWidth(double width)
    {
      double z;
      z = width >= (1 + 2 / 3) ? Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2) : Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
      var errorAmt = width - z;
      double adj;
      if (width >= (1 + 2 / 3)) adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
      else adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
      if (z > 0) return width + adj;
      return 0d;
    }

    public static double MeasureTextHeight(string text, ExcelFont font, int width)
    {
      if (string.IsNullOrEmpty(text)) return 0.0;
      var bitmap = new Bitmap(1, 1);
      var graphics = Graphics.FromImage(bitmap);

      var pixelWidth = Convert.ToInt32(width * 7.5);  //7.5 pixels per excel column width
      var drawingFont = new Font(font.Name, font.Size);
      var size = graphics.MeasureString(text, drawingFont, pixelWidth);

      //72 DPI and 96 points per inch.  Excel height in points with max of 409 per Excel requirements.
      return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
    }
  }
}

public static class EnumerationExtensions
{
  public static string AsNullIfEmpty(this string items)
  {
    if (String.IsNullOrEmpty(items))
    {
      return null;
    }
    return items;
  }

  public static string AsNullIfWhiteSpace(this string items)
  {
    if (String.IsNullOrWhiteSpace(items))
    {
      return null;
    }
    return items;
  }

  public static IEnumerable<T> AsNullIfEmpty<T>(this IEnumerable<T> items)
  {
    if (items == null || !items.Any())
    {
      return null;
    }
    return items;
  }
}