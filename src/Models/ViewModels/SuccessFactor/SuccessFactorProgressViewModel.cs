namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model for recording progress updates on Success Factors
    /// </summary>
    public class SuccessFactorProgressViewModel
    {
        /// <summary>
        /// ID of the Success Factor being updated
        /// </summary>
        [Required]
        public Guid SuccessFactorId { get; set; }

        /// <summary>
        /// Name of the Success Factor (for display purposes)
        /// </summary>
        [Display(Name = "Success Factor")]
        public string SuccessFactorName { get; set; } = string.Empty;

        /// <summary>
        /// Code of the Success Factor (for display purposes)
        /// </summary>
        [Display(Name = "Code")]
        public string SuccessFactorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date of the progress update
        /// </summary>
        [Required(ErrorMessage = "Update date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Required(ErrorMessage = "Progress percentage is required")]
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        [Display(Name = "Progress %")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Previous progress percentage (for comparison)
        /// </summary>
        [Display(Name = "Previous %")]
        public int? PreviousPercentage { get; set; }

        /// <summary>
        /// Current status of the Success Factor
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public SuccessFactorStatus Status { get; set; } = SuccessFactorStatus.InProgress;

        /// <summary>
        /// Previous status (for comparison)
        /// </summary>
        [Display(Name = "Previous Status")]
        public SuccessFactorStatus? PreviousStatus { get; set; }

        /// <summary>
        /// Risk level associated with this Success Factor
        /// </summary>
        [Required(ErrorMessage = "Risk level is required")]
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;

        /// <summary>
        /// Previous risk level (for comparison)
        /// </summary>
        [Display(Name = "Previous Risk Level")]
        public RiskLevel? PreviousRiskLevel { get; set; }

        /// <summary>
        /// Comments, observations or notes about this progress update
        /// </summary>
        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Comments { get; set; }

        /// <summary>
        /// Issues or obstacles encountered
        /// </summary>
        [StringLength(500, ErrorMessage = "Issues cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Issues")]
        public string? Issues { get; set; }

        /// <summary>
        /// Actions taken or planned to address issues
        /// </summary>
        [StringLength(500, ErrorMessage = "Actions cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Actions")]
        public string? Actions { get; set; }

        /// <summary>
        /// Person who reported this progress update
        /// </summary>
        [StringLength(100, ErrorMessage = "Reported by cannot exceed 100 characters")]
        [Display(Name = "Reported By")]
        public string? ReportedBy { get; set; }

        /// <summary>
        /// Flag indicating if this update requires management attention
        /// </summary>
        [Display(Name = "Needs Management Attention")]
        public bool NeedsAttention { get; set; }

        /// <summary>
        /// Reason why this update needs attention
        /// </summary>
        [StringLength(200, ErrorMessage = "Attention reason cannot exceed 200 characters")]
        [Display(Name = "Attention Reason")]
        public string? AttentionReason { get; set; }

        /// <summary>
        /// Target date for the Success Factor (for reference)
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Flag indicating if the Success Factor is on track to meet its target date
        /// </summary>
        [Display(Name = "On Track")]
        public bool IsOnTrack { get; set; }

        /// <summary>
        /// Next steps planned for this Success Factor
        /// </summary>
        [StringLength(500, ErrorMessage = "Next steps cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Next Steps")]
        public string? NextSteps { get; set; }

        /// <summary>
        /// When the next update is scheduled
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Next Update")]
        public DateTime? NextUpdateDate { get; set; }

        /// <summary>
        /// Key achievements since the last update
        /// </summary>
        [StringLength(500, ErrorMessage = "Achievements cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Achievements")]
        public string? Achievements { get; set; }
    }
}
