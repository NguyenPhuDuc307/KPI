namespace KPISolution.Models.ViewModels.Dashboard
{
    /// <summary>
    /// View model for the department overview dashboard
    /// </summary>
    public class DepartmentOverviewViewModel
    {
        /// <summary>
        /// Gets or sets the title of the dashboard
        /// </summary>
        public string Title { get; set; } = "Dashboard phòng ban";

        /// <summary>
        /// Gets or sets the last updated date/time
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the list of departments with performance metrics
        /// </summary>
        public List<DepartmentPerformanceViewModel> Departments { get; set; } = new List<DepartmentPerformanceViewModel>();

        /// <summary>
        /// Gets or sets the total number of departments
        /// </summary>
        public int TotalDepartments => this.Departments.Count;

        /// <summary>
        /// Gets or sets the number of departments performing well (>= 80%)
        /// </summary>
        public int PerformingWellCount => this.Departments.Count(d => d.PerformanceScore >= 80);

        /// <summary>
        /// Gets or sets the number of departments requiring attention (< 60%)
        /// </summary>
        public int RequiringAttentionCount => this.Departments.Count(d => d.PerformanceScore < 60);

        /// <summary>
        /// Gets the percentage of departments performing well
        /// </summary>
        public decimal PerformingWellPercentage =>
            this.TotalDepartments > 0
            ? (decimal)this.PerformingWellCount / this.TotalDepartments * 100
            : 0;
    }

    /// <summary>
    /// View model for department summary in dashboards
    /// </summary>
    public class DepartmentSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the department ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the department name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manager name
        /// </summary>
        public string ManagerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent department name, if any
        /// </summary>
        public string? ParentDepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the number of employees
        /// </summary>
        public int EmployeeCount { get; set; }
    }
}