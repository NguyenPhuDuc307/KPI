namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    public class SuccessFactorProgressWidgetData
    {
        public SuccessFactorProgressWidgetData()
        {
            this.RecentUpdates = [];
        }

        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Owner { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsCritical { get; set; }
        public List<SuccessFactorUpdateItem> RecentUpdates { get; set; }

        // Display options
        public bool ShowHeader { get; set; } = true;
        public bool ShowDetails { get; set; } = true;
        public bool ShowActions { get; set; } = true;
    }

    public class SuccessFactorUpdateItem
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Author { get; set; }
    }
}
