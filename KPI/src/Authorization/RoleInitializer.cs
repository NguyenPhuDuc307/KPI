using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPISolution.Models.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KPISolution.Authorization
{
    /// <summary>
    /// Lớp khởi tạo các vai trò mặc định cho hệ thống
    /// </summary>
    public static class RoleInitializer
    {
        /// <summary>
        /// Khởi tạo các vai trò mặc định cho hệ thống
        /// </summary>
        /// <param name="serviceProvider">DI ServiceProvider</param>
        /// <returns>Task</returns>
        public static async Task InitializeRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationUser>>();

            try
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<KpiRole>>();

                // Danh sách các vai trò cần tạo
                var roles = new List<string>
                {
                    KpiAuthorizationPolicies.RoleNames.Administrator,
                    KpiAuthorizationPolicies.RoleNames.Manager,
                    KpiAuthorizationPolicies.RoleNames.User,
                    KpiAuthorizationPolicies.RoleNames.CMO
                };

                // Tạo các vai trò nếu chưa tồn tại
                foreach (var roleName in roles)
                {
                    var roleExists = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        // Tạo vai trò mới
                        var role = new KpiRole { Name = roleName, IsSystemRole = true, IsActive = true };
                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            logger.LogInformation("Đã tạo vai trò {RoleName}", roleName);
                        }
                        else
                        {
                            logger.LogError("Lỗi khi tạo vai trò {RoleName}: {Errors}",
                                roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                }

                logger.LogInformation("Khởi tạo vai trò hoàn tất");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi khởi tạo vai trò");
            }
        }

        /// <summary>
        /// Khởi tạo người dùng quản trị mặc định
        /// </summary>
        /// <param name="serviceProvider">DI ServiceProvider</param>
        /// <param name="adminEmail">Email của tài khoản admin</param>
        /// <param name="adminPassword">Mật khẩu của tài khoản admin</param>
        /// <returns>Task</returns>
        public static async Task EnsureAdminUserAsync(IServiceProvider serviceProvider, string adminEmail, string adminPassword)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationUser>>();

            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Kiểm tra xem tài khoản admin đã tồn tại chưa
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    // Tạo tài khoản admin mới
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "System",
                        LastName = "Administrator",
                        IsActive = true,
                        JobTitle = "System Administrator",
                        HierarchyLevel = 0,
                        IsDepartmentAdmin = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        logger.LogInformation("Đã tạo tài khoản admin {Email}", adminEmail);

                        // Gán vai trò admin
                        var roleResult = await userManager.AddToRoleAsync(adminUser, KpiAuthorizationPolicies.RoleNames.Administrator);

                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation("Đã gán vai trò admin cho {Email}", adminEmail);
                        }
                        else
                        {
                            logger.LogError("Lỗi khi gán vai trò admin cho {Email}: {Errors}",
                                adminEmail, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogError("Lỗi khi tạo tài khoản admin {Email}: {Errors}",
                            adminEmail, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    // Đảm bảo người dùng admin có vai trò admin
                    if (!await userManager.IsInRoleAsync(adminUser, KpiAuthorizationPolicies.RoleNames.Administrator))
                    {
                        var roleResult = await userManager.AddToRoleAsync(adminUser, KpiAuthorizationPolicies.RoleNames.Administrator);

                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation("Đã gán vai trò admin cho người dùng hiện tại {Email}", adminEmail);
                        }
                        else
                        {
                            logger.LogError("Lỗi khi gán vai trò admin cho người dùng hiện tại {Email}: {Errors}",
                                adminEmail, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi khởi tạo tài khoản admin");
            }
        }
    }
}