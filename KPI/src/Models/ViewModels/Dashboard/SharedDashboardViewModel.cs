namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for dashboard that has been shared with a user
    /// </summary>
    public class SharedDashboardViewModel
    {
        /// <summary>
        /// Gets or sets the dashboard identifier
        /// </summary>
        public Guid DashboardId { get; set; }

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
        /// ID of the dashboard owner
        /// </summary>
        [Required]
        [Display(Name = "Owner ID")]
        public required string OwnerId { get; set; } = string.Empty;

        /// <summary>
        /// Username of the dashboard owner
        /// </summary>
        [Display(Name = "Owner")]
        public required string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// Permission level for the shared dashboard
        /// </summary>
        [Display(Name = "Permission Level")]
        public required string Permission { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date/time the dashboard was shared
        /// </summary>
        [Display(Name = "Shared On")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime SharedAt { get; set; }

        /// <summary>
        /// Gets or sets the date/time the dashboard was last updated
        /// </summary>
        [Display(Name = "Last Updated")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime LastUpdated { get; set; }
    }
}
