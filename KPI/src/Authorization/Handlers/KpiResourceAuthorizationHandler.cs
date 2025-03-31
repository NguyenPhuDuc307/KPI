using System.Threading.Tasks;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace KPISolution.Authorization.Handlers
{
    /// <summary>
    /// Handler xử lý ủy quyền cho tài nguyên KPI
    /// </summary>
    public class KpiResourceAuthorizationHandler : ResourceBasedAuthorizationHandler<KpiBase>
    {
        /// <summary>
        /// Khởi tạo handler
        /// </summary>
        /// <param name="userManager">User Manager</param>
        public KpiResourceAuthorizationHandler(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        /// <summary>
        /// Xử lý ủy quyền đặc thù cho KPI
        /// </summary>
        protected override async Task HandleResourceSpecificAuthorizationAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            KpiBase resource,
            ApplicationUser user,
            bool isManager,
            bool isCmo)
        {
            // Người dùng có thuộc phòng ban quản lý KPI không
            bool isInSameDepartment = false;
            if (user.DepartmentId.HasValue && !string.IsNullOrEmpty(resource.Department))
            {
                // So sánh với tên phòng ban thay vì ID
                isInSameDepartment = user.Department?.Name == resource.Department;
            }

            // Kiểm tra người dùng có phải là người sở hữu KPI không
            bool isOwner = user.IsKpiOwner && (resource.ResponsiblePerson == $"{user.FirstName} {user.LastName}");

            // Manager trong cùng phòng ban có tất cả quyền
            if (isManager && isInSameDepartment)
            {
                context.Succeed(requirement);
                return;
            }

            // Người sở hữu KPI có thể đọc và cập nhật
            if (isOwner)
            {
                if (requirement.Name == Operations.Read.Name ||
                    requirement.Name == Operations.Update.Name)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // CMO có thể xem và cập nhật tất cả KPI
            if (isCmo)
            {
                if (requirement.Name == Operations.Read.Name ||
                    requirement.Name == Operations.Update.Name)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            await Task.CompletedTask;
        }
    }
}