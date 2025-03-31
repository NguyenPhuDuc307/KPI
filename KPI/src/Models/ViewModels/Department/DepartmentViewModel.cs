using System;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// View model for department information.
    /// </summary>
    public class DepartmentViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        [Required]
        [Display(Name = "Department Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code of the department.
        /// </summary>
        [Required]
        [Display(Name = "Department Code")]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the department.
        /// </summary>
        [Display(Name = "Description")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent department ID.
        /// </summary>
        [Display(Name = "Parent Department")]
        public Guid? ParentDepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the parent department name.
        /// </summary>
        [Display(Name = "Parent Department")]
        public string ParentDepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manager ID.
        /// </summary>
        [Display(Name = "Manager")]
        public string ManagerId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manager name.
        /// </summary>
        [Display(Name = "Manager")]
        public string ManagerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of employees.
        /// </summary>
        [Display(Name = "Number of Employees")]
        public int EmployeeCount { get; set; }

        /// <summary>
        /// Gets or sets the number of KPIs.
        /// </summary>
        [Display(Name = "Number of KPIs")]
        public int KpiCount { get; set; }

        /// <summary>
        /// Gets or sets the overall performance score.
        /// </summary>
        [Display(Name = "Performance Score")]
        public decimal? PerformanceScore { get; set; }

        /// <summary>
        /// Gets or sets the status of the department.
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CSS class for status indication.
        /// </summary>
        public string StatusCssClass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the department was created.
        /// </summary>
        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the department was last updated.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }
    }
}