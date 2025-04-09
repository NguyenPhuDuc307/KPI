namespace KPISolution.Models.Enums.Auth
{
    /// <summary>
    /// Enum defining permission types for the application
    /// </summary>
    [Flags]
    public enum Permission
    {
        /// <summary>
        /// No permissions
        /// </summary>
        None = 0,

        /// <summary>
        /// Permission to view indicators
        /// </summary>
        [Display(Name = "View Indicators")]
        ViewIndicators = 1 << 0,

        /// <summary>
        /// Permission to create new indicators
        /// </summary>
        [Display(Name = "Create Indicators")]
        CreateIndicators = 1 << 1,

        /// <summary>
        /// Permission to edit existing indicators
        /// </summary>
        [Display(Name = "Edit Indicators")]
        EditIndicators = 1 << 2,

        /// <summary>
        /// Permission to delete indicators
        /// </summary>
        [Display(Name = "Delete Indicators")]
        DeleteIndicators = 1 << 3,

        /// <summary>
        /// Permission to approve indicators
        /// </summary>
        [Display(Name = "Approve Indicators")]
        ApproveIndicators = 1 << 4,

        /// <summary>
        /// Permission to record measurements
        /// </summary>
        [Display(Name = "Record Measurements")]
        RecordMeasurements = 1 << 5,

        /// <summary>
        /// Permission to verify measurements
        /// </summary>
        [Display(Name = "Verify Measurements")]
        VerifyMeasurements = 1 << 6,

        /// <summary>
        /// Permission to view dashboards
        /// </summary>
        [Display(Name = "View Dashboards")]
        ViewDashboards = 1 << 7,

        /// <summary>
        /// Permission to create and edit dashboards
        /// </summary>
        [Display(Name = "Edit Dashboards")]
        EditDashboards = 1 << 8,

        /// <summary>
        /// Permission to view reports
        /// </summary>
        [Display(Name = "View Reports")]
        ViewReports = 1 << 9,

        /// <summary>
        /// Permission to create and edit reports
        /// </summary>
        [Display(Name = "Create Reports")]
        CreateReports = 1 << 10,

        /// <summary>
        /// Permission to manage user accounts
        /// </summary>
        [Display(Name = "Manage Users")]
        ManageUsers = 1 << 11,

        /// <summary>
        /// Permission to manage roles
        /// </summary>
        [Display(Name = "Manage Roles")]
        ManageRoles = 1 << 12,

        /// <summary>
        /// Permission to manage system settings
        /// </summary>
        [Display(Name = "Manage Settings")]
        ManageSettings = 1 << 13,

        /// <summary>
        /// Permission to export data
        /// </summary>
        [Display(Name = "Export Data")]
        ExportData = 1 << 14,

        /// <summary>
        /// Permission to import data
        /// </summary>
        [Display(Name = "Import Data")]
        ImportData = 1 << 15,

        /// <summary>
        /// Full administrator permissions
        /// </summary>
        [Display(Name = "Administrator")]
        Administrator = ~0
    }

    /// <summary>
    /// Enum representing user roles in the system
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// System administrator with full permissions
        /// </summary>
        [Display(Name = "System Administrator")]
        SystemAdmin = 1,

        /// <summary>
        /// Manager with permissions to manage indicators and view reports
        /// </summary>
        [Display(Name = "Manager")]
        Manager = 2,

        /// <summary>
        /// User who can record measurements and view assigned indicators
        /// </summary>
        [Display(Name = "Contributor")]
        Contributor = 3,

        /// <summary>
        /// User who can only view dashboard and reports, no edit permissions
        /// </summary>
        [Display(Name = "Viewer")]
        Viewer = 4,

        /// <summary>
        /// Auditor who can verify measurements but not create/edit
        /// </summary>
        [Display(Name = "Auditor")]
        Auditor = 5,

        /// <summary>
        /// Department administrator who manages indicators for a specific department
        /// </summary>
        [Display(Name = "Department Admin")]
        DepartmentAdmin = 6
    }
}
