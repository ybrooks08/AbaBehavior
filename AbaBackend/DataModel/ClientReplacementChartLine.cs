using System;

namespace AbaBackend.DataModel
{
  public class ClientReplacementChartLine
  {
    public int ClientReplacementChartLineId { get; set; }
    public int ClientReplacementId { get; set; }
    public DateTime ChartDate { get; set; }
    public string ChartLabel { get; set; }
    public string ChartOrientation { get; set; }
    public string ChartLineStyle { get; set; }
    public string ChartLineColor { get; set; }
    public string ChartLabelFontSize { get; set; }
  }
}