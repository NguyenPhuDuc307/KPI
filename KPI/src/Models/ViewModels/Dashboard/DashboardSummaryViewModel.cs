using System;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// Summary information about a dashboard for listing purposes
    /// </summary>
    public class DashboardSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the dashboard identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Dashboard title
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public required string Title { get; set; } = string.Empty;

        /// <summary>
        /// Dashboard description
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public required string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of items in the dashboard
        /// </summary>
        [Display(Name = "Items")]
        public int ItemCount { get; set; }

        /// <summary>
        /// Gets or sets the date/time the dashboard was created
        /// </summary>
        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date/time the dashboard was last updated
        /// </summary>
        [Display(Name = "Last Updated")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dashboard is the user's default
        /// </summary>
        [Display(Name = "Default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dashboard is shared with others
        /// </summary>
        [Display(Name = "Shared")]
        public bool IsShared { get; set; }
    }
}