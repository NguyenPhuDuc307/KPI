namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// View model for department hierarchical structure.
    /// </summary>
    public class DepartmentHierarchyViewModel
    {
        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        [Display(Name = "Department")]
        public string DepartmentName { get; init; }

        /// <summary>
        /// Gets or sets the parent department ID.
        /// </summary>
        [Display(Name = "Parent Department")]
        public Guid? ParentDepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the parent department name.
        /// </summary>
        [Display(Name = "Parent Department")]
        public string ParentDepartmentName { get; init; }

        /// <summary>
        /// Gets or sets the department level in the hierarchy.
        /// </summary>
        [Display(Name = "Hierarchy Level")]
        public int HierarchyLevel { get; init; }

        /// <summary>
        /// Gets or sets the department head information.
        /// </summary>
        [Display(Name = "Department Head")]
        public string DepartmentHead { get; set; }

        /// <summary>
        /// Gets or sets the list of child departments.
        /// </summary>
        [Display(Name = "Child Departments")]
        public List<DepartmentHierarchyViewModel> ChildDepartments { get; init; }

        /// <summary>
        /// Gets or sets the department path from root.
        /// </summary>
        [Display(Name = "Department Path")]
        public string DepartmentPath { get; init; }

        /// <summary>
        /// Gets or sets whether this department has children.
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// Gets or sets the total number of employees in this department and its children.
        /// </summary>
        [Display(Name = "Total Employees")]
        public int TotalEmployees { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the department.
        /// </summary>
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; init; }

        /// <summary>
        /// Gets or sets the last modified date of the department.
        /// </summary>
        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the department status.
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the department description.
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; init; }

        /// <summary>
        /// Initializes a new instance of the DepartmentHierarchyViewModel class.
        /// </summary>
        public DepartmentHierarchyViewModel()
        {
            this.DepartmentName = string.Empty;
            this.ParentDepartmentName = string.Empty;
            this.DepartmentHead = string.Empty;
            this.DepartmentPath = string.Empty;
            this.Description = string.Empty;
            this.ChildDepartments = [];
            this.HasChildren = false;
            this.Status = "Active";
        }
    }
}
