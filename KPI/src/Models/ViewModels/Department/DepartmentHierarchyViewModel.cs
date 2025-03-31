using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the parent department ID.
        /// </summary>
        [Display(Name = "Parent Department")]
        public Guid? ParentDepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the parent department name.
        /// </summary>
        [Display(Name = "Parent Department")]
        public string ParentDepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the department level in the hierarchy.
        /// </summary>
        [Display(Name = "Hierarchy Level")]
        public int HierarchyLevel { get; set; }

        /// <summary>
        /// Gets or sets the department head information.
        /// </summary>
        [Display(Name = "Department Head")]
        public string DepartmentHead { get; set; }

        /// <summary>
        /// Gets or sets the list of child departments.
        /// </summary>
        [Display(Name = "Child Departments")]
        public List<DepartmentHierarchyViewModel> ChildDepartments { get; set; }

        /// <summary>
        /// Gets or sets the department path from root.
        /// </summary>
        [Display(Name = "Department Path")]
        public string DepartmentPath { get; set; }

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
        public DateTime CreatedDate { get; set; }

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
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the DepartmentHierarchyViewModel class.
        /// </summary>
        public DepartmentHierarchyViewModel()
        {
            DepartmentName = string.Empty;
            ParentDepartmentName = string.Empty;
            DepartmentHead = string.Empty;
            DepartmentPath = string.Empty;
            Description = string.Empty;
            ChildDepartments = new List<DepartmentHierarchyViewModel>();
            HasChildren = false;
            Status = "Active";
        }
    }
}