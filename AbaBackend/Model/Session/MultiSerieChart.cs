using System.Collections.Generic;
using AbaBackend.DataModel;

namespace AbaBackend.Model.Session
{
  public class MultiSerieChart
  {
    public string Name { get; set; }
    public List<int?> Data { get; set; }
  }

  public class Label
  {
    public string Text { get; set; }
  }

  public class PlotLine
  {
    public Label Label { get; set; }
    public string Color { get; set; } = "red";
    public string DashStyle { get; set; } = "ShortDash";
    public int Value { get; set; }
    public int Width { get; set; } = 1;
  }

  public class PlotBand
  {
    public Label Label { get; set; }
    public string Color { get; set; } = "#DAFFCE";
    public int From { get; set; }
    public int To { get; set; } = 1;
  }

  public class ChartNoteData
  {
    public List<PlotLine> PlotLines { get; set; }
    public List<ClientChartNote> ClientChartNotes { get; set; }
  }
}