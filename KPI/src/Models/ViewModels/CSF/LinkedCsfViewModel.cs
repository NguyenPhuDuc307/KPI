using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for CSFs linked to a KPI
    /// </summary>
    public class LinkedCsfViewModel
    {
        /// <summary>
        /// Gets or sets the CSF ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the CSF name
        /// </summary>
        [Display(Name = "CSF Name")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the CSF description
        /// </summary>
        [Display(Name = "Description")]
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the CSF category
        /// </summary>
        [Display(Name = "Category")]
        public required string Category { get; set; }

        /// <summary>
        /// Gets or sets the CSF status
        /// </summary>
        [Display(Name = "Status")]
        public required string Status { get; set; }

        /// <summary>
        /// Gets or sets the department
        /// </summary>
        [Display(Name = "Department")]
        public string? Department { get; set; }

        /// <summary>
        /// Gets or sets the progress percentage
        /// </summary>
        [Display(Name = "Progress")]
        [Range(0, 100)]
        public decimal Progress { get; set; }

        /// <summary>
        /// Gets or sets the relationship type between CSF and KPI
        /// </summary>
        [Display(Name = "Relationship Type")]
        public string RelationshipType { get; set; } = "Contributes";

        /// <summary>
        /// Gets or sets the relationship strength
        /// </summary>
        [Display(Name = "Relationship Strength")]
        public string RelationshipStrength { get; set; } = "Moderate";

        /// <summary>
        /// Gets or sets the impact score
        /// </summary>
        [Display(Name = "Impact Score")]
        [Range(1, 10)]
        public int ImpactScore { get; set; } = 5;

        /// <summary>
        /// Gets or sets the link creation date
        /// </summary>
        [Display(Name = "Linked Date")]
        public DateTime LinkedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets whether this is a primary CSF for the KPI
        /// </summary>
        [Display(Name = "Primary CSF")]
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the LinkedCsfViewModel class
        /// </summary>
        public LinkedCsfViewModel()
        {
        }
    }
}