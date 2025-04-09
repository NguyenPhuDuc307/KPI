namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public class IndicatorTableWidgetData
    {
        public IndicatorTableWidgetData()
        {
            this.IndicatorItems = [];
        }

        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public bool ShowTrend { get; set; } = true;
        public List<IndicatorTableItemData> IndicatorItems { get; set; }

        // Paging properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)this.TotalItems / this.PageSize);
    }

    public class IndicatorTableItemData
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CurrentValue { get; set; }
        public string? TargetValue { get; set; }
        public string? MeasurementUnit { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TrendDirection { get; set; }
        public string? TrendValue { get; set; }
    }
}
