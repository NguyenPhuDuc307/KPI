using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for displaying CSFs linked to a KPI
    /// </summary>
    public class LinkedCsfViewModel
    {
        /// <summary>
        /// CSF ID
        /// </summary>
        public Guid CsfId { get; set; }

        /// <summary>
        /// CSF ID (alias for CsfId to maintain compatibility with views)
        /// </summary>
        public Guid Id => CsfId;

        /// <summary>
        /// CSF Name
        /// </summary>
        [Display(Name = "CSF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// CSF Code
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Category of the CSF
        /// </summary>
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; }

        /// <summary>
        /// Category as a display-friendly string
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Progress (alias for ProgressPercentage to maintain compatibility with views)
        /// </summary>
        [Display(Name = "Progress (%)")]
        public int Progress => ProgressPercentage;

        /// <summary>
        /// CSS class for progress styling based on percentage
        /// </summary>
        public string ProgressCssClass { get; set; } = string.Empty;

        /// <summary>
        /// The relationship strength between the CSF and this KPI
        /// </summary>
        [Display(Name = "Relationship Strength")]
        public RelationshipStrength RelationshipStrength { get; set; }

        /// <summary>
        /// Relationship strength as a display-friendly string
        /// </summary>
        [Display(Name = "Relationship")]
        public string RelationshipStrengthDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Description of how this CSF is impacted by the KPI
        /// </summary>
        [Display(Name = "Contribution")]
        public string? ContributionDescription { get; set; }

        /// <summary>
        /// Status of the CSF
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; }

        /// <summary>
        /// Status as a display-friendly string
        /// </summary>
        [Display(Name = "Status")]
        public string StatusDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for status styling
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Target date for achieving this CSF
        /// </summary>
        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Days remaining until the target date
        /// </summary>
        [Display(Name = "Days Remaining")]
        public int? DaysRemaining { get; set; }

        /// <summary>
        /// The weight of this KPI's contribution to the CSF
        /// </summary>
        [Display(Name = "Weight (%)")]
        public int Weight { get; set; }

        /// <summary>
        /// The impact level this KPI has on the CSF
        /// </summary>
        [Display(Name = "Impact")]
        public ImpactLevel ImpactLevel { get; set; }

        /// <summary>
        /// Impact level as a display-friendly string
        /// </summary>
        [Display(Name = "Impact")]
        public string ImpactLevelDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Department responsible for this CSF
        /// </summary>
        [Display(Name = "Department")]
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Department (alias for DepartmentName to maintain compatibility with views)
        /// </summary>
        [Display(Name = "Department")]
        public string? Department => DepartmentName;
    }
}