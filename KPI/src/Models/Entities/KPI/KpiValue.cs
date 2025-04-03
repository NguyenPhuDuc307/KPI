using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Represents a measurement value for a KPI
    /// </summary>
    public class KpiValue : BaseEntity
    {
        /// <summary>
        /// ID of the KPI this value belongs to
        /// </summary>
        [Required]
        public Guid KpiId { get; set; }

        /// <summary>
        /// The actual measured value
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Date when the measurement was taken
        /// </summary>
        [Required]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Additional notes about the measurement
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Navigation property to the KPI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual KpiBase? Kpi { get; set; }
    }
}