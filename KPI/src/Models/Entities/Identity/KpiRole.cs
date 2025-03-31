using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;

namespace KPISolution.Models.Entities.Identity
{
    /// <summary>
    /// KPI Role class that extends IdentityRole with additional properties
    /// </summary>
    public class KpiRole : IdentityRole
    {
        /// <summary>
        /// Description of the role
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Date and time when the role was created
        /// </summary>
        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User ID who created this role
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the role was last updated
        /// </summary>
        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User ID who last updated this role
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Whether the role is a system role that cannot be modified or deleted
        /// </summary>
        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Whether the role is active
        /// </summary>
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Department ID associated with this role, if applicable
        /// </summary>
        [Display(Name = "Department")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property to the department
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Permission level from 1-5, where 5 is highest permission
        /// </summary>
        [Range(1, 5)]
        [Display(Name = "Permission Level")]
        public int PermissionLevel { get; set; } = 1;

        /// <summary>
        /// Specific permissions assigned to this role
        /// </summary>
        [Display(Name = "Permissions")]
        public KpiPermission Permissions { get; set; } = KpiPermission.None;

        /// <summary>
        /// Gets the permission level as enum value
        /// </summary>
        [NotMapped]
        public PermissionLevel PermissionLevelEnum
        {
            get => (PermissionLevel)PermissionLevel;
            set => PermissionLevel = (int)value;
        }
    }
}
