using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;
using System.Collections.Generic;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for displaying KPI information.
    /// </summary>
    public class KpiViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the KPI.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the KPI.
        /// </summary>
        [Required]
        [Display(Name = "KPI Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code of the KPI.
        /// </summary>
        [Required]
        [Display(Name = "KPI Code")]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the KPI.
        /// </summary>
        [Display(Name = "Description")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the KPI (KRI, RI, PI).
        /// </summary>
        [Required]
        [Display(Name = "KPI Type")]
        public KpiType Type { get; set; }

        /// <summary>
        /// Gets or sets the KPI type
        /// </summary>
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// Gets or sets the department ID this KPI belongs to.
        /// </summary>
        [Required]
        [Display(Name = "Department")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target value for this KPI.
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Gets or sets the actual value for this KPI.
        /// </summary>
        [Display(Name = "Actual Value")]
        public decimal? ActualValue { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement.
        /// </summary>
        [Display(Name = "Unit")]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the frequency of measurement.
        /// </summary>
        [Required]
        [Display(Name = "Frequency")]
        public MeasurementFrequency Frequency { get; set; }

        /// <summary>
        /// Gets or sets the data source for this KPI.
        /// </summary>
        [Display(Name = "Data Source")]
        [StringLength(100)]
        public string DataSource { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the KPI.
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the achievement percentage.
        /// </summary>
        [Display(Name = "Achievement")]
        public decimal? AchievementPercentage { get; set; }

        /// <summary>
        /// Gets or sets the trend direction.
        /// </summary>
        [Display(Name = "Trend")]
        public string Trend { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last updated date.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the owner/responsible person.
        /// </summary>
        [Required]
        [Display(Name = "Owner")]
        [StringLength(100)]
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CSS class for status indication.
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the KPI
        /// </summary>
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the responsible person for the PI
        /// </summary>
        [Display(Name = "Responsible Person")]
        public string ResponsiblePerson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity type for the PI
        /// </summary>
        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Indicates whether this is a Key Result Indicator (KRI). If true, this RI
        /// is considered a KRI for the organization.
        /// </summary>
        [Display(Name = "Is Key Result Indicator (KRI)")]
        public bool IsRIKey { get; set; } = false;

        /// <summary>
        /// Indicates whether this is a Key Performance Indicator (KPI). If true, this PI
        /// is considered a KPI for the organization.
        /// </summary>
        [Display(Name = "Is Key Performance Indicator (KPI)")]
        public bool IsPIKey { get; set; } = false;

        /// <summary>
        /// Gets or sets the performance level for the PI
        /// </summary>
        [Display(Name = "Performance Level")]
        public string PerformanceLevel { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the measurement direction
        /// </summary>
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; set; }

        /// <summary>
        /// Gets or sets the weight percentage
        /// </summary>
        [Display(Name = "Weight")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the formula for calculating the KPI
        /// </summary>
        [Display(Name = "Formula")]
        public string Formula { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the action plan
        /// </summary>
        [Display(Name = "Action Plan")]
        public string ActionPlan { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the effective date
        /// </summary>
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the selected CSF IDs
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<Guid>? SelectedCsfIds { get; set; }

        /// <summary>
        /// Parent Result Indicator ID for Performance Indicators
        /// </summary>
        [Display(Name = "Parent Result Indicator")]
        public Guid? RIId { get; set; }

        /// <summary>
        /// Parent Key Result Indicator ID for direct Performance Indicators
        /// </summary>
        [Display(Name = "Parent Key Result Indicator")]
        public Guid? KRIId { get; set; }
    }
}