using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for creating a KPI
    /// </summary>
    public class CreateKpiViewModel
    {
        /// <summary>
        /// KPI type (KRI, RI, PI)
        /// </summary>
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// KPI name
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// KPI description
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// KPI code
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Required]
        [StringLength(30)]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Formula used to calculate the KPI
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// Target value
        /// </summary>
        [Required]
        [Display(Name = "Target Value")]
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Minimum acceptable value
        /// </summary>
        [Required]
        [Display(Name = "Minimum Value")]
        public decimal MinimumValue { get; set; }

        /// <summary>
        /// Maximum possible value
        /// </summary>
        [Required]
        [Display(Name = "Maximum Value")]
        public decimal MaximumValue { get; set; }

        /// <summary>
        /// KPI weight
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Weight (%)")]
        public int Weight { get; set; }

        /// <summary>
        /// Measurement frequency
        /// </summary>
        [Required]
        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency MeasurementFrequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Department responsible
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Department")]
        public string? Department { get; set; }

        /// <summary>
        /// Department ID for the KPI
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Person responsible
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Responsible Person")]
        public string? Owner { get; set; }

        /// <summary>
        /// Date effective
        /// </summary>
        [Required]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// KPI status
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; } = KpiStatus.Draft;

        /// <summary>
        /// KPI category
        /// </summary>
        [Required]
        [Display(Name = "Category")]
        public KpiCategory Category { get; set; }

        /// <summary>
        /// Measurement direction
        /// </summary>
        [Required]
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; set; }

        /// <summary>
        /// Selected CSF IDs
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<Guid>? SelectedCsfIds { get; set; }

        /// <summary>
        /// List of departments for dropdown
        /// </summary>
        public SelectList? Departments { get; set; }

        /// <summary>
        /// List of CSFs for dropdown
        /// </summary>
        public SelectList? CriticalSuccessFactors { get; set; }

        // KRI specific properties
        /// <summary>
        /// Strategic objective for KRIs
        /// </summary>
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Impact level for KRIs
        /// </summary>
        [Display(Name = "Impact Level")]
        public ImpactLevel? ImpactLevel { get; set; }

        /// <summary>
        /// Business area for KRIs
        /// </summary>
        [Display(Name = "Business Area")]
        public BusinessArea? BusinessArea { get; set; }

        /// <summary>
        /// Executive owner for KRIs
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Executive Owner")]
        public string? ExecutiveOwner { get; set; }

        /// <summary>
        /// Confidence level for KRIs
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Confidence Level (%)")]
        public int? ConfidenceLevel { get; set; }

        /// <summary>
        /// List of business areas for dropdown
        /// </summary>
        public SelectList? BusinessAreas { get; set; }

        /// <summary>
        /// List of impact levels for dropdown
        /// </summary>
        public SelectList? ImpactLevels { get; set; }

        // RI specific properties
        /// <summary>
        /// Parent KRI ID (for RIs)
        /// </summary>
        [Display(Name = "Parent KRI")]
        public Guid? ParentKriId { get; set; }

        /// <summary>
        /// Process area (for RIs)
        /// </summary>
        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; set; }

        /// <summary>
        /// Department responsible person's manager (for RIs)
        /// </summary>
        [Display(Name = "Responsible Manager")]
        public string? ResponsibleManager { get; set; }

        /// <summary>
        /// Scope of measurement (for RIs)
        /// </summary>
        [Display(Name = "Measurement Scope")]
        public string? MeasurementScope { get; set; }

        /// <summary>
        /// Time frame for the RI
        /// </summary>
        [Display(Name = "Time Frame")]
        public string? TimeFrame { get; set; }

        /// <summary>
        /// Type of result produced
        /// </summary>
        [Display(Name = "Result Type")]
        public string? ResultType { get; set; }

        /// <summary>
        /// Percentage contribution to parent KRI
        /// </summary>
        [Display(Name = "Contribution Percentage")]
        [Range(0, 100)]
        public int? ContributionPercentage { get; set; }

        /// <summary>
        /// Method of calculation
        /// </summary>
        [Display(Name = "Calculation Method")]
        public string? CalculationMethod { get; set; }

        /// <summary>
        /// Source of data
        /// </summary>
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// Selection of process areas for the dropdown
        /// </summary>
        public SelectList? ProcessAreas { get; set; }

        /// <summary>
        /// Selection of parent KRIs for the dropdown
        /// </summary>
        public SelectList? ParentKris { get; set; }

        // PI specific properties
        /// <summary>
        /// Activity type for PIs
        /// </summary>
        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Performance level for PIs
        /// </summary>
        [Range(1, 5)]
        [Display(Name = "Performance Level")]
        public int PerformanceLevel { get; set; } = 3;

        /// <summary>
        /// Indicator type for PIs
        /// </summary>
        [Display(Name = "Indicator Type")]
        public IndicatorType IndicatorType { get; set; }

        /// <summary>
        /// Action plan for PIs
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; set; }

        /// <summary>
        /// Parent RI ID for PIs
        /// </summary>
        [Display(Name = "Parent RI")]
        public Guid? ParentRiId { get; set; }

        /// <summary>
        /// List of activity types for dropdown
        /// </summary>
        public SelectList? ActivityTypes { get; set; }

        /// <summary>
        /// List of RIs for dropdown
        /// </summary>
        public SelectList? ParentRis { get; set; }

        /// <summary>
        /// List of indicator types for dropdown
        /// </summary>
        public SelectList? IndicatorTypes { get; set; }

        /// <summary>
        /// Measurement unit (e.g., %, count, time)
        /// </summary>
        [Display(Name = "Measurement Unit")]
        [Required(ErrorMessage = "Measurement unit is required")]
        public string? MeasurementUnit { get; set; }

        /// <summary>
        /// Constructor to set default values
        /// </summary>
        public CreateKpiViewModel()
        {
            // Explicitly use the enum values with their fully qualified names
            MeasurementDirection = KPISolution.Models.Enums.MeasurementDirection.HigherIsBetter;
            ImpactLevel = KPISolution.Models.Enums.ImpactLevel.Medium;
            BusinessArea = KPISolution.Models.Enums.BusinessArea.Financial;
            ProcessArea = KPISolution.Models.Enums.ProcessArea.CoreBusiness;
            ActivityType = KPISolution.Models.Enums.ActivityType.StandardOperations;
            IndicatorType = KPISolution.Models.Enums.IndicatorType.Leading;
        }
    }
}
