using System;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for tracking Critical Success Factor (CSF) progress.
    /// </summary>
    public class CsfProgressViewModel
    {
        /// <summary>
        /// Gets or sets the CSF ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the CSF name.
        /// </summary>
        [Display(Name = "CSF Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current progress percentage.
        /// </summary>
        [Display(Name = "Progress")]
        [Range(0, 100)]
        public decimal Progress { get; set; }

        /// <summary>
        /// Gets or sets the target progress percentage.
        /// </summary>
        [Display(Name = "Target Progress")]
        [Range(0, 100)]
        public decimal TargetProgress { get; set; }

        /// <summary>
        /// Gets or sets the progress status.
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the trend indicator.
        /// </summary>
        [Display(Name = "Trend")]
        public string Trend { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the next review date.
        /// </summary>
        [Display(Name = "Next Review")]
        public DateTime NextReview { get; set; }

        /// <summary>
        /// Gets or sets any blockers or issues.
        /// </summary>
        [Display(Name = "Blockers")]
        public string Blockers { get; set; }

        /// <summary>
        /// Gets or sets the mitigation actions.
        /// </summary>
        [Display(Name = "Mitigation Actions")]
        public string MitigationActions { get; set; }

        /// <summary>
        /// Gets or sets whether the CSF is at risk.
        /// </summary>
        [Display(Name = "At Risk")]
        public bool IsAtRisk { get; set; }

        /// <summary>
        /// Gets or sets whether the CSF is behind schedule.
        /// </summary>
        [Display(Name = "Behind Schedule")]
        public bool IsBehindSchedule { get; set; }

        /// <summary>
        /// Initializes a new instance of the CsfProgressViewModel class.
        /// </summary>
        public CsfProgressViewModel()
        {
            Name = string.Empty;
            Blockers = string.Empty;
            MitigationActions = string.Empty;
            LastUpdated = DateTime.UtcNow;
            NextReview = DateTime.UtcNow.AddDays(7);
            Status = "On Track";
            Trend = "Stable";
            IsAtRisk = false;
            IsBehindSchedule = false;
        }
    }
}