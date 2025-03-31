using System;
using System.Collections.Generic;

namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị tiến độ của Critical Success Factor
    /// </summary>
    public class CsfProgressWidgetData : WidgetData
    {
        /// <summary>
        /// CSF unique identifier
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// CSF code
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// CSF name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// CSF description
        /// </summary>
        public new string Description { get; set; } = string.Empty;

        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// CSF status (Completed, On Track, At Risk, Delayed, Not Started)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Owner/person responsible for the CSF
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Target completion date
        /// </summary>
        public DateTime? TargetDate { get; set; }

        /// <summary>
        /// Recent updates/history of the CSF
        /// </summary>
        public List<CsfUpdate> RecentUpdates { get; set; } = new List<CsfUpdate>();

        /// <summary>
        /// Whether to show the header section
        /// </summary>
        public bool ShowHeader { get; set; } = true;

        /// <summary>
        /// Whether to show the details section
        /// </summary>
        public bool ShowDetails { get; set; } = true;

        /// <summary>
        /// Whether to show action buttons
        /// </summary>
        public bool ShowActions { get; set; } = true;
    }

    public class CsfUpdate
    {
        /// <summary>
        /// Update title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Update description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Date of the update
        /// </summary>
        public DateTime Date { get; set; }
    }
}