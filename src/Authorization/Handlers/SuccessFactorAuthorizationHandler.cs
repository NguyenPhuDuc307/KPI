using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Xử lý ủy quyền cho SuccessFactor
    /// </summary>
    public class SuccessFactorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, SuccessFactor>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public SuccessFactorAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        /// <summary>
        /// Các hoạt động được phép cho SuccessFactor
        /// </summary>
        public static class Operations
        {
            public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
            public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
            public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
            public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
            public static readonly OperationAuthorizationRequirement LinkIndicator = new() { Name = nameof(LinkIndicator) };
        }

        /// <summary>
        /// Xử lý yêu cầu ủy quyền
        /// </summary>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, SuccessFactor resource)
        {
            var user = await this._userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            // Kiểm tra người dùng có thuộc vai trò Administrator không
            var isAdmin = await this._userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.Administrator);
            if (isAdmin)
            {
                // Người dùng Admin có toàn quyền
                context.Succeed(requirement);
                return;
            }

            // Kiểm tra các vai trò Manager và CMO
            var isManager = await this._userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.Manager);
            var isCmo = await this._userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.CMO);

            // Người dùng có thuộc phòng ban quản lý SuccessFactor không
            bool isInSameDepartment = false;
            if (user.DepartmentId.HasValue && resource.DepartmentId.HasValue)
            {
                isInSameDepartment = user.DepartmentId.Value == resource.DepartmentId.Value;
            }

            // Người dùng Manager của phòng ban có thể quản lý SuccessFactor của phòng đó
            if (isManager && isInSameDepartment)
            {
                context.Succeed(requirement);
                return;
            }

            // Xử lý các quyền cụ thể dựa trên yêu cầu
            switch (requirement.Name)
            {
                case nameof(Operations.Read):
                    // Tất cả người dùng đã xác thực có thể xem SuccessFactor
                    context.Succeed(requirement);
                    break;

                case nameof(Operations.Create):
                    // Manager và CMO có thể tạo SuccessFactor mới
                    if (isManager || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Update):
                    // Manager của phòng ban có thể cập nhật SuccessFactor
                    if ((isManager || isCmo) && isInSameDepartment)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Delete):
                    // Chỉ Admin mới có quyền xóa (đã xử lý ở trên)
                    break;

                case nameof(Operations.LinkIndicator):
                    // Manager và CMO có thể liên kết Indicator với SuccessFactor
                    if ((isManager || isCmo) && isInSameDepartment)
                    {
                        context.Succeed(requirement);
                    }
                    break;
            }
        }
    }
}
