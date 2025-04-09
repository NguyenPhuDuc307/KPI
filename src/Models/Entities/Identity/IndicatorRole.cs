using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Identity
{
    /// <summary>
    /// Enum defining indicator permissions
    /// </summary>
    [Flags]
    public enum IndicatorPermission
    {
        /// <summary>
        /// No permissions
        /// </summary>
        None = 0,

        /// <summary>
        /// Can view indicator data
        /// </summary>
        View = 1,

        /// <summary>
        /// Can create indicators
        /// </summary>
        Create = 2,

        /// <summary>
        /// Can edit indicator data
        /// </summary>
        Edit = 4,

        /// <summary>
        /// Can delete indicators
        /// </summary>
        Delete = 8,

        /// <summary>
        /// Can approve indicators
        /// </summary>
        Approve = 16,

        /// <summary>
        /// Can assign indicators to users
        /// </summary>
        Assign = 32,

        /// <summary>
        /// Can manage indicator settings
        /// </summary>
        ManageSettings = 64,

        /// <summary>
        /// Can view all indicators across the organization
        /// </summary>
        ViewAll = 128,

        /// <summary>
        /// Can export indicator data
        /// </summary>
        Export = 256,

        /// <summary>
        /// Can import indicator data
        /// </summary>
        Import = 512,

        /// <summary>
        /// Administrator permissions (all permissions)
        /// </summary>
        Admin = View | Create | Edit | Delete | Approve | Assign | ManageSettings | ViewAll | Export | Import
    }

    /// <summary>
    /// Enum representing permission levels
    /// </summary>
    public enum PermissionLevel
    {
        /// <summary>
        /// Basic viewer - can only view data
        /// </summary>
        Viewer = 1,

        /// <summary>
        /// Contributor - can add and edit data
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// Editor - can edit settings and data
        /// </summary>
        Editor = 3,

        /// <summary>
        /// Manager - can approve and manage users
        /// </summary>
        Manager = 4,

        /// <summary>
        /// Administrator - has all permissions
        /// </summary>
        Administrator = 5
    }

    /// <summary>
    /// Indicator Role class that extends IdentityRole with additional properties
    /// </summary>
    public class IndicatorRole : IdentityRole
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
        public IndicatorPermission Permissions { get; set; } = IndicatorPermission.None;

        /// <summary>
        /// Gets the permission level as enum value
        /// </summary>
        [NotMapped]
        public PermissionLevel PermissionLevelEnum
        {
            get => (PermissionLevel)this.PermissionLevel;
            init => this.PermissionLevel = (int)value;
        }
    }
}
