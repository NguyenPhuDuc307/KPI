namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public class IndicatorCardWidgetData
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? CurrentValue { get; init; }
        public string? TargetValue { get; init; }
        public string? MeasurementUnit { get; init; }
        public int ProgressPercentage { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime LastUpdated { get; init; }
        public string? TrendDirection { get; init; }
        public string? TrendValue { get; init; }
        public string? IndicatorType { get; init; }
        public bool IsKeyIndicator { get; init; }
        public string? Department { get; init; }
        public string? ResponsiblePerson { get; init; }
    }
}
