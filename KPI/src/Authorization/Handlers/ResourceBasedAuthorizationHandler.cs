using System.Security.Claims;
using System.Threading.Tasks;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Xử lý ủy quyền dựa trên tài nguyền và phòng ban
    /// </summary>
    /// <typeparam name="T">Loại entity cần ủy quyền</typeparam>
    public class ResourceBasedAuthorizationHandler<T> : AuthorizationHandler<OperationAuthorizationRequirement, T> where T : BaseEntity
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public ResourceBasedAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Các hoạt động được phép
        /// </summary>
        public static class Operations
        {
            public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
            public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
            public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
            public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
        }

        /// <summary>
        /// Xử lý yêu cầu ủy quyền
        /// </summary>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            T resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            // Kiểm tra người dùng có thuộc vai trò Administrator không
            if (await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Administrator))
            {
                context.Succeed(requirement);
                return;
            }

            // Kiểm tra các vai trò cụ thể khác
            var isManager = await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Manager);
            var isCmo = await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.CMO);

            // Kiểm tra quyền theo loại tài nguyên cụ thể
            await HandleResourceSpecificAuthorizationAsync(context, requirement, resource, user, isManager, isCmo);

            // Xử lý các trường hợp chung
            await HandleCommonAuthorizationAsync(context, requirement, resource, user, isManager, isCmo);
        }

        /// <summary>
        /// Xử lý ủy quyền theo loại tài nguyên cụ thể
        /// </summary>
        protected virtual async Task HandleResourceSpecificAuthorizationAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            T resource,
            ApplicationUser user,
            bool isManager,
            bool isCmo)
        {
            // Lớp cơ sở - lớp kế thừa sẽ ghi đè phương thức này
            await Task.CompletedTask;
        }

        /// <summary>
        /// Xử lý ủy quyền chung cho mọi loại tài nguyên
        /// </summary>
        private async Task HandleCommonAuthorizationAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            T resource,
            ApplicationUser user,
            bool isManager,
            bool isCmo)
        {
            // Kiểm tra quyền dựa trên người tạo tài nguyên
            if (resource.CreatedBy == user.Id)
            {
                if (requirement.Name == Operations.Read.Name)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // Manager có thể xem tất cả tài nguyên
            if (isManager && requirement.Name == Operations.Read.Name)
            {
                context.Succeed(requirement);
                return;
            }

            // CMO có thể xem tất cả tài nguyên
            if (isCmo && requirement.Name == Operations.Read.Name)
            {
                context.Succeed(requirement);
                return;
            }

            await Task.CompletedTask;
        }
    }
}