namespace KPISolution.Models.Entities.Dashboard
{
    /// <summary>
    /// Entity representing a custom dashboard created by a user
    /// </summary>
    public class CustomDashboard : BaseEntity
    {
        /// <summary>
        /// Title of the dashboard
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// User ID who owns this dashboard
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Username who owns this dashboard
        /// </summary>
        [Required]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Date/time when the dashboard was last refreshed
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date/time when the dashboard configuration was last modified
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Layout configuration (JSON)
        /// </summary>
        public string LayoutConfiguration { get; set; } = string.Empty;

        /// <summary>
        /// Whether this dashboard is the user's default
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Whether this dashboard is shared with others
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Refresh interval in minutes
        /// </summary>
        [Range(0, 1440)]
        public int RefreshInterval { get; set; } = 30;

        /// <summary>
        /// Items shown on this dashboard
        /// </summary>
        public virtual ICollection<DashboardItem> DashboardItems { get; set; } = new List<DashboardItem>();
    }
}