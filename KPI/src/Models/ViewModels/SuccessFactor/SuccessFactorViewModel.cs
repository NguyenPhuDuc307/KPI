namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model for Success Factor information.
    /// </summary>
    public class SuccessFactorViewModel
    {
        /// <summary>
        /// Gets or sets the Success Factor ID.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets or sets the Success Factor name.
        /// </summary>
        [Required]
        [Display(Name = "Name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the Success Factor description.
        /// </summary>
        [Required]
        [Display(Name = "Description")]
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this is a Critical Success Factor.
        /// </summary>
        [Display(Name = "Critical")]
        public bool IsCritical { get; init; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        [Display(Name = "Category")]
        public SuccessFactorCategory Category { get; init; }

        /// <summary>
        /// Gets or sets the progress percentage.
        /// </summary>
        [Display(Name = "Progress")]
        [Range(0, 100)]
        public int ProgressPercentage { get; init; }

        /// <summary>
        /// Gets or sets the status of the indicator.
        /// </summary>
        [Display(Name = "Status")]
        public IndicatorStatus Status { get; init; } = IndicatorStatus.Draft;

        /// <summary>
        /// Gets or sets the objective name.
        /// </summary>
        [Display(Name = "Objective")]
        public string ObjectiveName { get; init; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the SuccessFactorViewModel class.
        /// </summary>
        public SuccessFactorViewModel()
        {
            // Initialize any other necessary properties
        }
    }
}
