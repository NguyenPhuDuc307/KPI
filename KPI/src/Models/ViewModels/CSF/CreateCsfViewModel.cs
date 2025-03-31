using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for creating a new Critical Success Factor
    /// </summary>
    public class CreateCsfViewModel
    {
        /// <summary>
        /// Constructor to initialize collections and SelectList properties
        /// </summary>
        public CreateCsfViewModel()
        {
            SelectedKpiIds = new List<Guid>();
            BusinessObjectives = new SelectList(Array.Empty<SelectListItem>());
            Departments = new SelectList(Array.Empty<SelectListItem>());
            AvailableKpis = new SelectList(Array.Empty<SelectListItem>());
        }

        /// <summary>
        /// Name of the Critical Success Factor
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "CSF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Critical Success Factor
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unique code for identifying this CSF
        /// </summary>
        [Required(ErrorMessage = "Code is required")]
        [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Code must contain only uppercase letters, numbers, and hyphens")]
        [Display(Name = "CSF Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The strategic objective this CSF is related to
        /// </summary>
        [StringLength(200, ErrorMessage = "Strategic objective cannot exceed 200 characters")]
        [Display(Name = "Strategic Objective")]
        public string? StrategicObjective { get; set; }

        /// <summary>
        /// Reference to the business objective this CSF supports
        /// </summary>
        [Display(Name = "Business Objective")]
        public Guid? BusinessObjectiveId { get; set; }

        /// <summary>
        /// Priority of this CSF
        /// </summary>
        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        /// <summary>
        /// Status of this CSF
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public CSFStatus Status { get; set; } = CSFStatus.NotStarted;

        /// <summary>
        /// Category of this CSF
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; } = CSFCategory.Other;

        /// <summary>
        /// Start date when this CSF becomes active
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Target date for achieving this CSF
        /// </summary>
        [Required(ErrorMessage = "Target date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddYears(1);

        /// <summary>
        /// Department responsible for this CSF
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this CSF
        /// </summary>
        [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Risk level associated with this CSF
        /// </summary>
        [Required(ErrorMessage = "Risk level is required")]
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

        /// <summary>
        /// Additional notes or comments about this CSF
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Initial progress percentage (usually starts at 0)
        /// </summary>
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        [Display(Name = "Initial Progress (%)")]
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// List of selected KPI IDs to link to this CSF
        /// </summary>
        [Display(Name = "Linked KPIs")]
        public List<Guid> SelectedKpiIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Dropdown items for business objectives
        /// </summary>
        public SelectList BusinessObjectives { get; set; }

        /// <summary>
        /// Dropdown items for departments
        /// </summary>
        public SelectList Departments { get; set; }

        /// <summary>
        /// Dropdown items for available KPIs
        /// </summary>
        public SelectList AvailableKpis { get; set; }

        /// <summary>
        /// Success criteria for this CSF
        /// </summary>
        [Display(Name = "Tiêu chí thành công")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string SuccessCriteria { get; set; } = string.Empty;
    }
}
