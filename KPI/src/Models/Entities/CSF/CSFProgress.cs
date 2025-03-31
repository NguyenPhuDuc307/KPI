using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.CSF
{
    /// <summary>
    /// Represents a progress update for a Critical Success Factor
    /// </summary>
    public class CSFProgress : BaseEntity
    {
        /// <summary>
        /// The Critical Success Factor ID this progress update is for
        /// </summary>
        [Required]
        [Display(Name = "CSF")]
        public Guid CSFId { get; set; }

        /// <summary>
        /// Navigation property to the Critical Success Factor
        /// </summary>
        [ForeignKey("CSFId")]
        public virtual CriticalSuccessFactor? CSF { get; set; }

        /// <summary>
        /// Date of the progress update
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The progress percentage at this update point
        /// </summary>
        [Required]
        [Range(0, 100)]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Previous progress percentage (before this update)
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Previous Progress (%)")]
        public int? PreviousProgressPercentage { get; set; }

        /// <summary>
        /// Status of the CSF at this update point
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; } = CSFStatus.InProgress;

        /// <summary>
        /// Previous status (before this update)
        /// </summary>
        [Display(Name = "Previous Status")]
        public CSFStatus? PreviousStatus { get; set; }

        /// <summary>
        /// Commentary on what has been achieved
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Achievements")]
        public string? Achievements { get; set; }

        /// <summary>
        /// Commentary on challenges or blockers
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Challenges")]
        public string? Challenges { get; set; }

        /// <summary>
        /// Next steps planned
        /// </summary>
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Next Steps")]
        public string? NextSteps { get; set; }

        /// <summary>
        /// Person who provided this update
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Updated By")]
        public new string? UpdatedBy { get; set; }

        /// <summary>
        /// Risk level associated with the CSF at this update point
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

        /// <summary>
        /// Previous risk level (before this update)
        /// </summary>
        [Display(Name = "Previous Risk Level")]
        public RiskLevel? PreviousRiskLevel { get; set; }

        /// <summary>
        /// Flag indicating if this update requires management attention
        /// </summary>
        [Display(Name = "Needs Attention")]
        public bool NeedsAttention { get; set; } = false;

        /// <summary>
        /// Expected completion date based on current progress
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Expected Completion Date")]
        public DateTime? ExpectedCompletionDate { get; set; }
    }
}
