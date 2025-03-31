using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.CSF;
using KPISolution.Models.ViewModels.Department;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for displaying KPI details with related entities
    /// </summary>
    public class KpiDetailsViewModel
    {
        /// <summary>
        /// Unique identifier for the KPI
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the KPI
        /// </summary>
        [Display(Name = "KPI Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the KPI
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The code or identifier for the KPI
        /// </summary>
        [Display(Name = "KPI Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Type of the KPI
        /// </summary>
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// Type of the KPI as string
        /// </summary>
        [Display(Name = "KPI Type")]
        public string KpiTypeString { get; set; } = string.Empty;

        /// <summary>
        /// Category of the KPI
        /// </summary>
        [Display(Name = "Category")]
        public KpiCategory Category { get; set; }

        /// <summary>
        /// Category of the KPI as string
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryString { get; set; } = string.Empty;

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Direction of the KPI (e.g., Higher is better)
        /// </summary>
        [Display(Name = "Direction")]
        public MeasurementDirection Direction { get; set; }

        /// <summary>
        /// Direction of the KPI as string
        /// </summary>
        [Display(Name = "Direction")]
        public string DirectionString { get; set; } = string.Empty;

        /// <summary>
        /// Frequency of measurement
        /// </summary>
        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; set; }

        /// <summary>
        /// Frequency of measurement as string
        /// </summary>
        [Display(Name = "Measurement Frequency")]
        public string FrequencyString { get; set; } = string.Empty;

        /// <summary>
        /// Formula used to calculate the KPI
        /// </summary>
        [Display(Name = "Calculation Formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// Department ID that owns this KPI
        /// </summary>
        [Display(Name = "Department ID")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Department that owns this KPI
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Department entity
        /// </summary>
        public KPISolution.Models.ViewModels.Department.DepartmentViewModel? Department { get; set; }

        /// <summary>
        /// Owner of the KPI
        /// </summary>
        [Display(Name = "Owner")]
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Data source for the KPI
        /// </summary>
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// Current value of the KPI
        /// </summary>
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Date of the last measurement
        /// </summary>
        [Display(Name = "Last Measured")]
        [DataType(DataType.DateTime)]
        public DateTime? LastMeasuredAt { get; set; }

        /// <summary>
        /// Target value for the KPI
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Minimum acceptable value
        /// </summary>
        [Display(Name = "Minimum Threshold")]
        public decimal? MinThreshold { get; set; }

        /// <summary>
        /// Maximum acceptable value
        /// </summary>
        [Display(Name = "Maximum Threshold")]
        public decimal? MaxThreshold { get; set; }

        /// <summary>
        /// Starting measurement date
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Whether the KPI is displayed on dashboard
        /// </summary>
        [Display(Name = "Display on Dashboard")]
        public bool DisplayOnDashboard { get; set; }

        /// <summary>
        /// Whether alerts are enabled for this KPI
        /// </summary>
        [Display(Name = "Enable Alerts")]
        public bool EnableAlerts { get; set; }

        /// <summary>
        /// Additional notes
        /// </summary>
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Date when the KPI was created
        /// </summary>
        [Display(Name = "Created On")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who created the KPI
        /// </summary>
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Date when the KPI was last updated
        /// </summary>
        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who last updated the KPI
        /// </summary>
        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Current status of the KPI
        /// </summary>
        [Display(Name = "Status")]
        public KpiStatus Status { get; set; }

        /// <summary>
        /// Current status of the KPI as string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusString { get; set; } = string.Empty;

        /// <summary>
        /// CSFs linked to this KPI
        /// </summary>
        [Display(Name = "Critical Success Factors")]
        public List<LinkedCsfViewModel>? LinkedCsfs { get; set; } = new List<LinkedCsfViewModel>();

        /// <summary>
        /// Historical values for this KPI
        /// </summary>
        [Display(Name = "Historical Values")]
        public List<KpiValueViewModel>? HistoricalValues { get; set; } = new List<KpiValueViewModel>();

        /// <summary>
        /// Threshold information
        /// </summary>
        public List<Threshold>? Thresholds { get; set; }

        /// <summary>
        /// Target information
        /// </summary>
        public List<Target>? Targets { get; set; }

        /// <summary>
        /// Current trend indicator (Up, Down, Stable)
        /// </summary>
        [Display(Name = "Trend")]
        public TrendDirection Trend { get; set; }

        /// <summary>
        /// Trend as a string
        /// </summary>
        [Display(Name = "Trend")]
        public string TrendString { get; set; } = string.Empty;

        /// <summary>
        /// Percentage change from previous period
        /// </summary>
        [Display(Name = "Change %")]
        public decimal? PercentageChange { get; set; }

        /// <summary>
        /// Custom properties as dictionary
        /// </summary>
        public Dictionary<string, string>? CustomProperties { get; set; }

        /// <summary>
        /// Performance indicator (calculated based on thresholds and targets)
        /// </summary>
        [Display(Name = "Performance Indicator")]
        public string PerformanceIndicator { get; set; } = string.Empty;

        /// <summary>
        /// Performance indicator as string
        /// </summary>
        [Display(Name = "Performance")]
        public string PerformanceLevelString { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for styling based on performance
        /// </summary>
        public string PerformanceCssClass { get; set; } = string.Empty;

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
        /// Impact level as a string
        /// </summary>
        [Display(Name = "Impact")]
        public string? ImpactLevelDisplay { get; set; }

        /// <summary>
        /// Business area for KRIs
        /// </summary>
        [Display(Name = "Business Area")]
        public BusinessArea? BusinessArea { get; set; }

        /// <summary>
        /// Business area as a string
        /// </summary>
        [Display(Name = "Business Area")]
        public string? BusinessAreaDisplay { get; set; }

        /// <summary>
        /// Executive owner for KRIs
        /// </summary>
        [Display(Name = "Executive Owner")]
        public string? ExecutiveOwner { get; set; }

        /// <summary>
        /// Confidence level for KRIs
        /// </summary>
        [Display(Name = "Confidence Level (%)")]
        public int? ConfidenceLevel { get; set; }

        /// <summary>
        /// Related KPIs
        /// </summary>
        [Display(Name = "Related KPIs")]
        public List<LinkedKpiViewModel> RelatedKpis { get; set; } = new List<LinkedKpiViewModel>();

        // RI specific properties
        /// <summary>
        /// Parent KRI ID (for RI)
        /// </summary>
        [Display(Name = "Parent KRI")]
        public Guid? ParentKriId { get; set; }

        /// <summary>
        /// Parent KPI information (linked KRI for RIs)
        /// </summary>
        [Display(Name = "Parent KPI")]
        public LinkedKpiViewModel? ParentKpi { get; set; }

        /// <summary>
        /// Process area (for RIs)
        /// </summary>
        [Display(Name = "Process Area")]
        public ProcessArea? ProcessArea { get; set; }

        /// <summary>
        /// Process area display name
        /// </summary>
        [Display(Name = "Process Area")]
        public string? ProcessAreaDisplay { get; set; }

        /// <summary>
        /// Scope of measurement (for RIs)
        /// </summary>
        [Display(Name = "Measurement Scope")]
        public string? MeasurementScope { get; set; }

        /// <summary>
        /// Time frame of the result (for RIs)
        /// </summary>
        [Display(Name = "Time Frame")]
        public string? TimeFrame { get; set; }

        /// <summary>
        /// Type of result (for RIs)
        /// </summary>
        [Display(Name = "Result Type")]
        public string? ResultType { get; set; }

        /// <summary>
        /// Contribution percentage to parent KRI (for RIs)
        /// </summary>
        [Display(Name = "Contribution Percentage")]
        public int? ContributionPercentage { get; set; }

        /// <summary>
        /// Method of calculation
        /// </summary>
        [Display(Name = "Calculation Method")]
        public string? CalculationMethod { get; set; }

        /// <summary>
        /// Effective date of the KPI
        /// </summary>
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// Unit of measurement (e.g., %, $, count)
        /// </summary>
        [Display(Name = "Measurement Unit")]
        public string? MeasurementUnit { get; set; }

        /// <summary>
        /// Frequency of measurement
        /// </summary>
        [Display(Name = "Measurement Frequency")]
        public string? MeasurementFrequency { get; set; }

        /// <summary>
        /// Gets or sets the measurement direction
        /// </summary>
        [Display(Name = "Measurement Direction")]
        public string? MeasurementDirection { get; set; } = string.Empty;

        // PI specific properties
        /// <summary>
        /// Activity type for PIs
        /// </summary>
        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Activity type as a string
        /// </summary>
        [Display(Name = "Activity Type")]
        public string? ActivityTypeDisplay { get; set; }

        /// <summary>
        /// Performance level rating for PIs (1-5 scale)
        /// </summary>
        [Display(Name = "Performance Rating")]
        public int? PerformanceRating { get; set; }

        /// <summary>
        /// Indicator type for PIs
        /// </summary>
        [Display(Name = "Indicator Type")]
        public IndicatorType? IndicatorType { get; set; }

        /// <summary>
        /// Indicator type as a string
        /// </summary>
        [Display(Name = "Indicator Type")]
        public string? IndicatorTypeDisplay { get; set; }

        /// <summary>
        /// Action plan for PIs
        /// </summary>
        [Display(Name = "Action Plan")]
        public string? ActionPlan { get; set; }

        /// <summary>
        /// Gets or sets the trend direction
        /// </summary>
        [Display(Name = "Trend Direction")]
        public TrendDirection? TrendDirection { get; set; }

        /// <summary>
        /// Gets or sets the trend value
        /// </summary>
        [Display(Name = "Trend Value")]
        public decimal? TrendValue { get; set; }

        /// <summary>
        /// Gets or sets the last updated date
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the responsible person for the PI
        /// </summary>
        [Display(Name = "Responsible Person")]
        public string ResponsiblePerson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the performance level for the PI
        /// </summary>
        [Display(Name = "Performance Level")]
        public string PerformanceLevel { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the weight percentage
        /// </summary>
        [Display(Name = "Weight")]
        public decimal? Weight { get; set; }
    }
}
