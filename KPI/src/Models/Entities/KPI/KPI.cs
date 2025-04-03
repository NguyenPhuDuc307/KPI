using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Key Performance Indicator (KPI) - Chỉ số hiệu suất then chốt
    /// </summary>
    public class KPI : KpiBase
    {
        /// <summary>
        /// Domain or area of performance this KPI focuses on (Productivity, Quality, Cost, etc.)
        /// </summary>
        [Required]
        [Display(Name = "Performance Domain")]
        public PerformanceDomain PerformanceDomain { get; set; }

        /// <summary>
        /// The organizational level this KPI applies to (Strategic, Tactical, Operational)
        /// </summary>
        [Required]
        [Display(Name = "Category Level")]
        public CategoryLevel CategoryLevel { get; set; }

        /// <summary>
        /// Specific area or aspect of performance being measured
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Focus Area")]
        public string? FocusArea { get; set; }

        /// <summary>
        /// Target for improvement (e.g., "Increase by 10% annually")
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Improvement Target")]
        public string? ImprovementTarget { get; set; }

        /// <summary>
        /// Reference value for benchmarking against industry or past performance
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Benchmark Value")]
        public decimal? BenchmarkValue { get; set; }

        /// <summary>
        /// ID of the department that owns this KPI
        /// </summary>
        [Display(Name = "Owner Department")]
        public Guid? OwnerDepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the department that owns this KPI
        /// </summary>
        [ForeignKey("OwnerDepartmentId")]
        public virtual Organization.Department? OwnerDepartment { get; set; }
    }

    /// <summary>
    /// Domain or area of performance measurement
    /// </summary>
    public enum PerformanceDomain
    {
        [Display(Name = "Productivity")]
        Productivity = 1,

        [Display(Name = "Quality")]
        Quality = 2,

        [Display(Name = "Time")]
        Time = 3,

        [Display(Name = "Cost")]
        Cost = 4,

        [Display(Name = "Customer")]
        Customer = 5,

        [Display(Name = "Innovation")]
        Innovation = 6,

        [Display(Name = "Growth")]
        Growth = 7,

        [Display(Name = "Compliance")]
        Compliance = 8,

        [Display(Name = "Risk")]
        Risk = 9,

        [Display(Name = "Other")]
        Other = 99
    }

    /// <summary>
    /// Organizational level the KPI applies to
    /// </summary>
    public enum CategoryLevel
    {
        [Display(Name = "Strategic")]
        Strategic = 1,

        [Display(Name = "Tactical")]
        Tactical = 2,

        [Display(Name = "Operational")]
        Operational = 3
    }
}