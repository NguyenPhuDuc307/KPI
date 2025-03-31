using System;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Permission flags for application roles
    /// </summary>
    [Flags]
    public enum KpiPermission : long
    {
        /// <summary>
        /// No permissions
        /// </summary>
        None = 0,

        /// <summary>
        /// View dashboard
        /// </summary>
        ViewDashboard = 1L << 0,

        /// <summary>
        /// View KPI data
        /// </summary>
        ViewKpi = 1L << 1,

        /// <summary>
        /// Edit KPI data
        /// </summary>
        EditKpi = 1L << 2,

        /// <summary>
        /// Create new KPIs
        /// </summary>
        CreateKpi = 1L << 3,

        /// <summary>
        /// Delete KPIs
        /// </summary>
        DeleteKpi = 1L << 4,

        /// <summary>
        /// View CSF data
        /// </summary>
        ViewCsf = 1L << 5,

        /// <summary>
        /// Edit CSF data
        /// </summary>
        EditCsf = 1L << 6,

        /// <summary>
        /// Create new CSFs
        /// </summary>
        CreateCsf = 1L << 7,

        /// <summary>
        /// Delete CSFs
        /// </summary>
        DeleteCsf = 1L << 8,

        /// <summary>
        /// View department data
        /// </summary>
        ViewDepartment = 1L << 9,

        /// <summary>
        /// Edit department data
        /// </summary>
        EditDepartment = 1L << 10,

        /// <summary>
        /// Create new departments
        /// </summary>
        CreateDepartment = 1L << 11,

        /// <summary>
        /// Delete departments
        /// </summary>
        DeleteDepartment = 1L << 12,

        /// <summary>
        /// View user data
        /// </summary>
        ViewUser = 1L << 13,

        /// <summary>
        /// Edit user data
        /// </summary>
        EditUser = 1L << 14,

        /// <summary>
        /// Create new users
        /// </summary>
        CreateUser = 1L << 15,

        /// <summary>
        /// Delete users
        /// </summary>
        DeleteUser = 1L << 16,

        /// <summary>
        /// View role data
        /// </summary>
        ViewRole = 1L << 17,

        /// <summary>
        /// Edit role data
        /// </summary>
        EditRole = 1L << 18,

        /// <summary>
        /// Create new roles
        /// </summary>
        CreateRole = 1L << 19,

        /// <summary>
        /// Delete roles
        /// </summary>
        DeleteRole = 1L << 20,

        /// <summary>
        /// View KPI values and measurements
        /// </summary>
        ViewKpiValue = 1L << 21,

        /// <summary>
        /// Edit KPI values and measurements
        /// </summary>
        EditKpiValue = 1L << 22,

        /// <summary>
        /// Create new KPI values and measurements
        /// </summary>
        CreateKpiValue = 1L << 23,

        /// <summary>
        /// Generate reports
        /// </summary>
        GenerateReports = 1L << 24,

        /// <summary>
        /// Export data
        /// </summary>
        ExportData = 1L << 25,

        /// <summary>
        /// Import data
        /// </summary>
        ImportData = 1L << 26,

        /// <summary>
        /// Configure system settings
        /// </summary>
        ConfigureSystem = 1L << 27,

        /// <summary>
        /// View system logs
        /// </summary>
        ViewLogs = 1L << 28,

        /// <summary>
        /// View all departments' data (not just own)
        /// </summary>
        ViewAllDepartments = 1L << 29,

        /// <summary>
        /// Edit all departments' data (not just own)
        /// </summary>
        EditAllDepartments = 1L << 30,

        /// <summary>
        /// View notifications
        /// </summary>
        ViewNotifications = 1L << 31,

        /// <summary>
        /// Create notifications
        /// </summary>
        CreateNotifications = 1L << 32,

        /// <summary>
        /// Manage notifications
        /// </summary>
        ManageNotifications = 1L << 33,

        /// <summary>
        /// Department Admin - can manage department data
        /// </summary>
        DepartmentAdmin = ViewDepartment | EditDepartment | ViewUser | EditUser | ViewKpi | EditKpi | ViewKpiValue | EditKpiValue | ViewCsf | ViewNotifications,

        /// <summary>
        /// KPI Admin - can manage all KPI-related data
        /// </summary>
        KpiAdmin = ViewKpi | EditKpi | CreateKpi | DeleteKpi | ViewKpiValue | EditKpiValue | CreateKpiValue | ViewCsf | EditCsf | CreateCsf | DeleteCsf | ViewNotifications | CreateNotifications,

        /// <summary>
        /// User Admin - can manage users and roles
        /// </summary>
        UserAdmin = ViewUser | EditUser | CreateUser | DeleteUser | ViewRole | EditRole | CreateRole | DeleteRole,

        /// <summary>
        /// System Admin - has all permissions
        /// </summary>
        SystemAdmin = ~None
    }

    /// <summary>
    /// Permission levels for the application
    /// </summary>
    public enum PermissionLevel
    {
        /// <summary>
        /// Regular user - view only
        /// </summary>
        Regular = 1,

        /// <summary>
        /// Editor - can edit content
        /// </summary>
        Editor = 2,

        /// <summary>
        /// Manager - departmental management
        /// </summary>
        Manager = 3,

        /// <summary>
        /// Administrator - system-wide management
        /// </summary>
        Administrator = 4,

        /// <summary>
        /// System - complete control
        /// </summary>
        System = 5
    }
}
