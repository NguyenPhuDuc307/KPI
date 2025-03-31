using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for Critical Success Factor (CSF) information.
    /// </summary>
    public class CsfViewModel
    {
        /// <summary>
        /// Gets or sets the CSF ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the CSF code.
        /// </summary>
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is required")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CSF name.
        /// </summary>
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the CSF description.
        /// </summary>
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the CSF category.
        /// </summary>
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required")]
        public required string Category { get; set; }

        /// <summary>
        /// Gets or sets the CSF status.
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; } = CSFStatus.NotStarted;

        /// <summary>
        /// Gets or sets the CSF priority.
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Gets or sets the CSF creation date.
        /// </summary>
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the CSF last modified date.
        /// </summary>
        [Display(Name = "Last Modified")]
        public DateTime LastModifiedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the progress percentage.
        /// </summary>
        [Display(Name = "Progress")]
        [Range(0, 100)]
        public decimal Progress { get; set; } = 0;

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        [Display(Name = "Department")]
        public string? Department { get; set; }

        /// <summary>
        /// Gets or sets the linked KPIs count.
        /// </summary>
        [Display(Name = "Linked KPIs")]
        public int LinkedKpiCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the target completion date.
        /// </summary>
        [Display(Name = "Target Date")]
        public DateTime? TargetDate { get; set; }

        /// <summary>
        /// Gets or sets the actual completion date.
        /// </summary>
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Gets or sets the list of stakeholders.
        /// </summary>
        [Display(Name = "Stakeholders")]
        public List<string> Stakeholders { get; set; }

        /// <summary>
        /// Gets or sets the risk level.
        /// </summary>
        [Display(Name = "Risk Level")]
        public string RiskLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of the CsfViewModel class.
        /// </summary>
        public CsfViewModel()
        {
            Stakeholders = new List<string>();
            RiskLevel = "Low";
        }
    }
}