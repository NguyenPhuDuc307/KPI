namespace KPISolution.Models.ViewModels.Users
{
    /// <summary>
    /// View model for displaying user information
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Username for login
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Full name (first + last)
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Job title or position
        /// </summary>
        public string JobTitle { get; set; } = string.Empty;

        /// <summary>
        /// Whether the account is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Comma-separated list of roles
        /// </summary>
        public string Roles { get; set; } = string.Empty;

        /// <summary>
        /// Department ID
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Manager's user ID
        /// </summary>
        public string ManagerId { get; set; } = string.Empty;

        /// <summary>
        /// Manager's name
        /// </summary>
        public string ManagerName { get; set; } = string.Empty;

        /// <summary>
        /// Date when user was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date of last login
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Whether user is an indicator owner
        /// </summary>
        public bool IsIndicatorOwner { get; set; }

        /// <summary>
        /// Whether user is a department admin
        /// </summary>
        public bool IsDepartmentAdmin { get; set; }

        /// <summary>
        /// Whether user is a KPI owner
        /// </summary>
        public bool IsKpiOwner { get; set; }
    }
}
