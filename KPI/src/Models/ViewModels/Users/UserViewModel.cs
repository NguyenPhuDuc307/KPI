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
        public string Id { get; init; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Username for login
        /// </summary>
        public string UserName { get; init; } = string.Empty;

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// Full name (first + last)
        /// </summary>
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Job title or position
        /// </summary>
        public string JobTitle { get; init; } = string.Empty;

        /// <summary>
        /// Whether the account is active
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Comma-separated list of roles
        /// </summary>
        public string Roles { get; init; } = string.Empty;

        /// <summary>
        /// Department ID
        /// </summary>
        public Guid? DepartmentId { get; init; }

        /// <summary>
        /// Department name
        /// </summary>
        public string DepartmentName { get; init; } = string.Empty;

        /// <summary>
        /// Manager's user ID
        /// </summary>
        public string ManagerId { get; init; } = string.Empty;

        /// <summary>
        /// Manager's name
        /// </summary>
        public string ManagerName { get; init; } = string.Empty;

        /// <summary>
        /// Date when user was created
        /// </summary>
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Date of last login
        /// </summary>
        public DateTime? LastLoginAt { get; init; }

        /// <summary>
        /// Whether user is an indicator owner
        /// </summary>
        public bool IsIndicatorOwner { get; init; }

        /// <summary>
        /// Whether user is a department admin
        /// </summary>
        public bool IsDepartmentAdmin { get; init; }

        /// <summary>
        /// Whether user is a KPI owner
        /// </summary>
        public bool IsKpiOwner { get; init; }
    }
}
