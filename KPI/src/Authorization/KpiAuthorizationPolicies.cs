using Microsoft.AspNetCore.Authorization;

namespace KPISolution.Authorization
{
    /// <summary>
    /// Định nghĩa các chính sách ủy quyền cho hệ thống KPI
    /// </summary>
    public static class KpiAuthorizationPolicies
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

            // Chính sách KPI
            public const string CanViewKpis = "CanViewKpis";
            public const string CanManageKpis = "CanManageKpis";
            public const string CanDeleteKpis = "CanDeleteKpis";

            // Chính sách CSF
            public const string CanViewCsfs = "CanViewCsfs";
            public const string CanManageCsfs = "CanManageCsfs";
            public const string CanDeleteCsfs = "CanDeleteCsfs";

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
            public const string Administrator = "Administrator";
            public const string Manager = "Manager";
            public const string User = "User";
            public const string CMO = "CMO";
        }

        /// <summary>
        /// Đăng ký các chính sách vào container DI
        /// </summary>
        /// <param name="services">Container dịch vụ</param>
        public static void AddKpiPolicies(this IServiceCollection services)
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

                // Chính sách KPI
                options.AddPolicy(PolicyNames.CanViewKpis, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.User, RoleNames.CMO));

                options.AddPolicy(PolicyNames.CanManageKpis, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager));

                options.AddPolicy(PolicyNames.CanDeleteKpis, policy =>
                    policy.RequireRole(RoleNames.Administrator));

                // Chính sách CSF
                options.AddPolicy(PolicyNames.CanViewCsfs, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager, RoleNames.CMO, RoleNames.User));

                options.AddPolicy(PolicyNames.CanManageCsfs, policy =>
                    policy.RequireRole(RoleNames.Administrator, RoleNames.Manager));

                options.AddPolicy(PolicyNames.CanDeleteCsfs, policy =>
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
}