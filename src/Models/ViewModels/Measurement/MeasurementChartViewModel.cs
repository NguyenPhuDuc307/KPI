namespace KPISolution.Models.ViewModels.Measurement
{
    public class MeasurementChartViewModel
    {
        public Guid IndicatorId { get; set; }
        public IndicatorType IndicatorType { get; set; }
        public string IndicatorName { get; set; } = string.Empty;
        public decimal? TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new();
        public List<decimal> ActualValues { get; set; } = new();
        public string ChartTitle => $"Biểu đồ theo dõi {this.IndicatorName}";
        public string YAxisLabel => $"Giá trị ({this.Unit})";
    }
} 