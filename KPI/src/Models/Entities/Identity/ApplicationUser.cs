using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using KPISolution.Models.Entities.Organization;

namespace KPISolution.Models.Entities.Identity
{
    /// <summary>
    /// Application user class that extends IdentityUser with additional properties
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// User's first name
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's full name (read-only, computed from first and last name)
        /// </summary>
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// User's job title
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        /// <summary>
        /// Department ID the user belongs to
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the user's department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// User's manager or supervisor ID
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Manager ID")]
        public string? ManagerId { get; set; }

        /// <summary>
        /// Navigation property to the user's manager
        /// </summary>
        [ForeignKey("ManagerId")]
        public virtual ApplicationUser? Manager { get; set; }

        /// <summary>
        /// Collection of users this user manages
        /// </summary>
        public virtual ICollection<ApplicationUser>? DirectReports { get; set; }

        /// <summary>
        /// User's position within the department hierarchy (0=highest)
        /// </summary>
        [Range(0, 10)]
        [Display(Name = "Hierarchy Level")]
        public int? HierarchyLevel { get; set; }

        /// <summary>
        /// Employee ID or staff number
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Employee ID")]
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Date the user joined the organization
        /// </summary>
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// User's profile picture URL
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Profile Picture")]
        [DataType(DataType.ImageUrl)]
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Whether the user is active in the system
        /// </summary>
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date and time when the user account was created
        /// </summary>
        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the user last logged in
        /// </summary>
        [Display(Name = "Last Login")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// User's notification preferences as JSON string
        /// </summary>
        [Display(Name = "Notification Preferences")]
        public string? NotificationPreferences { get; set; }

        /// <summary>
        /// User's dashboard preferences as JSON string
        /// </summary>
        [Display(Name = "Dashboard Preferences")]
        public string? DashboardPreferences { get; set; }

        /// <summary>
        /// KPIs this user is responsible for
        /// </summary>
        [Display(Name = "Is KPI Owner")]
        public bool IsKpiOwner { get; set; } = false;

        /// <summary>
        /// Whether this user has admin privileges for their department
        /// </summary>
        [Display(Name = "Is Department Admin")]
        public bool IsDepartmentAdmin { get; set; } = false;
    }
}
