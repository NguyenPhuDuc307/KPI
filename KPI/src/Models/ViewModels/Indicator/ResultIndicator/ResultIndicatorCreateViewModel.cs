namespace KPISolution.Models.ViewModels.ResultIndicator
{
    /// <summary>
    /// ViewModel for creating a new Result Indicator
    /// </summary>
    public class ResultIndicatorCreateViewModel
    {
        /// <summary>
        /// Name of the Result Indicator
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Code of the Result Indicator
        /// </summary>
        [Required(ErrorMessage = "Code is required")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Result Indicator
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Flag indicating if this is a Key Result Indicator (KRI)
        /// </summary>
        [Display(Name = "Is Key Result Indicator (KRI)")]
        public bool IsKey { get; set; }

        /// <summary>
        /// Target value of the Result Indicator
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Threshold value of the Result Indicator
        /// </summary>
        [Display(Name = "Threshold Value")]
        public decimal? ThresholdValue { get; set; }

        /// <summary>
        /// Priority level of the Result Indicator
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Related Success Factor ID
        /// </summary>
        [Display(Name = "Success Factor")]
        public Guid? SuccessFactorId { get; set; }

        /// <summary>
        /// Department ID responsible for this Result Indicator
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Display(Name = "Unit")]
        public MeasurementUnit Unit { get; set; } = MeasurementUnit.Number;

        /// <summary>
        /// Frequency of measurement
        /// </summary>
        [Display(Name = "Measurement Frequency")]
        public MeasurementFrequency Frequency { get; set; } = MeasurementFrequency.Monthly;

        /// <summary>
        /// Direction of measurement (whether higher or lower values are better)
        /// </summary>
        [Display(Name = "Direction")]
        public MeasurementDirection Direction { get; set; } = MeasurementDirection.HigherIsBetter;

        /// <summary>
        /// Notes about this Result Indicator
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Owner/responsible person for this Result Indicator
        /// </summary>
        [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// User responsible for this Result Indicator
        /// </summary>
        [Display(Name = "Responsible User")]
        public string? ResponsibleUserId { get; set; }

        /// <summary>
        /// Start date of the Result Indicator
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Target date for achieving this Result Indicator
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddYears(1);
    }
}
