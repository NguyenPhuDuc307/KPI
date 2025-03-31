using System.Threading.Tasks;
using KPISolution.Models.Entities.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

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
            System.Security.Claims.ClaimsPrincipal user,
            T resource,
            Microsoft.AspNetCore.Authorization.Infrastructure.OperationAuthorizationRequirement operation) where T : BaseEntity
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
        public static async Task<bool> IsAdminAsync(UserManager<Models.Entities.Identity.ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Administrator);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải là manager không
        /// </summary>
        /// <param name="userManager">User Manager</param>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>True nếu là manager, ngược lại là False</returns>
        public static async Task<bool> IsManagerAsync(UserManager<Models.Entities.Identity.ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Manager);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải là CMO không
        /// </summary>
        /// <param name="userManager">User Manager</param>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>True nếu là CMO, ngược lại là False</returns>
        public static async Task<bool> IsCmoAsync(UserManager<Models.Entities.Identity.ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.CMO);
        }
    }
}