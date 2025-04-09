using System.Security.Claims;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace KPISolution.Authorization
{
    /// <summary>
    /// Lớp tiện ích cho việc kiểm tra quyền
    /// </summary>
    public static class AuthorizationUtility
    {
        /// <summary>
        /// Kiểm tra nhanh xem người dùng có quyền thực hiện hành động nào đó trên tài nguyên không
        /// </summary>
        /// <typeparam name="T">Loại tài nguyên</typeparam>
        /// <param name="authService">Dịch vụ kiểm tra quyền</param>
        /// <param name="user">ClaimsPrincipal của người dùng</param>
        /// <param name="resource">Tài nguyên cần kiểm tra</param>
        /// <param name="operation">Hành động cần kiểm tra</param>
        /// <returns>True nếu được phép, ngược lại là False</returns>
        public static async Task<bool> AuthorizeAsync<T>(
            IAuthorizationService authService,
            ClaimsPrincipal user,
            T resource,
            OperationAuthorizationRequirement operation) where T : BaseEntity
        {
            var result = await authService.AuthorizeAsync(user, resource, operation);
            return result.Succeeded;
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải là admin không
        /// </summary>
        /// <param name="userManager">User Manager</param>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>True nếu là admin, ngược lại là False</returns>
        public static async Task<bool> IsAdminAsync(UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.Administrator);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải là manager không
        /// </summary>
        /// <param name="userManager">User Manager</param>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>True nếu là manager, ngược lại là False</returns>
        public static async Task<bool> IsManagerAsync(UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.Manager);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải là CMO không
        /// </summary>
        /// <param name="userManager">User Manager</param>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>True nếu là CMO, ngược lại là False</returns>
        public static async Task<bool> IsCmoAsync(UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.CMO);
        }

        public static async Task<bool> CheckPermissionAsync(
            IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            string policyName,
            object? resource = null)
        {
            AuthorizationResult result;
            if (resource != null)
            {
                result = await authorizationService.AuthorizeAsync(user, resource, policyName);
            }
            else
            {
                result = await authorizationService.AuthorizeAsync(user, policyName);
            }
            return result.Succeeded;
        }

        // Helper methods for common policy checks
        public static Task<bool> CanViewIndicatorAsync(IAuthorizationService authService, ClaimsPrincipal user, object indicatorResource)
        {
            return CheckPermissionAsync(authService, user, IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators, indicatorResource);
        }

        public static Task<bool> CanManageIndicatorAsync(IAuthorizationService authService, ClaimsPrincipal user, object indicatorResource)
        {
            return CheckPermissionAsync(authService, user, IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators, indicatorResource);
        }

        public static Task<bool> CanManageMeasurementsAsync(IAuthorizationService authService, ClaimsPrincipal user)
        {
            return CheckPermissionAsync(authService, user, IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators);
        }
    }
}