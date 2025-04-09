namespace KPISolution.Models.ViewModels.Department
{
    /// <summary>
    /// View model for department resource utilization information.
    /// </summary>
    public class DepartmentResourceViewModel
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
        /// Gets or sets the total number of employees.
        /// </summary>
        [Display(Name = "Total Employees")]
        public int TotalEmployees { get; set; }

        /// <summary>
        /// Gets or sets the number of managers.
        /// </summary>
        [Display(Name = "Managers")]
        public int ManagerCount { get; set; }

        /// <summary>
        /// Gets or sets the number of staff.
        /// </summary>
        [Display(Name = "Staff")]
        public int StaffCount { get; set; }

        /// <summary>
        /// Gets or sets the employee allocation by project.
        /// </summary>
        [Display(Name = "Project Allocation")]
        public Dictionary<string, int> ProjectAllocation { get; set; }

        /// <summary>
        /// Gets or sets the skill distribution.
        /// </summary>
        [Display(Name = "Skill Distribution")]
        public Dictionary<string, int> SkillDistribution { get; set; }

        /// <summary>
        /// Gets or sets the resource utilization rate.
        /// </summary>
        [Display(Name = "Utilization Rate")]
        public decimal UtilizationRate { get; set; }

        /// <summary>
        /// Gets or sets the resource availability.
        /// </summary>
        [Display(Name = "Resource Availability")]
        public decimal ResourceAvailability { get; set; }

        /// <summary>
        /// Gets or sets the workload distribution.
        /// </summary>
        [Display(Name = "Workload Distribution")]
        public Dictionary<string, decimal> WorkloadDistribution { get; set; }

        /// <summary>
        /// Gets or sets the resource efficiency score.
        /// </summary>
        [Display(Name = "Efficiency Score")]
        public decimal EfficiencyScore { get; set; }

        /// <summary>
        /// Gets or sets the resource cost information.
        /// </summary>
        [Display(Name = "Resource Costs")]
        public Dictionary<string, decimal> ResourceCosts { get; set; }

        /// <summary>
        /// Gets or sets the training and development metrics.
        /// </summary>
        [Display(Name = "Training Metrics")]
        public Dictionary<string, decimal> TrainingMetrics { get; set; }

        /// <summary>
        /// Gets or sets the last updated timestamp.
        /// </summary>
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Initializes a new instance of the DepartmentResourceViewModel class.
        /// </summary>
        public DepartmentResourceViewModel()
        {
            this.DepartmentName = string.Empty;
            this.ProjectAllocation = new Dictionary<string, int>();
            this.SkillDistribution = new Dictionary<string, int>();
            this.WorkloadDistribution = new Dictionary<string, decimal>();
            this.ResourceCosts = new Dictionary<string, decimal>();
            this.TrainingMetrics = new Dictionary<string, decimal>();
        }
    }
}
