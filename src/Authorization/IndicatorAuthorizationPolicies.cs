namespace KPISolution.Authorization
{
    /// <summary>
    /// Định nghĩa các chính sách ủy quyền cho hệ thống Indicator
    /// </summary>
    public static class IndicatorAuthorizationPolicies
    {
        /// <summary>
        /// Tên của các chính sách
        /// </summary>
        public static class PolicyNames
        {
            // Chính sách quản trị
            public const string RequireAdministratorRole = "RequireAdministratorRole";
            public const string RequireManagerRole = "RequireManagerRole";
            public const string RequireCmoRole = "RequireCmoRole";

            // Chính sách Indicator
            public const string CanViewIndicators = "CanViewIndicators";
            public const string CanManageIndicators = "CanManageIndicators";
            public const string CanDeleteIndicators = "CanDeleteIndicators";

            // Chính sách CSF -> SuccessFactor
            public const string CanViewSuccessFactors = "CanViewSuccessFactors";
            public const string CanManageSuccessFactors = "CanManageSuccessFactors";
            public const string CanDeleteSuccessFactors = "CanDeleteSuccessFactors";

            // Chính sách Dashboard
            public const string CanViewDashboards = "CanViewDashboards";
            public const string CanConfigureDashboards = "CanConfigureDashboards";

            // Chính sách báo cáo
            public const string CanGenerateReports = "CanGenerateReports";
            public const string CanExportData = "CanExportData";
        }

        /// <summary>
        /// Tên các vai trò trong hệ thống
        /// </summary>
        public static class RoleNames
        {
            /// <summary>
            /// Vai trò quản trị hệ thống
            /// </summary>
            public const string Administrator = "Administrator";

            /// <summary>
            /// Vai trò quản lý
            /// </summary>
            public const string Manager = "Manager";

            /// <summary>
            /// Vai trò người dùng thông thường
            /// </summary>
            public const string User = "User";

            /// <summary>
            /// Vai trò khách
            /// </summary>
            public const string Guest = "Guest";

            /// <summary>
            /// Vai trò chủ sở hữu chỉ số
            /// </summary>
            public const string IndicatorOwner = "IndicatorOwner";

            /// <summary>
            /// Vai trò quản lý phòng ban
            /// </summary>
            public const string DepartmentManager = "DepartmentManager";

            /// <summary>
            /// Vai trò CMO
            /// </summary>
            public const string CMO = "CMO";
        }

        /// <summary>
        /// Đăng ký các chính sách vào container DI
        /// </summary>
        /// <param name="services">Container dịch vụ</param>
        public static void AddIndicatorPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Chính sách vai trò
                options.AddPolicy(PolicyNames.RequireAdministratorRole, policy =>
                    policy.RequireRole(RoleNames.Administrator));

                options.AddPolicy(PolicyNames.RequireManagerRole, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager));

                options.AddPolicy(PolicyNames.RequireCmoRole, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.CMO));

                // Chính sách Indicator
                options.AddPolicy(PolicyNames.CanViewIndicators, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.User, RoleNames.CMO));

                options.AddPolicy(PolicyNames.CanManageIndicators, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager));

                options.AddPolicy(PolicyNames.CanDeleteIndicators, policy =>
                    policy.RequireRole(RoleNames.Administrator));

                // Chính sách CSF -> SuccessFactor
                options.AddPolicy(PolicyNames.CanViewSuccessFactors, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.CMO, RoleNames.User));

                options.AddPolicy(PolicyNames.CanManageSuccessFactors, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager));

                options.AddPolicy(PolicyNames.CanDeleteSuccessFactors, policy =>
                    policy.RequireRole(RoleNames.Administrator));

                // Chính sách Dashboard
                options.AddPolicy(PolicyNames.CanViewDashboards, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.CMO, RoleNames.User));

                options.AddPolicy(PolicyNames.CanConfigureDashboards, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.CMO));

                // Chính sách báo cáo
                options.AddPolicy(PolicyNames.CanGenerateReports, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.CMO));

                options.AddPolicy(PolicyNames.CanExportData, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.CMO));
            });
        }
    }

    /// <summary>
    /// Policy names for Indicator authorization
    /// </summary>
    public static class IndicatorPermissionPolicies
    {
        /// <summary>
        /// Policy for viewing Indicators
        /// </summary>
        public const string ViewIndicators = "ViewIndicators";

        /// <summary>
        /// Policy for creating Indicators
        /// </summary>
        public const string CreateIndicators = "CreateIndicators";

        /// <summary>
        /// Policy for editing Indicators
        /// </summary>
        public const string EditIndicators = "EditIndicators";

        /// <summary>
        /// Policy for deleting Indicators
        /// </summary>
        public const string DeleteIndicators = "DeleteIndicators";

        /// <summary>
        /// Policy for managing Indicators (view, create, edit, delete)
        /// </summary>
        public const string ManageIndicators = "ManageIndicators";

        /// <summary>
        /// Policy for viewing measurements
        /// </summary>
        public const string ViewMeasurement = "ViewMeasurement";

        /// <summary>
        /// Policy for creating measurements
        /// </summary>
        public const string CreateMeasurement = "CreateMeasurement";

        /// <summary>
        /// Policy for editing measurements
        /// </summary>
        public const string EditMeasurement = "EditMeasurement";

        /// <summary>
        /// Policy for deleting measurements
        /// </summary>
        public const string DeleteMeasurement = "DeleteMeasurement";

        /// <summary>
        /// Policy for approving measurements
        /// </summary>
        public const string ApproveMeasurement = "ApproveMeasurement";

        /// <summary>
        /// Policy for managing measurements (view, create, edit, delete, approve)
        /// </summary>
        public const string ManageMeasurement = "ManageMeasurement";

        /// <summary>
        /// Policy for viewing departments
        /// </summary>
        public const string ViewDepartment = "ViewDepartment";

        /// <summary>
        /// Policy for managing departments
        /// </summary>
        public const string ManageDepartment = "ManageDepartment";

        /// <summary>
        /// Policy for managing users
        /// </summary>
        public const string ManageUsers = "ManageUsers";

        /// <summary>
        /// Policy for managing roles
        /// </summary>
        public const string ManageRoles = "ManageRoles";

        /// <summary>
        /// Policy for viewing dashboards
        /// </summary>
        public const string ViewDashboard = "ViewDashboard";

        /// <summary>
        /// Policy for viewing reports
        /// </summary>
        public const string ViewReports = "ViewReports";

        /// <summary>
        /// Policy for exporting data
        /// </summary>
        public const string ExportData = "ExportData";

        /// <summary>
        /// Policy for importing data
        /// </summary>
        public const string ImportData = "ImportData";

        /// <summary>
        /// Policy for system administration
        /// </summary>
        public const string SystemAdmin = "SystemAdmin";
    }
}
