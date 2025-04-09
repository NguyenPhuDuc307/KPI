namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public class IndicatorCardWidgetData
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CurrentValue { get; set; }
        public string? TargetValue { get; set; }
        public string? MeasurementUnit { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string? TrendDirection { get; set; }
        public string? TrendValue { get; set; }
        public string? IndicatorType { get; set; }
        public bool IsKeyIndicator { get; set; }
        public string? Department { get; set; }
        public string? ResponsiblePerson { get; set; }
    }
}
