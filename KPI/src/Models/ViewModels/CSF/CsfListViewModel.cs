using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.CSF
{
    /// <summary>
    /// View model for displaying a list of Critical Success Factors with filtering options
    /// </summary>
    public class CsfListViewModel
    {
        /// <summary>
        /// Collection of CSF items to display
        /// </summary>
        public List<CsfListItemViewModel> CsfItems { get; set; } = new List<CsfListItemViewModel>();

        /// <summary>
        /// Total count of CSFs matching the filter criteria
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number for pagination
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Number of items to display per page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Total number of pages based on TotalCount and PageSize
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Filter criteria for CSFs
        /// </summary>
        public CsfFilterViewModel Filter { get; set; } = new CsfFilterViewModel();

        /// <summary>
        /// Dropdown items for departments
        /// </summary>
        public SelectList? Departments { get; set; }

        /// <summary>
        /// Dropdown items for categories
        /// </summary>
        public SelectList? Categories { get; set; }

        /// <summary>
        /// Dropdown items for priorities
        /// </summary>
        public SelectList? Priorities { get; set; }

        /// <summary>
        /// Dropdown items for statuses
        /// </summary>
        public SelectList? Statuses { get; set; }

        /// <summary>
        /// Dropdown items for risk levels
        /// </summary>
        public SelectList? RiskLevels { get; set; }

        /// <summary>
        /// Sort options for the dropdown
        /// </summary>
        public SelectList SortOptions { get; set; } = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "Name", Text = "Name" },
            new SelectListItem { Value = "Code", Text = "Code" },
            new SelectListItem { Value = "Priority", Text = "Priority" },
            new SelectListItem { Value = "Status", Text = "Status" },
            new SelectListItem { Value = "Progress", Text = "Progress" },
            new SelectListItem { Value = "Category", Text = "Category" },
            new SelectListItem { Value = "TargetDate", Text = "Target Date" },
            new SelectListItem { Value = "Department", Text = "Department" }
        }, "Value", "Text");

        /// <summary>
        /// Flag to show archived CSFs
        /// </summary>
        public bool ShowArchived { get; set; } = false;

        /// <summary>
        /// Period start date for filtering
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? PeriodStart { get; set; }

        /// <summary>
        /// Period end date for filtering
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? PeriodEnd { get; set; }

        /// <summary>
        /// Flag indicating whether there is a next page
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Flag indicating whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Flag indicating whether there are any items in the list
        /// </summary>
        public bool HasItems => CsfItems.Any();
    }

    /// <summary>
    /// View model for an individual CSF item in a list
    /// </summary>
    public class CsfListItemViewModel
    {
        /// <summary>
        /// Unique identifier for the CSF
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the Critical Success Factor
        /// </summary>
        [Display(Name = "CSF Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Unique code for identifying this CSF
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Brief description of the CSF
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of this CSF
        /// </summary>
        [Display(Name = "Category")]
        public CSFCategory Category { get; set; }

        /// <summary>
        /// Category as a display-friendly string
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Priority of this CSF
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Priority as a display-friendly string
        /// </summary>
        [Display(Name = "Priority")]
        public string PriorityDisplay { get; set; } = string.Empty;

        /// <summary>
        /// Current status of this CSF
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
        /// Department responsible for this CSF
        /// </summary>
        [Display(Name = "Department")]
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Person who owns or is responsible for this CSF
        /// </summary>
        [Display(Name = "Owner")]
        public string? Owner { get; set; }

        /// <summary>
        /// Risk level associated with this CSF
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Risk level as a display-friendly string
        /// </summary>
        [Display(Name = "Risk")]
        public string RiskLevelDisplay { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for risk level styling
        /// </summary>
        public string RiskLevelCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Current progress percentage
        /// </summary>
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// CSS class for progress styling
        /// </summary>
        public string ProgressCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Target date for achieving this CSF
        /// </summary>
        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Creation date of the CSF
        /// </summary>
        [Display(Name = "Created On")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update date of the CSF
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Number of KPIs linked to this CSF
        /// </summary>
        [Display(Name = "Linked KPIs")]
        public int LinkedKpisCount { get; set; }

        /// <summary>
        /// Flag indicating if the CSF is on track
        /// </summary>
        [Display(Name = "On Track")]
        public bool IsOnTrack { get; set; }

        /// <summary>
        /// Number of days remaining until target date
        /// </summary>
        [Display(Name = "Days Remaining")]
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Flag indicating if this CSF needs attention (based on risk or NeedsAttention flag in progress updates)
        /// </summary>
        [Display(Name = "Needs Attention")]
        public bool NeedsAttention { get; set; }
    }

    /// <summary>
    /// View model for CSF filtering criteria
    /// </summary>
    public class CsfFilterViewModel
    {
        /// <summary>
        /// Search term for filtering by name, code, or description
        /// </summary>
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filter by category
        /// </summary>
        [Display(Name = "Category")]
        public CSFCategory? Category { get; set; }

        /// <summary>
        /// Filter by priority
        /// </summary>
        [Display(Name = "Priority")]
        public PriorityLevel? Priority { get; set; }

        /// <summary>
        /// Filter by status
        /// </summary>
        [Display(Name = "Status")]
        public CSFStatus? Status { get; set; }

        /// <summary>
        /// Filter by department
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Filter by risk level
        /// </summary>
        [Display(Name = "Risk Level")]
        public RiskLevel? RiskLevel { get; set; }

        /// <summary>
        /// Filter by on track status
        /// </summary>
        [Display(Name = "On Track")]
        public bool? IsOnTrack { get; set; }

        /// <summary>
        /// Filter by needs attention flag
        /// </summary>
        [Display(Name = "Needs Attention")]
        public bool? NeedsAttention { get; set; }

        /// <summary>
        /// Filter by completion status
        /// </summary>
        [Display(Name = "Completed")]
        public bool? IsCompleted { get; set; }

        /// <summary>
        /// Minimum progress percentage for filtering
        /// </summary>
        [Display(Name = "Min Progress (%)")]
        [Range(0, 100)]
        public int? MinProgress { get; set; }

        /// <summary>
        /// Maximum progress percentage for filtering
        /// </summary>
        [Display(Name = "Max Progress (%)")]
        [Range(0, 100)]
        public int? MaxProgress { get; set; }

        /// <summary>
        /// Sort field for ordering results
        /// </summary>
        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "Name";

        /// <summary>
        /// Sort direction (ascending or descending)
        /// </summary>
        [Display(Name = "Sort Direction")]
        public string SortDirection { get; set; } = "asc";
    }
}
