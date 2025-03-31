using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.KPI
{
    /// <summary>
    /// Key Result Indicator (KRI) - High level strategic indicators that measure overall company performance
    /// </summary>
    public class KRI : KpiBase
    {
        /// <summary>
        /// Strategic objective this KRI is aligned with
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Strategic Objective")]
        public string StrategicObjective { get; set; } = string.Empty;

        /// <summary>
        /// Impact level of this KRI on the organization
        /// </summary>
        [Required]
        [Display(Name = "Impact Level")]
        public ImpactLevel ImpactLevel { get; set; } = ImpactLevel.Medium;

        /// <summary>
        /// Business area that this KRI is related to (e.g., Financial, Customer, Operations)
        /// </summary>
        [Required]
        [Display(Name = "Business Area")]
        public BusinessArea BusinessArea { get; set; } = BusinessArea.Other;

        /// <summary>
        /// Collection of related Result Indicators (RIs) that contribute to this KRI
        /// </summary>
        public virtual ICollection<RI>? RelatedRIs { get; set; }

        /// <summary>
        /// Board or executive level owner of this KRI
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Executive Owner")]
        public string? ExecutiveOwner { get; set; }

        /// <summary>
        /// Estimate of how accurate this KRI reflects the strategic objective
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Confidence Level (%)")]
        public int? ConfidenceLevel { get; set; }
    }
}
