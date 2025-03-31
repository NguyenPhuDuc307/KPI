using System.Threading.Tasks;
using KPISolution.Models.Entities.KPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using KPISolution.Models.Entities.Identity;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Xử lý ủy quyền cho KPI
    /// </summary>
    public class KpiAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, KpiBase>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public KpiAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Các hoạt động được phép cho KPI
        /// </summary>
        public static class Operations
        {
            public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
            public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
            public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
            public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
            public static readonly OperationAuthorizationRequirement LinkCsf = new() { Name = nameof(LinkCsf) };
            public static readonly OperationAuthorizationRequirement UpdateValue = new() { Name = nameof(UpdateValue) };
        }

        /// <summary>
        /// Xử lý yêu cầu ủy quyền
        /// </summary>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, KpiBase resource)
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

            // Người dùng có thuộc phòng ban quản lý KPI không
            bool isInSameDepartment = false;
            if (user.DepartmentId.HasValue && !string.IsNullOrEmpty(resource.Department))
            {
                // So sánh với tên phòng ban thay vì ID
                isInSameDepartment = user.Department?.Name == resource.Department;
            }

            // Kiểm tra người dùng có phải là người sở hữu KPI không
            bool isOwner = user.IsKpiOwner && (resource.ResponsiblePerson == $"{user.FirstName} {user.LastName}");

            // Người dùng Manager của phòng ban có thể quản lý KPI của phòng đó
            if (isManager && isInSameDepartment)
            {
                context.Succeed(requirement);
                return;
            }

            // Xử lý các quyền cụ thể dựa trên yêu cầu
            switch (requirement.Name)
            {
                case nameof(Operations.Read):
                    // Tất cả người dùng đã xác thực có thể xem KPI
                    context.Succeed(requirement);
                    break;

                case nameof(Operations.Create):
                    // Manager và CMO có thể tạo KPI mới
                    if (isManager || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Update):
                    // Manager của phòng ban và người sở hữu KPI có thể cập nhật KPI
                    if ((isManager && isInSameDepartment) || isOwner || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Delete):
                    // Chỉ Admin và Manager mới có quyền xóa
                    if (isManager && isInSameDepartment)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.LinkCsf):
                    // Manager và CMO có thể liên kết CSF với KPI
                    if ((isManager && isInSameDepartment) || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.UpdateValue):
                    // Manager, CMO và người sở hữu KPI có thể cập nhật giá trị KPI
                    if ((isManager && isInSameDepartment) || isOwner || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;
            }
        }
    }
}