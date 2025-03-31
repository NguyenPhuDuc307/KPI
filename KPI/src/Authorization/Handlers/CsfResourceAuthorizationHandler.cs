using System.Threading.Tasks;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Handler xử lý ủy quyền cho tài nguyên CSF
    /// </summary>
    public class CsfResourceAuthorizationHandler : ResourceBasedAuthorizationHandler<CriticalSuccessFactor>
    {
        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public CsfResourceAuthorizationHandler(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        /// <summary>
        /// Xử lý ủy quyền đặc thù cho CSF
        /// </summary>
        protected override async Task HandleResourceSpecificAuthorizationAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CriticalSuccessFactor resource,
            ApplicationUser user,
            bool isManager,
            bool isCmo)
        {
            // Manager của phòng ban có thể quản lý CSF của phòng mình
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

            // CMO có quyền đọc và cập nhật CSF 
            if (isCmo)
            {
                if (requirement.Name == Operations.Read.Name ||
                    requirement.Name == Operations.Update.Name)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // Người dùng bình thường chỉ có thể xem CSF
            if (requirement.Name == Operations.Read.Name)
            {
                context.Succeed(requirement);
                return;
            }

            await Task.CompletedTask;
        }
    }
}