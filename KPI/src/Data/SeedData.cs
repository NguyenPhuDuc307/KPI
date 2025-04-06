using System.Diagnostics;

namespace KPISolution.Data
{
    /// <summary>
    /// Handles seeding initial data for the application
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Initializes the database with seed data
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IndicatorRole>>();

            await SeedAsync(userManager, roleManager, context);
        }

        /// <summary>
        /// Seeds all data in the correct order
        /// </summary>
        private static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IndicatorRole> roleManager, ApplicationDbContext context)
        {
            try
            {
                // Seed roles first
                await SeedRolesAsync(roleManager);
                await context.SaveChangesAsync();

                // Seed users and assign roles
                await SeedUsersAsync(userManager, roleManager);
                await context.SaveChangesAsync();

                // Seed departments - must be before seeding business objectives
                await SeedDepartmentsAsync(context);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                Debug.WriteLine($"Error in SeedAsync: {ex}");
            }
        }

        /// <summary>
        /// Seeds default roles
        /// </summary>
        private static async Task SeedRolesAsync(RoleManager<IndicatorRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roleNames = ["Administrator", "Manager", "User", "Guest", "CMO", "IndicatorOwner", "DepartmentManager"
            ];
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var description = roleName switch
                    {
                        "Administrator" => "Quản trị viên hệ thống, có toàn quyền.",
                        "Manager" => "Quản lý, có quyền quản lý chỉ số và người dùng trong phòng ban.",
                        "User" => "Người dùng thông thường, có thể xem và cập nhật chỉ số được giao.",
                        "Guest" => "Khách, chỉ có quyền xem hạn chế.",
                        "CMO" => "Giám đốc Marketing, có quyền xem báo cáo và dashboard tổng thể.",
                        "IndicatorOwner" => "Người sở hữu chỉ số, chịu trách nhiệm về chỉ số cụ thể.",
                        "DepartmentManager" => "Trưởng phòng, quản lý các hoạt động trong phòng ban.",
                        _ => $"Vai trò {roleName}"
                    };
                    // Tạo IndicatorRole thay vì KpiRole
                    var role = new IndicatorRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        Description = description,
                        IsSystemRole = (roleName == "Administrator" || roleName == "Manager" || roleName == "User" || roleName == "Guest"),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "SystemSeed",
                        IsActive = true,
                        PermissionLevel = roleName switch
                        {
                            "Administrator" => (int)PermissionLevel.Administrator,
                            "Manager" => (int)PermissionLevel.Manager,
                            "DepartmentManager" => (int)PermissionLevel.Manager,
                            "CMO" => (int)PermissionLevel.Manager, // Hoặc cấp độ phù hợp khác
                            "IndicatorOwner" => (int)PermissionLevel.Editor,
                            "User" => (int)PermissionLevel.Contributor,
                            "Guest" => (int)PermissionLevel.Viewer,
                            _ => (int)PermissionLevel.Viewer
                        },
                        Permissions = roleName switch
                        {
                            "Administrator" => KPISolution.Models.Entities.Identity.IndicatorPermission.Admin,
                            "Manager" => KPISolution.Models.Entities.Identity.IndicatorPermission.View | KPISolution.Models.Entities.Identity.IndicatorPermission.Create | KPISolution.Models.Entities.Identity.IndicatorPermission.Edit | KPISolution.Models.Entities.Identity.IndicatorPermission.Assign | KPISolution.Models.Entities.Identity.IndicatorPermission.ViewAll,
                            "DepartmentManager" => KPISolution.Models.Entities.Identity.IndicatorPermission.View | KPISolution.Models.Entities.Identity.IndicatorPermission.Create | KPISolution.Models.Entities.Identity.IndicatorPermission.Edit | KPISolution.Models.Entities.Identity.IndicatorPermission.Assign | KPISolution.Models.Entities.Identity.IndicatorPermission.ViewAll,
                            "CMO" => KPISolution.Models.Entities.Identity.IndicatorPermission.View | KPISolution.Models.Entities.Identity.IndicatorPermission.ViewAll | KPISolution.Models.Entities.Identity.IndicatorPermission.Export, // Quyền xem và xuất dữ liệu
                            "IndicatorOwner" => KPISolution.Models.Entities.Identity.IndicatorPermission.View | KPISolution.Models.Entities.Identity.IndicatorPermission.Edit, // Quyền xem và sửa chỉ số mình sở hữu
                            "User" => KPISolution.Models.Entities.Identity.IndicatorPermission.View, // Chỉ quyền xem
                            "Guest" => KPISolution.Models.Entities.Identity.IndicatorPermission.None,
                            _ => KPISolution.Models.Entities.Identity.IndicatorPermission.None
                        }
                    };
                    await roleManager.CreateAsync(role);
                }
            }
        }

        /// <summary>
        /// Seeds users with various roles
        /// </summary>
        private static async Task<List<ApplicationUser>> SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IndicatorRole> roleManager)
        {
            var users = new List<ApplicationUser>();

            // Create admin user if it doesn't exist
            var adminEmail = "admin@kpiapp.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    JobTitle = "System Administrator",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsKpiOwner = true,
                    IsDepartmentAdmin = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                    users.Add(admin);
                }
            }
            else
            {
                users.Add(admin);
            }

            // Create executive user
            var executiveEmail = "ceo@kpiapp.com";
            var executive = await userManager.FindByEmailAsync(executiveEmail);

            if (executive == null)
            {
                executive = new ApplicationUser
                {
                    UserName = executiveEmail,
                    Email = executiveEmail,
                    EmailConfirmed = true,
                    FirstName = "Jane",
                    LastName = "Smith",
                    JobTitle = "Chief Executive Officer",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsKpiOwner = true,
                    IsDepartmentAdmin = false
                };

                var result = await userManager.CreateAsync(executive, "Exec@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(executive, "Manager");
                    users.Add(executive);
                }
            }
            else
            {
                users.Add(executive);
            }

            // Create department managers
            var departmentManagers = new List<(string email, string firstName, string lastName, string jobTitle)>
            {
                ("finance@kpiapp.com", "Michael", "Johnson", "Finance Director"),
                ("marketing@kpiapp.com", "Sarah", "Williams", "Marketing Director"),
                ("operations@kpiapp.com", "Robert", "Brown", "Operations Director"),
                ("hr@kpiapp.com", "Emily", "Davis", "HR Director"),
                ("it@kpiapp.com", "David", "Miller", "IT Director")
            };

            foreach (var manager in departmentManagers)
            {
                var managerUser = await userManager.FindByEmailAsync(manager.email);

                if (managerUser == null)
                {
                    managerUser = new ApplicationUser
                    {
                        UserName = manager.email,
                        Email = manager.email,
                        EmailConfirmed = true,
                        FirstName = manager.firstName,
                        LastName = manager.lastName,
                        JobTitle = manager.jobTitle,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsKpiOwner = true,
                        IsDepartmentAdmin = true
                    };

                    var result = await userManager.CreateAsync(managerUser, "Manager@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(managerUser, "Manager");
                        await userManager.AddToRoleAsync(managerUser, "DepartmentManager");
                        users.Add(managerUser);
                    }
                }
                else
                {
                    users.Add(managerUser);
                }
            }

            // Create analysts
            var analysts = new List<(string email, string firstName, string lastName, string jobTitle)>
            {
                ("analyst1@kpiapp.com", "Alex", "Turner", "Data Analyst"),
                ("analyst2@kpiapp.com", "Nina", "Garcia", "Business Analyst"),
                ("analyst3@kpiapp.com", "Thomas", "Wilson", "Performance Analyst")
            };

            foreach (var analyst in analysts)
            {
                var analystUser = await userManager.FindByEmailAsync(analyst.email);

                if (analystUser == null)
                {
                    analystUser = new ApplicationUser
                    {
                        UserName = analyst.email,
                        Email = analyst.email,
                        EmailConfirmed = true,
                        FirstName = analyst.firstName,
                        LastName = analyst.lastName,
                        JobTitle = analyst.jobTitle,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsKpiOwner = false,
                        IsDepartmentAdmin = false
                    };

                    var result = await userManager.CreateAsync(analystUser, "Analyst@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(analystUser, "User");
                        users.Add(analystUser);
                    }
                }
                else
                {
                    users.Add(analystUser);
                }
            }

            // Create regular users
            var regularUsers = new List<(string email, string firstName, string lastName, string jobTitle)>
            {
                ("user1@kpiapp.com", "Jessica", "Parker", "Marketing Specialist"),
                ("user2@kpiapp.com", "John", "Adams", "Sales Representative"),
                ("user3@kpiapp.com", "Lisa", "Chen", "HR Specialist"),
                ("user4@kpiapp.com", "Mark", "Taylor", "IT Support Specialist"),
                ("user5@kpiapp.com", "Amanda", "Lopez", "Financial Analyst"),
                ("user6@kpiapp.com", "Kevin", "Wilson", "Customer Support Specialist"),
                ("user7@kpiapp.com", "Rachel", "Murphy", "Operations Coordinator"),
                ("user8@kpiapp.com", "Daniel", "Martin", "Product Manager")
            };

            foreach (var regularUser in regularUsers)
            {
                var user = await userManager.FindByEmailAsync(regularUser.email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = regularUser.email,
                        Email = regularUser.email,
                        EmailConfirmed = true,
                        FirstName = regularUser.firstName,
                        LastName = regularUser.lastName,
                        JobTitle = regularUser.jobTitle,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsKpiOwner = false,
                        IsDepartmentAdmin = false
                    };

                    var result = await userManager.CreateAsync(user, "User@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        users.Add(user);
                    }
                }
                else
                {
                    users.Add(user);
                }
            }

            return users;
        }

        /// <summary>
        /// Seeds departments
        /// </summary>
        private static async Task<List<Department>> SeedDepartmentsAsync(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có dữ liệu phòng ban chưa
            if (await context.Departments.AnyAsync())
            {
                return await context.Departments.ToListAsync();
            }

            var departments = new List<Department>
            {
                // Cấp lãnh đạo
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Ban Lãnh Đạo",
                    Code = "EXEC",
                    Description = "Ban lãnh đạo điều hành công ty",
                    Location = "Tầng 10, Tòa nhà Trung tâm",
                    Email = "leadership@company.com",
                    PhoneNumber = "0123456789",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                // Các phòng ban chính
                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Tài Chính Kế Toán",
                    Code = "FINANCE",
                    Description = "Quản lý tài chính và kế toán của công ty",
                    Location = "Tầng 9, Tòa nhà Trung tâm",
                    Email = "finance@company.com",
                    PhoneNumber = "0123456788",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Nhân Sự",
                    Code = "HR",
                    Description = "Quản lý nhân sự và phát triển nguồn nhân lực",
                    Location = "Tầng 8, Tòa nhà Trung tâm",
                    Email = "hr@company.com",
                    PhoneNumber = "0123456787",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Công Nghệ Thông Tin",
                    Code = "IT",
                    Description = "Quản lý hệ thống công nghệ thông tin và hạ tầng kỹ thuật",
                    Location = "Tầng 7, Tòa nhà Trung tâm",
                    Email = "it@company.com",
                    PhoneNumber = "0123456786",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Marketing",
                    Code = "MARKETING",
                    Description = "Quản lý hoạt động tiếp thị và xây dựng thương hiệu",
                    Location = "Tầng 6, Tòa nhà Trung tâm",
                    Email = "marketing@company.com",
                    PhoneNumber = "0123456785",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Kinh Doanh",
                    Code = "SALES",
                    Description = "Quản lý hoạt động bán hàng và phát triển kinh doanh",
                    Location = "Tầng 5, Tòa nhà Trung tâm",
                    Email = "sales@company.com",
                    PhoneNumber = "0123456784",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Sản Xuất",
                    Code = "PRODUCTION",
                    Description = "Quản lý quy trình sản xuất và vận hành nhà máy",
                    Location = "Khu Công Nghiệp A",
                    Email = "production@company.com",
                    PhoneNumber = "0123456783",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng R&D",
                    Code = "RND",
                    Description = "Nghiên cứu và phát triển sản phẩm mới",
                    Location = "Tầng 4, Tòa nhà Trung tâm",
                    Email = "rnd@company.com",
                    PhoneNumber = "0123456782",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                },

                new Department
                {
                    Id = Guid.NewGuid(),
                    Name = "Phòng Chất Lượng",
                    Code = "QA",
                    Description = "Đảm bảo chất lượng sản phẩm và dịch vụ",
                    Location = "Khu Công Nghiệp A",
                    Email = "qa@company.com",
                    PhoneNumber = "0123456781",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System"
                }
            };

            // Thiết lập mối quan hệ cấp bậc giữa các phòng ban
            var execDept = departments.First(d => d.Code == "EXEC");

            // Thêm ParentDepartmentId cho các phòng ban trực thuộc Ban Lãnh Đạo
            foreach (var dept in departments.Where(d => d.Code != "EXEC"))
            {
                dept.ParentDepartmentId = execDept.Id;
            }

            await context.Departments.AddRangeAsync(departments);
            await context.SaveChangesAsync();

            return departments;
        }
    }
}
