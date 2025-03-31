using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Represents a measurement or value recorded for a KPI at a specific point in time
    /// </summary>
    public class KpiMeasurement : BaseEntity
    {
        /// <summary>
        /// ID of the KPI this measurement belongs to
        /// </summary>
        [Required]
        public Guid KpiId { get; set; }

        /// <summary>
        /// The actual value recorded for the KPI
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Value { get; set; }

        /// <summary>
        /// Date and time when the measurement was taken
        /// </summary>
        [Required]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Status of the measurement (e.g., Recorded, Verified, Rejected)
        /// </summary>
        [Required]
        public MeasurementStatus Status { get; set; }

        /// <summary>
        /// Additional notes or comments about the measurement
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Navigation property to the associated KPI
        /// </summary>
        public virtual KpiBase? Kpi { get; set; }
    }
}