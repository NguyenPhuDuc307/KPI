using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.CSF
{
    /// <summary>
    /// Represents the relationship between a CSF and a KPI
    /// </summary>
    public class CSFKPI : BaseEntity
    {
        /// <summary>
        /// The Critical Success Factor ID
        /// </summary>
        [Required]
        [Display(Name = "CSF")]
        public Guid CsfId { get; set; }

        /// <summary>
        /// Navigation property to the related Critical Success Factor
        /// </summary>
        [ForeignKey("CsfId")]
        public virtual CriticalSuccessFactor? CSF { get; set; }

        /// <summary>
        /// The KPI ID
        /// </summary>
        [Required]
        [Display(Name = "KPI")]
        public Guid KpiId { get; set; }

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        [Required]
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// The strength of relationship between this CSF and KPI
        /// </summary>
        [Required]
        [Display(Name = "Relationship Strength")]
        public RelationshipStrength RelationshipStrength { get; set; } = RelationshipStrength.Strong;

        /// <summary>
        /// Description of how this KPI contributes to the CSF
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Contribution Description")]
        public string? ContributionDescription { get; set; }

        /// <summary>
        /// The weight or importance of this KPI to the CSF
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Weight (%)")]
        public int Weight { get; set; } = 0;

        /// <summary>
        /// The impact level this KPI has on the CSF
        /// </summary>
        [Display(Name = "Impact Level")]
        public ImpactLevel ImpactLevel { get; set; } = ImpactLevel.High;

        /// <summary>
        /// Navigation property to the related KPI based on KpiType
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual KpiBase? KPI { get; set; }
    }
}
