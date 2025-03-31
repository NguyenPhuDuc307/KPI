using System;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for KPI impact on CSFs
    /// </summary>
    public class KpiImpactViewModel
    {
        /// <summary>
        /// Gets or sets the KPI ID
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// Gets or sets the CSF ID
        /// </summary>
        public Guid CsfId { get; set; }

        /// <summary>
        /// Gets or sets the KPI name
        /// </summary>
        [Display(Name = "KPI Name")]
        public required string KpiName { get; set; }

        /// <summary>
        /// Gets or sets the CSF name
        /// </summary>
        [Display(Name = "CSF Name")]
        public required string CsfName { get; set; }

        /// <summary>
        /// Gets or sets the impact level
        /// </summary>
        [Display(Name = "Impact Level")]
        [Range(1, 10)]
        public int ImpactLevel { get; set; } = 5;

        /// <summary>
        /// Gets or sets the impact description
        /// </summary>
        [Display(Name = "Impact Description")]
        public required string ImpactDescription { get; set; }

        /// <summary>
        /// Gets or sets the impact type
        /// </summary>
        [Display(Name = "Impact Type")]
        public string ImpactType { get; set; } = "Positive";

        /// <summary>
        /// Gets or sets the relationship creation date
        /// </summary>
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the impact start date
        /// </summary>
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the impact end date (if applicable)
        /// </summary>
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this impact is primary/critical
        /// </summary>
        [Display(Name = "Is Critical")]
        public bool IsCritical { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the KpiImpactViewModel class
        /// </summary>
        public KpiImpactViewModel()
        {
        }
    }
}