using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for creating a KPI - extended version for the PI controller
    /// </summary>
    public class KpiCreateViewModel
    {
        /// <summary>
        /// Gets or sets the KPI type (KRI, RI, PI)
        /// </summary>
        [Required]
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// Gets or sets the name of the KPI
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the KPI
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code of the KPI
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the department ID
        /// </summary>
        [Required]
        [Display(Name = "Department")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the responsible person
        /// </summary>
        [Required]
        [Display(Name = "Responsible Person")]
        public string ResponsiblePerson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity type
        /// </summary>
        [Display(Name = "Activity Type")]
        public ActivityType ActivityType { get; set; }

        /// <summary>
        /// Gets or sets the performance level
        /// </summary>
        [Display(Name = "Performance Level")]
        public string PerformanceLevel { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target value
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Gets or sets the unit
        /// </summary>
        [Required]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the measurement direction
        /// </summary>
        [Required]
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        [Display(Name = "Weight")]
        [Range(0, 100)]
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the effective date
        /// </summary>
        [Required]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the formula
        /// </summary>
        [Display(Name = "Formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// Gets or sets the action plan
        /// </summary>
        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; set; }

        /// <summary>
        /// Gets or sets the selected CSFs
        /// </summary>
        [Display(Name = "Selected CSFs")]
        public List<Guid>? SelectedCsfs { get; set; }
    }
}