using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Xử lý ủy quyền cho Indicator
    /// </summary>
    public class IndicatorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IndicatorBase>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public IndicatorAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        /// <summary>
        /// Các hoạt động được phép cho Indicator
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
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IndicatorBase resource)
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

            // Người dùng có thuộc phòng ban quản lý Indicator không
            bool isInSameDepartment = false;
            if (user.DepartmentId.HasValue && resource.DepartmentId.HasValue)
            {
                isInSameDepartment = user.DepartmentId == resource.DepartmentId;
            }

            // Kiểm tra người dùng có phải là người sở hữu Indicator không
            bool isOwner = await this.IsIndicatorOwnerAsync(user, resource);

            // Manager trong cùng phòng ban có tất cả quyền
            if (isManager && isInSameDepartment)
            {
                context.Succeed(requirement);
                return;
            }

            // Logic từ ResourceBasedAuthorizationHandler
            // Kiểm tra quyền dựa trên người tạo tài nguyên
            if (resource.CreatedBy == user.Id && requirement.Name == nameof(Operations.Read))
            {
                context.Succeed(requirement);
                return;
            }

            // Xử lý các quyền cụ thể dựa trên yêu cầu
            switch (requirement.Name)
            {
                case nameof(Operations.Read):
                    // Người dùng là chủ sở hữu/người được giao trách nhiệm cho Indicator
                    // hoặc Manager có thể xem Indicator
                    if (isOwner || isManager || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Create):
                    // Manager và CMO có thể tạo Indicator mới
                    if (isManager || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.Update):
                    // Manager của phòng ban và người sở hữu Indicator có thể cập nhật Indicator
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
                    // Manager và CMO có thể liên kết CSF với Indicator
                    if ((isManager && isInSameDepartment) || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case nameof(Operations.UpdateValue):
                    // Manager, CMO và người sở hữu Indicator có thể cập nhật giá trị Indicator
                    if ((isManager && isInSameDepartment) || isOwner || isCmo)
                    {
                        context.Succeed(requirement);
                    }
                    break;
            }
        }

        /// <summary>
        /// Kiểm tra nếu người dùng là chủ sở hữu Indicator
        /// </summary>
        private async Task<bool> IsIndicatorOwnerAsync(ApplicationUser user, IndicatorBase resource)
        {
            // Kiểm tra nếu người dùng có vai trò IndicatorOwner
            bool hasIndicatorOwnerRole = await this._userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.IndicatorOwner);

            // Kiểm tra nếu người dùng được gán là người chịu trách nhiệm
            bool isResponsible = resource.ResponsibleUserId == user.Id;

            // Kiểm tra nếu tên người dùng khớp với tên chủ sở hữu (để hỗ trợ tương thích ngược)
            bool nameMatches = !string.IsNullOrEmpty(resource.Owner) &&
                              resource.Owner.Contains(user.FirstName) &&
                              resource.Owner.Contains(user.LastName);

            return hasIndicatorOwnerRole && (isResponsible || nameMatches);
        }
    }
}
