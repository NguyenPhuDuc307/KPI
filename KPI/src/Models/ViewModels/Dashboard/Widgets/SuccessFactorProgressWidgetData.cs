namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public class SuccessFactorProgressWidgetData
    {
        public SuccessFactorProgressWidgetData()
        {
            this.RecentUpdates = [];
        }

        public Guid Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int ProgressPercentage { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? Owner { get; init; }
        public DateTime? TargetDate { get; init; }
        public bool IsCritical { get; init; }
        public List<SuccessFactorUpdateItem> RecentUpdates { get; init; }

        // Display options
        public bool ShowHeader { get; init; } = true;
        public bool ShowDetails { get; init; } = true;
        public bool ShowActions { get; init; } = true;
    }

    public class SuccessFactorUpdateItem
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Author { get; set; }
    }
}
