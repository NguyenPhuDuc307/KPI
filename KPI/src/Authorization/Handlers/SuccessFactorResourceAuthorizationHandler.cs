using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Handler xử lý ủy quyền cho tài nguyên SuccessFactor
    /// </summary>
    public class SuccessFactorResourceAuthorizationHandler : ResourceBasedAuthorizationHandler<SuccessFactor>
    {
        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public SuccessFactorResourceAuthorizationHandler(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        /// <summary>
        /// Xử lý ủy quyền đặc thù cho SuccessFactor
        /// </summary>
        protected override async Task HandleResourceSpecificAuthorizationAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            SuccessFactor resource,
            ApplicationUser user,
            bool isManager,
            bool isCmo)
        {
            // Manager của phòng ban có thể quản lý SuccessFactor của phòng mình
            bool isInSameDepartment = false;
            if (user.DepartmentId.HasValue && resource.DepartmentId.HasValue)
            {
                isInSameDepartment = user.DepartmentId.Value == resource.DepartmentId.Value;
            }

            // Manager trong cùng phòng ban có tất cả quyền
            if (isManager && isInSameDepartment)
            {
                context.Succeed(requirement);
                return;
            }

            // CMO có quyền đọc và cập nhật SuccessFactor 
            if (isCmo)
            {
                if (requirement.Name == Operations.Read.Name ||
                    requirement.Name == Operations.Update.Name)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // Người dùng bình thường chỉ có thể xem SuccessFactor
            if (requirement.Name == Operations.Read.Name)
            {
                context.Succeed(requirement);
                return;
            }

            await Task.CompletedTask;
        }
    }
}