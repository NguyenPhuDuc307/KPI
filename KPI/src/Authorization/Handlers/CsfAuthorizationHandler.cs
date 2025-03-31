using System.Threading.Tasks;
using KPISolution.Models.Entities.CSF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using KPISolution.Models.Entities.Identity;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Xử lý ủy quyền cho CSF
    /// </summary>
    public class CsfAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CriticalSuccessFactor>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public CsfAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Các hoạt động được phép cho CSF
        /// </summary>
        public static class Operations
        {
            public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
            public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
            public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
            public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
            public static readonly OperationAuthorizationRequirement LinkKpi = new() { Name = nameof(LinkKpi) };
        }

        /// <summary>
        /// Xử lý yêu cầu ủy quyền
        /// </summary>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, CriticalSuccessFactor resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            // Kiểm tra người dùng có thuộc vai trò Administrator không
            var isAdmin = await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Administrator);
            if (isAdmin)
            {
                // Người dùng Admin có toàn quyền
                context.Succeed(requirement);
                return;
            }

            // Kiểm tra các vai trò Manager và CMO
            var isManager = await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Manager);
            var isCmo = await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.CMO);

            // Người dùng có thuộc phòng ban quản lý CSF không
            bool isInSameDepartment = false;
            if (user.DepartmentId.HasValue && resource.DepartmentId.HasValue)
            {
                isInSameDepartment = user.DepartmentId.Value == resource.DepartmentId.Value;
            }

            // Người dùng Manager của phòng ban có thể quản lý CSF của phòng đó
            if (isManager && isInSameDepartment)
            {
                context.Succeed(requirement);
                return;
            }

            // Xử lý các quyền cụ thể dựa trên yêu cầu
            switch (requirement.Name)
            {
                case nameof(Operations.Read):
                    // Tất cả người dùng đã xác thực có thể xem CSF
                    context.Succeed(requirement);
                    break;

                case nameof(Operations.Create):
                    // Manager và CMO có thể tạo CSF mới
                    if (isManager || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Update):
                    // Manager của phòng ban có thể cập nhật CSF
                    if ((isManager || isCmo) && isInSameDepartment)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Delete):
                    // Chỉ Admin mới có quyền xóa (đã xử lý ở trên)
                    break;

                case nameof(Operations.LinkKpi):
                    // Manager và CMO có thể liên kết KPI với CSF
                    if ((isManager || isCmo) && isInSameDepartment)
                    {
                        context.Succeed(requirement);
                    }
                    break;
            }
        }
    }
}