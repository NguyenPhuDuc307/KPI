using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Organization
{
    /// <summary>
    /// Represents a department within the organization
    /// </summary>
    public class Department : BaseEntity
    {
        /// <summary>
        /// Name of the department
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Department code or identifier
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Department Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Description of the department
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Parent department ID, if this is a sub-department
        /// </summary>
        [Display(Name = "Parent Department")]
        public Guid? ParentDepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the parent department
        /// </summary>
        [ForeignKey("ParentDepartmentId")]
        public Department? ParentDepartment { get; init; }

        /// <summary>
        /// Collection of child departments
        /// </summary>
        public virtual ICollection<Department>? ChildDepartments { get; init; }

        /// <summary>
        /// ID of the department head or manager
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Department Head")]
        public string? DepartmentHeadId { get; set; }

        /// <summary>
        /// Navigation property to the department head
        /// </summary>
        [ForeignKey("DepartmentHeadId")]
        public virtual ApplicationUser? DepartmentHead { get; init; }

        /// <summary>
        /// Collection of employees in this department
        /// </summary>
        public virtual ICollection<ApplicationUser>? Employees { get; init; }

        /// <summary>
        /// Email address of the department
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Department Email")]
        public string? Email { get; init; }

        /// <summary>
        /// Phone number of the department
        /// </summary>
        [StringLength(30)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; init; }

        /// <summary>
        /// Location of the department
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Location")]
        public string? Location { get; init; }

        /// <summary>
        /// Budget allocated to the department
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Budget")]
        public decimal? Budget { get; init; }

        /// <summary>
        /// Date when the department was established
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Established Date")]
        public DateTime? EstablishedDate { get; init; }

        /// <summary>
        /// Department hierarchy level (0 for top level)
        /// </summary>
        [Display(Name = "Hierarchy Level")]
        public int HierarchyLevel { get; set; } = 0;

        /// <summary>
        /// Department's priority or importance within the organization (1-5, where 5 is highest)
        /// </summary>
        [Range(1, 5)]
        [Display(Name = "Priority")]
        public int? Priority { get; init; }
    }
}
