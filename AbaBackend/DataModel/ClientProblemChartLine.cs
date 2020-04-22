using System;

namespace AbaBackend.DataModel
{
    public class ClientProblemChartLine
    {
        public int ClientProblemChartLineId { get; set; }
        public int ClientProblemId { get; set; }
        public DateTime ChartDate { get; set; }
        public string ChartLabel { get; set; }
        public string ChartOrientation { get; set; }
        public string ChartLineStyle { get; set; }
        public string ChartLineColor { get; set; }
        public string ChartLabelFontSize { get; set; }
    }
}