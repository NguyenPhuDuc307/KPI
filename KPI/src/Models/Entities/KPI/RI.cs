using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Result Indicator (RI) - Measures what has been done or achieved in a specific area
    /// </summary>
    public class RI : KpiBase
    {
        /// <summary>
        /// Reference to the parent KRI
        /// </summary>
        [Display(Name = "Parent KRI")]
        public Guid? ParentKriId { get; set; }

        /// <summary>
        /// Navigation property to the parent KRI
        /// </summary>
        [ForeignKey("ParentKriId")]
        public virtual KRI? ParentKRI { get; set; }

        /// <summary>
        /// Process area this Result Indicator belongs to
        /// </summary>
        [Display(Name = "Process Area")]
        public ProcessArea ProcessArea { get; set; }

        /// <summary>
        /// Collection of related Performance Indicators (PIs) that contribute to this RI
        /// </summary>
        public virtual ICollection<PI>? RelatedPIs { get; set; }

        /// <summary>
        /// Scope or boundary of what this indicator is measuring
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Measurement Scope")]
        public string? MeasurementScope { get; set; }

        /// <summary>
        /// The timeframe for which this result is relevant
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Time Frame")]
        public string? TimeFrame { get; set; }

        /// <summary>
        /// Primary source of data for this indicator
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Data Source")]
        public string? DataSource { get; set; }

        /// <summary>
        /// The contribution percentage this RI has toward its parent KRI
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Contribution to KRI (%)")]
        public int? ContributionPercentage { get; set; }

        /// <summary>
        /// Type of result (e.g., Financial, Customer Satisfaction, Process Efficiency)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Result Type")]
        public string? ResultType { get; set; }

        /// <summary>
        /// Department level manager responsible for this RI
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Manager")]
        public string? ResponsibleManager { get; set; }
    }
}
