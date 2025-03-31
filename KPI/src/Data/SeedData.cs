using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// Add missing entities
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Dashboard;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Authorization;
using KPISolution.Models.Entities.Identity;
using KPISolution.Models.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<KpiRole>>();

            await SeedAllDataAsync(context, userManager, roleManager);
        }

        /// <summary>
        /// Seeds all data in the correct order
        /// </summary>
        private static async Task SeedAllDataAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<KpiRole> roleManager)
        {
            // Seed data in the right order to maintain referential integrity
            await SeedRolesAsync(roleManager);
            var users = await SeedUsersAsync(userManager);
            var departments = await SeedDepartmentsAsync(context, users);
            var businessObjectives = await SeedBusinessObjectivesAsync(context, departments);
            var csfs = await SeedCriticalSuccessFactorsAsync(context, departments, businessObjectives);
            var kpis = await SeedKpisAsync(context, departments, users);
            await SeedCSFKPIRelationshipsAsync(context);
            await SeedKpiValuesAsync(context, kpis);
            await SeedDashboardsAsync(context, users, kpis, csfs, departments);

            // Save all changes
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeds default roles
        /// </summary>
        private static async Task SeedRolesAsync(RoleManager<KpiRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roleNames = {
                KpiAuthorizationPolicies.RoleNames.Administrator,
                KpiAuthorizationPolicies.RoleNames.Manager,
                KpiAuthorizationPolicies.RoleNames.User,
                "Executive",
                "Analyst",
                "DepartmentAdmin"
            };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new KpiRole { Name = roleName, IsSystemRole = true, IsActive = true });
                }
            }
        }

        /// <summary>
        /// Seeds users with various roles
        /// </summary>
        private static async Task<List<ApplicationUser>> SeedUsersAsync(UserManager<ApplicationUser> userManager)
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
                    await userManager.AddToRoleAsync(admin, KpiAuthorizationPolicies.RoleNames.Administrator);
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
                    await userManager.AddToRoleAsync(executive, "Executive");
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
                        await userManager.AddToRoleAsync(managerUser, KpiAuthorizationPolicies.RoleNames.Manager);
                        await userManager.AddToRoleAsync(managerUser, "DepartmentAdmin");
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
                        await userManager.AddToRoleAsync(analystUser, "Analyst");
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
                        await userManager.AddToRoleAsync(user, KpiAuthorizationPolicies.RoleNames.User);
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
        private static async Task<List<Department>> SeedDepartmentsAsync(
            ApplicationDbContext context,
            List<ApplicationUser> users)
        {
            var departments = new List<Department>
            {
                new Department {
                    Name = "Finance",
                    Code = "FIN",
                    Description = "Financial management and reporting",
                    DepartmentHeadId = users.First(u => u.Email == "finance@kpiapp.com").Id
                },
                new Department {
                    Name = "Marketing",
                    Code = "MKT",
                    Description = "Marketing strategy and brand management",
                    DepartmentHeadId = users.First(u => u.Email == "marketing@kpiapp.com").Id
                },
                new Department {
                    Name = "Operations",
                    Code = "OPS",
                    Description = "Operational processes and customer service",
                    DepartmentHeadId = users.First(u => u.Email == "operations@kpiapp.com").Id
                },
                new Department {
                    Name = "HR",
                    Code = "HR",
                    Description = "Human resources and personnel management",
                    DepartmentHeadId = users.First(u => u.Email == "hr@kpiapp.com").Id
                },
                new Department {
                    Name = "IT",
                    Code = "IT",
                    Description = "Information technology and systems support",
                    DepartmentHeadId = users.First(u => u.Email == "it@kpiapp.com").Id
                }
            };

            await context.Departments.AddRangeAsync(departments);
            await context.SaveChangesAsync();

            return departments;
        }

        /// <summary>
        /// Seeds business objectives
        /// </summary>
        private static async Task<List<BusinessObjective>> SeedBusinessObjectivesAsync(
            ApplicationDbContext context,
            List<Department> departments)
        {
            var businessObjectives = new List<BusinessObjective>
            {
                // Finance department objectives
                new BusinessObjective {
                    Name = "Increase Annual Revenue",
                    Description = "Grow annual revenue by 20% through new customer acquisition and existing customer expansion",
                    DepartmentId = departments[0].Id,
                    BusinessPerspective = BusinessPerspective.Financial,
                    Priority = PriorityLevel.High,
                    Status = ObjectiveStatus.InProgress,
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    TargetDate = DateTime.UtcNow.AddDays(275),
                    ProgressPercentage = 35,
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new BusinessObjective {
                    Name = "Reduce Operational Costs",
                    Description = "Reduce operational costs by 10% through process optimization and automation",
                    DepartmentId = departments[0].Id,
                    BusinessPerspective = BusinessPerspective.Financial,
                    Priority = PriorityLevel.Medium,
                    Status = ObjectiveStatus.InProgress,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddDays(320),
                    ProgressPercentage = 25,
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new BusinessObjective {
                    Name = "Improve Cash Flow Management",
                    Description = "Improve cash flow by reducing receivable days by 15%",
                    DepartmentId = departments[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-120),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    Status = ObjectiveStatus.AtRisk,
                    Priority = PriorityLevel.High
                },
                
                // Marketing department objectives
                new BusinessObjective {
                    Name = "Expand Market Share",
                    Description = "Increase market share by 15% in key segments through targeted campaigns",
                    DepartmentId = departments[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    Status = ObjectiveStatus.InProgress,
                    Priority = PriorityLevel.High
                },
                new BusinessObjective {
                    Name = "Launch New Product Line",
                    Description = "Successfully launch new product line with 10,000 units sold in first quarter",
                    DepartmentId = departments[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    Status = ObjectiveStatus.NotStarted,
                    Priority = PriorityLevel.Critical
                },
                new BusinessObjective {
                    Name = "Increase Brand Awareness",
                    Description = "Increase brand recognition by 25% through social media and content marketing",
                    DepartmentId = departments[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    Status = ObjectiveStatus.InProgress,
                    Priority = PriorityLevel.Medium
                },
                
                // Operations department objectives
                new BusinessObjective {
                    Name = "Improve Customer Satisfaction",
                    Description = "Improve customer satisfaction score from 7.5 to 9.0 through service enhancements",
                    DepartmentId = departments[2].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-75),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4),
                    Status = ObjectiveStatus.InProgress,
                    Priority = PriorityLevel.High
                },
                new BusinessObjective {
                    Name = "Streamline Supply Chain",
                    Description = "Reduce supply chain lead time by 20% through process optimization",
                    DepartmentId = departments[2].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-100),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15),
                    Status = ObjectiveStatus.Completed,
                    Priority = PriorityLevel.Medium
                },
                new BusinessObjective {
                    Name = "Implement Lean Manufacturing",
                    Description = "Implement lean manufacturing principles to reduce waste by 15%",
                    DepartmentId = departments[2].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow,
                    Status = ObjectiveStatus.NotStarted,
                    Priority = PriorityLevel.Medium
                },
                
                // HR department objectives
                new BusinessObjective {
                    Name = "Increase Employee Engagement",
                    Description = "Increase employee engagement score from 6.8 to 8.5 through new initiatives",
                    DepartmentId = departments[3].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-50),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    Status = ObjectiveStatus.InProgress,
                    Priority = PriorityLevel.High
                },
                new BusinessObjective {
                    Name = "Reduce Employee Turnover",
                    Description = "Reduce voluntary employee turnover by 5% through retention programs",
                    DepartmentId = departments[3].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-85),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7),
                    Status = ObjectiveStatus.AtRisk,
                    Priority = PriorityLevel.High
                },
                new BusinessObjective {
                    Name = "Implement Training Program",
                    Description = "Launch comprehensive training program with 90% employee participation",
                    DepartmentId = departments[3].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow,
                    Status = ObjectiveStatus.NotStarted,
                    Priority = PriorityLevel.Medium
                },
                
                // IT department objectives
                new BusinessObjective {
                    Name = "Reduce IT Downtime",
                    Description = "Reduce system downtime by 5% through infrastructure improvements",
                    DepartmentId = departments[4].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    Status = ObjectiveStatus.InProgress,
                    Priority = PriorityLevel.Critical
                },
                new BusinessObjective {
                    Name = "Enhance Cybersecurity",
                    Description = "Implement advanced security measures to reduce vulnerability by 30%",
                    DepartmentId = departments[4].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-65),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Status = ObjectiveStatus.InProgress,
                    Priority = PriorityLevel.Critical
                },
                new BusinessObjective {
                    Name = "Migrate to Cloud Infrastructure",
                    Description = "Complete 75% migration of systems to cloud infrastructure",
                    DepartmentId = departments[4].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow,
                    Status = ObjectiveStatus.NotStarted,
                    Priority = PriorityLevel.High
                }
            };

            await context.BusinessObjectives.AddRangeAsync(businessObjectives);
            await context.SaveChangesAsync();

            return businessObjectives;
        }

        /// <summary>
        /// Seeds critical success factors
        /// </summary>
        private static async Task<List<CriticalSuccessFactor>> SeedCriticalSuccessFactorsAsync(
            ApplicationDbContext context,
            List<Department> departments,
            List<BusinessObjective> businessObjectives)
        {
            var objectives = businessObjectives.ToDictionary(
                bo => bo.Name,
                bo => bo);

            var csfs = new List<CriticalSuccessFactor>
            {
                new CriticalSuccessFactor {
                    Name = "New Customer Acquisition",
                    Description = "Increase customer base by 20% through targeted marketing campaigns",
                    Code = "CSF-FIN-001",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Increase Annual Revenue"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 35,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(6),
                    Owner = "Sarah Johnson",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "On track with digital marketing initiatives"
                },
                new CriticalSuccessFactor {
                    Name = "Process Automation",
                    Description = "Automate 50% of manual financial processes",
                    Code = "CSF-FIN-002",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Reduce Operational Costs"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 45,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Michael Chen",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Successfully implemented RPA for accounts payable"
                },
                new CriticalSuccessFactor {
                    Name = "Digital Marketing Campaigns",
                    Description = "Launch 3 major digital marketing campaigns",
                    Code = "CSF-MKT-001",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Expand Market Share"].Id,
                    Status = CSFStatus.AtRisk,
                    Category = CSFCategory.Customer,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 25,
                    StartDate = DateTime.UtcNow.AddDays(-15),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Emily Rodriguez",
                    RiskLevel = RiskLevel.High,
                    Notes = "Budget constraints affecting campaign scope"
                },
                new CriticalSuccessFactor {
                    Name = "Employee Recognition Program",
                    Description = "Implement comprehensive employee recognition system",
                    Code = "CSF-HR-001",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Increase Employee Engagement"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.Medium,
                    ProgressPercentage = 15,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddMonths(4),
                    Owner = "David Wilson",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Platform selection taking longer than expected"
                },
                new CriticalSuccessFactor {
                    Name = "IT Infrastructure Upgrade",
                    Description = "Upgrade core IT infrastructure components",
                    Code = "CSF-IT-001",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Enhance Cybersecurity"].Id,
                    Status = CSFStatus.NotStarted,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 0,
                    StartDate = DateTime.UtcNow.AddDays(15),
                    TargetDate = DateTime.UtcNow.AddMonths(8),
                    Owner = "Alex Thompson",
                    RiskLevel = RiskLevel.High,
                    Notes = "Pending budget approval"
                },
                
                // CSFs for Cost Reduction
                new CriticalSuccessFactor {
                    Name = "Vendor Consolidation",
                    Description = "Reduce number of vendors by 30% through strategic consolidation",
                    Code = "CSF-FIN-003",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Reduce Operational Costs"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.Medium,
                    ProgressPercentage = 60,
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Robert Lee",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Vendor assessment phase completed"
                },
                new CriticalSuccessFactor {
                    Name = "Energy Efficiency",
                    Description = "Reduce energy consumption by 15% across facilities",
                    Code = "CSF-FIN-006",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Reduce Operational Costs"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.Medium,
                    ProgressPercentage = 20,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(6),
                    Owner = "Peter Zhang",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Energy audit in progress"
                },
                
                // CSFs for Cash Flow
                new CriticalSuccessFactor {
                    Name = "Accounts Receivable Optimization",
                    Description = "Reduce average collection period from 45 to 30 days",
                    Code = "CSF-FIN-007",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Improve Cash Flow Management"].Id,
                    Status = CSFStatus.AtRisk,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 30,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Sarah Johnson",
                    RiskLevel = RiskLevel.High,
                    Notes = "Collection process review underway"
                },
                new CriticalSuccessFactor {
                    Name = "Payment Terms Renegotiation",
                    Description = "Extend average payment terms with vendors from 30 to 45 days",
                    Code = "CSF-FIN-008",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Improve Cash Flow Management"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 60,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Michael Chen",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Vendor negotiations progressing well"
                },
                
                // CSFs for Marketing department
                new CriticalSuccessFactor {
                    Name = "Strategic Partnerships",
                    Description = "Establish 5 strategic partnerships in key markets",
                    Code = "CSF-MKT-002",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Expand Market Share"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.Customer,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 40,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddMonths(5),
                    Owner = "Jennifer Park",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Two partnerships finalized, three in negotiation"
                },
                new CriticalSuccessFactor {
                    Name = "Competitive Pricing Strategy",
                    Description = "Adjust pricing to increase market competitiveness while maintaining margins",
                    Code = "CSF-MKT-005",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Expand Market Share"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.Customer,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 25,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Emily Rodriguez",
                    RiskLevel = RiskLevel.High,
                    Notes = "Market analysis taking longer than expected"
                },
                
                // CSFs for New Product Launch
                new CriticalSuccessFactor {
                    Name = "Product Development Timeline",
                    Description = "Complete product development and testing on schedule",
                    Code = "CSF-MKT-006",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Launch New Product Line"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 40,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "David Kim",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Development phase on track"
                },
                new CriticalSuccessFactor {
                    Name = "Pre-Launch Marketing",
                    Description = "Generate 5,000 pre-orders through effective marketing",
                    Code = "CSF-MKT-007",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Launch New Product Line"].Id,
                    Status = CSFStatus.NotStarted,
                    Category = CSFCategory.Customer,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 0,
                    StartDate = DateTime.UtcNow.AddDays(30),
                    TargetDate = DateTime.UtcNow.AddMonths(4),
                    Owner = "Jennifer Park",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Marketing strategy being developed"
                },
                new CriticalSuccessFactor {
                    Name = "Distribution Channel Preparation",
                    Description = "Ensure all distribution channels are ready for product launch",
                    Code = "CSF-MKT-008",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Launch New Product Line"].Id,
                    Status = CSFStatus.NotStarted,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 0,
                    StartDate = DateTime.UtcNow.AddDays(45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Mark Davis",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Channel assessment planned"
                },
                
                // CSFs for Operations department
                new CriticalSuccessFactor {
                    Name = "Service Response Time",
                    Description = "Reduce average service response time from 24 hours to 4 hours",
                    Code = "CSF-OPS-009",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Improve Customer Satisfaction"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.Customer,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 70,
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    TargetDate = DateTime.UtcNow.AddMonths(1),
                    Owner = "Thomas Brown",
                    RiskLevel = RiskLevel.Low,
                    Notes = "New response system showing positive results"
                },
                new CriticalSuccessFactor {
                    Name = "Product Quality Enhancement",
                    Description = "Reduce product defect rate from 5% to 1%",
                    Code = "CSF-OPS-010",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Improve Customer Satisfaction"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 65,
                    StartDate = DateTime.UtcNow.AddDays(-75),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Patricia White",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Quality improvement initiatives on track"
                },
                new CriticalSuccessFactor {
                    Name = "Customer Support Training",
                    Description = "Ensure 100% of support staff complete advanced customer service training",
                    Code = "CSF-OPS-011",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Improve Customer Satisfaction"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 35,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "James Wilson",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Training schedule being revised"
                },
                
                // CSFs for Supply Chain
                new CriticalSuccessFactor {
                    Name = "Supplier Consolidation",
                    Description = "Reduce number of suppliers by 15% for key components",
                    Code = "CSF-OPS-012",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Streamline Supply Chain"].Id,
                    Status = CSFStatus.Completed,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 100,
                    StartDate = DateTime.UtcNow.AddDays(-120),
                    TargetDate = DateTime.UtcNow.AddDays(-15),
                    Owner = "Robert Lee",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Supplier consolidation completed successfully"
                },
                new CriticalSuccessFactor {
                    Name = "Inventory Optimization",
                    Description = "Reduce average inventory levels by 20% while maintaining service levels",
                    Code = "CSF-OPS-013",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Streamline Supply Chain"].Id,
                    Status = CSFStatus.Completed,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 100,
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    TargetDate = DateTime.UtcNow.AddDays(-10),
                    Owner = "Michael Smith",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Inventory optimization targets achieved"
                },
                new CriticalSuccessFactor {
                    Name = "Logistics Route Optimization",
                    Description = "Optimize delivery routes to reduce transportation costs by 10%",
                    Code = "CSF-OPS-014",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Streamline Supply Chain"].Id,
                    Status = CSFStatus.Completed,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.Medium,
                    ProgressPercentage = 100,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddDays(-5),
                    Owner = "Jessica Lee",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Route optimization completed with cost savings achieved"
                },
                
                // CSFs for HR department
                new CriticalSuccessFactor {
                    Name = "Professional Development",
                    Description = "Implement structured career development program",
                    Code = "CSF-HR-002",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Increase Employee Engagement"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 75,
                    StartDate = DateTime.UtcNow.AddDays(-120),
                    TargetDate = DateTime.UtcNow.AddMonths(1),
                    Owner = "Maria Garcia",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Program framework established, pilot phase successful"
                },
                new CriticalSuccessFactor {
                    Name = "Work Environment Enhancement",
                    Description = "Implement flexible work policies and physical environment improvements",
                    Code = "CSF-HR-008",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Increase Employee Engagement"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 55,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Maria Garcia",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Policy updates being finalized"
                },
                
                // CSFs for Reducing Turnover
                new CriticalSuccessFactor {
                    Name = "Competitive Compensation",
                    Description = "Ensure all positions are compensated at or above industry average",
                    Code = "CSF-HR-009",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Reduce Employee Turnover"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 75,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddMonths(1),
                    Owner = "David Wilson",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Salary adjustments in final review"
                },
                new CriticalSuccessFactor {
                    Name = "Career Progression Paths",
                    Description = "Define clear career progression paths for all departments",
                    Code = "CSF-HR-010",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Reduce Employee Turnover"].Id,
                    Status = CSFStatus.AtRisk,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 25,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(4),
                    Owner = "Amanda Brown",
                    RiskLevel = RiskLevel.High,
                    Notes = "Department head input pending"
                },
                new CriticalSuccessFactor {
                    Name = "Exit Interview Analysis",
                    Description = "Analyze exit interview data to identify and address turnover causes",
                    Code = "CSF-HR-011",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Reduce Employee Turnover"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.Medium,
                    ProgressPercentage = 80,
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    TargetDate = DateTime.UtcNow.AddDays(15),
                    Owner = "Lisa Martinez",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Data analysis nearing completion"
                },
                
                // CSFs for IT department
                new CriticalSuccessFactor {
                    Name = "System Reliability Enhancement",
                    Description = "Increase system uptime from 97% to 99.9%",
                    Code = "CSF-IT-009",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Reduce IT Downtime"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 65,
                    StartDate = DateTime.UtcNow.AddDays(-75),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Steve Anderson",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Infrastructure upgrades proceeding as planned"
                },
                new CriticalSuccessFactor {
                    Name = "Disaster Recovery Implementation",
                    Description = "Implement robust disaster recovery with 15-minute recovery time objective",
                    Code = "CSF-IT-010",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Reduce IT Downtime"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 45,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "John Peterson",
                    RiskLevel = RiskLevel.High,
                    Notes = "Testing phase beginning"
                },
                new CriticalSuccessFactor {
                    Name = "Infrastructure Monitoring",
                    Description = "Deploy comprehensive monitoring with automated alerting for 100% of systems",
                    Code = "CSF-IT-011",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Reduce IT Downtime"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 30,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Daniel Kim",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Monitoring system in place"
                },
                
                // CSFs for Cybersecurity
                new CriticalSuccessFactor {
                    Name = "Security Training",
                    Description = "Complete security awareness training for all employees",
                    Code = "CSF-IT-012",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Enhance Cybersecurity"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 25,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Michelle Lee",
                    RiskLevel = RiskLevel.High,
                    Notes = "Training program in progress"
                },
                new CriticalSuccessFactor {
                    Name = "Penetration Testing",
                    Description = "Conduct quarterly penetration testing and remediate all critical findings",
                    Code = "CSF-IT-013",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Enhance Cybersecurity"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 40,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(4),
                    Owner = "Karen Wong",
                    RiskLevel = RiskLevel.High,
                    Notes = "First round of testing completed"
                },
                new CriticalSuccessFactor {
                    Name = "Cost Control Measures",
                    Description = "Implement cost control measures across departments",
                    Code = "CSF-FIN-004",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Reduce Operational Costs"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 30,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Andrew Clark",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Department budget reviews in progress"
                },
                new CriticalSuccessFactor {
                    Name = "Market Research Initiative",
                    Description = "Conduct comprehensive market research for expansion",
                    Code = "CSF-MKT-003",
                    DepartmentId = departments[1].Id,
                    BusinessObjectiveId = objectives["Expand Market Share"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.Customer,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 50,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Rachel Green",
                    RiskLevel = RiskLevel.Low,
                    Notes = "Initial market analysis completed"
                },
                new CriticalSuccessFactor {
                    Name = "Training Effectiveness",
                    Description = "Measure and improve training program effectiveness",
                    Code = "CSF-HR-005",
                    DepartmentId = departments[3].Id,
                    BusinessObjectiveId = objectives["Implement Training Program"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.LearningAndGrowth,
                    Priority = PriorityLevel.Medium,
                    ProgressPercentage = 20,
                    StartDate = DateTime.UtcNow.AddDays(-15),
                    TargetDate = DateTime.UtcNow.AddMonths(4),
                    Owner = "Brian Taylor",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Developing assessment metrics"
                },
                new CriticalSuccessFactor {
                    Name = "System Reliability",
                    Description = "Improve system uptime and reliability",
                    Code = "CSF-IT-006",
                    DepartmentId = departments[4].Id,
                    BusinessObjectiveId = objectives["Reduce IT Downtime"].Id,
                    Status = CSFStatus.InProgress,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.Critical,
                    ProgressPercentage = 60,
                    StartDate = DateTime.UtcNow.AddDays(-60),
                    TargetDate = DateTime.UtcNow.AddMonths(2),
                    Owner = "Kevin Zhang",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Infrastructure upgrades showing positive results"
                },
                new CriticalSuccessFactor {
                    Name = "Financial Planning",
                    Description = "Develop comprehensive financial planning process",
                    Code = "CSF-FIN-005",
                    DepartmentId = departments[0].Id,
                    BusinessObjectiveId = objectives["Increase Annual Revenue"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.Financial,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 35,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    TargetDate = DateTime.UtcNow.AddMonths(4),
                    Owner = "Linda Thompson",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Stakeholder reviews ongoing"
                },
                new CriticalSuccessFactor {
                    Name = "Inventory Management",
                    Description = "Optimize inventory levels and reduce carrying costs",
                    Code = "CSF-OPS-006",
                    DepartmentId = departments[2].Id,
                    BusinessObjectiveId = objectives["Streamline Supply Chain"].Id,
                    Status = CSFStatus.Delayed,
                    Category = CSFCategory.InternalProcess,
                    Priority = PriorityLevel.High,
                    ProgressPercentage = 25,
                    StartDate = DateTime.UtcNow.AddDays(-45),
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Owner = "Michael Smith",
                    RiskLevel = RiskLevel.Medium,
                    Notes = "Software implementation in progress"
                }
            };

            await context.CriticalSuccessFactors.AddRangeAsync(csfs);
            await context.SaveChangesAsync();

            return csfs;
        }

        /// <summary>
        /// Seeds KPIs
        /// </summary>
        private static async Task<List<KpiBase>> SeedKpisAsync(
            ApplicationDbContext context,
            List<Department> departments,
            List<ApplicationUser> users)
        {
            var kpis = new List<KpiBase>
            {
                // KRIs - Key Risk Indicators
                new KRI {
                    Name = "Customer Satisfaction Score",
                    Description = "Overall customer satisfaction rating",
                    Code = "KRI-CS-001",
                    Unit = "Score",
                    Formula = "Average of customer survey responses",
                    TargetValue = 4.5M,
                    MinimumValue = 3.0M,
                    MaximumValue = 5.0M,
                    Weight = 30,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Customer Service",
                    ResponsiblePerson = "Sarah Johnson",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },
                new KRI {
                    Name = "Employee Turnover Rate",
                    Description = "Rate at which employees leave the company",
                    Code = "KRI-HR-002",
                    Unit = "Percentage",
                    Formula = "(Number of departures / Average number of employees) * 100",
                    TargetValue = 5.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 15.0M,
                    Weight = 25,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Human Resources",
                    ResponsiblePerson = "David Wilson",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "System Downtime",
                    Description = "Total hours of critical system unavailability",
                    Code = "KRI-IT-003",
                    Unit = "Hours",
                    Formula = "Sum of downtime hours across critical systems",
                    TargetValue = 2.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 24.0M,
                    Weight = 35,
                    Frequency = MeasurementFrequency.Weekly,
                    Department = "IT",
                    ResponsiblePerson = "Kevin Zhang",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Cash Flow Ratio",
                    Description = "Measures company's liquidity and financial health",
                    Code = "KRI-FIN-004",
                    Unit = "Ratio",
                    Formula = "Operating Cash Flow / Current Liabilities",
                    TargetValue = 1.5M,
                    MinimumValue = 0.8M,
                    MaximumValue = 3.0M,
                    Weight = 40,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Finance",
                    ResponsiblePerson = "Michael Chen",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.AtRisk,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },
                new KRI {
                    Name = "Market Share",
                    Description = "Company's percentage of total market sales",
                    Code = "KRI-MKT-005",
                    Unit = "Percentage",
                    Formula = "(Company Sales / Total Market Sales) * 100",
                    TargetValue = 12.0M,
                    MinimumValue = 5.0M,
                    MaximumValue = 25.0M,
                    Weight = 35,
                    Frequency = MeasurementFrequency.Quarterly,
                    Department = "Marketing",
                    ResponsiblePerson = "Emily Rodriguez",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.OnTarget,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },
                new KRI {
                    Name = "Supply Chain Disruption Rate",
                    Description = "Percentage of supply chain disruptions",
                    Code = "KRI-OPS-006",
                    Unit = "Percentage",
                    Formula = "(Number of disruptions / Total supply chain activities) * 100",
                    TargetValue = 3.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 10.0M,
                    Weight = 30,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Operations",
                    ResponsiblePerson = "Thomas Brown",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.BelowTarget,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Cybersecurity Incident Rate",
                    Description = "Number of cybersecurity incidents per month",
                    Code = "KRI-IT-007",
                    Unit = "Count",
                    Formula = "Total number of detected security incidents",
                    TargetValue = 0.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 5.0M,
                    Weight = 45,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "IT",
                    ResponsiblePerson = "Alex Thompson",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.OnTarget,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Compliance Violation Rate",
                    Description = "Rate of regulatory compliance violations",
                    Code = "KRI-LEG-008",
                    Unit = "Percentage",
                    Formula = "(Number of violations / Total compliance checks) * 100",
                    TargetValue = 0.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 5.0M,
                    Weight = 40,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Legal",
                    ResponsiblePerson = "Jennifer Park",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.BelowTarget,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Product Defect Rate",
                    Description = "Percentage of products with defects",
                    Code = "KRI-QA-009",
                    Unit = "Percentage",
                    Formula = "(Number of defective products / Total products produced) * 100",
                    TargetValue = 1.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 5.0M,
                    Weight = 35,
                    Frequency = MeasurementFrequency.Daily,
                    Department = "Quality Assurance",
                    ResponsiblePerson = "Patricia White",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.OnTarget,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Project Delay Rate",
                    Description = "Percentage of projects behind schedule",
                    Code = "KRI-PM-010",
                    Unit = "Percentage",
                    Formula = "(Number of delayed projects / Total active projects) * 100",
                    TargetValue = 10.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 30.0M,
                    Weight = 30,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Project Management",
                    ResponsiblePerson = "Robert Lee",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.AtRisk,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Customer Complaint Rate",
                    Description = "Number of customer complaints per 1000 transactions",
                    Code = "KRI-CS-011",
                    Unit = "Count",
                    Formula = "(Total complaints / Total transactions) * 1000",
                    TargetValue = 5.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 20.0M,
                    Weight = 30,
                    Frequency = MeasurementFrequency.Weekly,
                    Department = "Customer Service",
                    ResponsiblePerson = "James Wilson",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.OnTarget,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new KRI {
                    Name = "Training Completion Rate",
                    Description = "Percentage of employees who completed required training",
                    Code = "KRI-HR-012",
                    Unit = "Percentage",
                    Formula = "(Employees completed training / Total employees) * 100",
                    TargetValue = 95.0M,
                    MinimumValue = 75.0M,
                    MaximumValue = 100.0M,
                    Weight = 25,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Human Resources",
                    ResponsiblePerson = "Maria Garcia",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.OnTarget,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },

                // Result Indicators (RIs)
                new RI {
                    Name = "Support Ticket Resolution Time",
                    Description = "Average time to resolve customer support tickets",
                    Code = "RI-CS-001",
                    Unit = "Hours",
                    Formula = "Sum of resolution times / Number of tickets",
                    TargetValue = 24.0M,
                    MinimumValue = 0.0M,
                    MaximumValue = 72.0M,
                    Weight = 20,
                    Frequency = MeasurementFrequency.Daily,
                    Department = "Customer Support",
                    ResponsiblePerson = "Mike Wilson",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.LowerIsBetter
                },
                new RI {
                    Name = "Sales Conversion Rate",
                    Description = "Percentage of leads converted to sales",
                    Code = "RI-SALES-002",
                    Unit = "Percentage",
                    Formula = "(Number of sales / Number of leads) * 100",
                    TargetValue = 15.0M,
                    MinimumValue = 5.0M,
                    MaximumValue = 30.0M,
                    Weight = 25,
                    Frequency = MeasurementFrequency.Weekly,
                    Department = "Sales",
                    ResponsiblePerson = "John Adams",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },
                new RI {
                    Name = "Average Order Value",
                    Description = "Average monetary value of customer orders",
                    Code = "RI-SALES-003",
                    Unit = "USD",
                    Formula = "Total revenue / Number of orders",
                    TargetValue = 250.0M,
                    MinimumValue = 100.0M,
                    MaximumValue = 500.0M,
                    Weight = 20,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Sales",
                    ResponsiblePerson = "Jessica Parker",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },

                // Performance Indicators (PIs)
                new PI {
                    Name = "Marketing Campaign ROI",
                    Description = "Return on investment for marketing campaigns",
                    Code = "PI-MKT-001",
                    Unit = "Percentage",
                    Formula = "(Revenue - Cost) / Cost * 100",
                    TargetValue = 150.0M,
                    MinimumValue = 100.0M,
                    MaximumValue = 300.0M,
                    Weight = 25,
                    Frequency = MeasurementFrequency.Monthly,
                    Department = "Marketing",
                    ResponsiblePerson = "Emily Brown",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },
                new PI {
                    Name = "Website Conversion Rate",
                    Description = "Percentage of website visitors who make a purchase",
                    Code = "PI-MKT-002",
                    Unit = "Percentage",
                    Formula = "(Number of purchases / Number of visitors) * 100",
                    TargetValue = 3.0M,
                    MinimumValue = 1.0M,
                    MaximumValue = 10.0M,
                    Weight = 20,
                    Frequency = MeasurementFrequency.Weekly,
                    Department = "Marketing",
                    ResponsiblePerson = "Rachel Murphy",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                },
                new PI {
                    Name = "Employee Productivity",
                    Description = "Average output per employee",
                    Code = "PI-HR-003",
                    Unit = "Units/Hour",
                    Formula = "Total output / Total work hours",
                    TargetValue = 12.0M,
                    MinimumValue = 8.0M,
                    MaximumValue = 20.0M,
                    Weight = 25,
                    Frequency = MeasurementFrequency.Daily,
                    Department = "Operations",
                    ResponsiblePerson = "Mark Taylor",
                    EffectiveDate = DateTime.UtcNow,
                    Status = KpiStatus.Active,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter
                }
            };

            await context.KRIs.AddRangeAsync(kpis.OfType<KRI>());
            await context.RIs.AddRangeAsync(kpis.OfType<RI>());
            await context.PIs.AddRangeAsync(kpis.OfType<PI>());
            await context.SaveChangesAsync();

            return kpis;
        }

        /// <summary>
        /// Seeds CSF-KPI relationships
        /// </summary>
        private static async Task SeedCSFKPIRelationshipsAsync(ApplicationDbContext context)
        {
            if (await context.CSFKPIs.AnyAsync())
            {
                return;
            }

            var csfs = await context.CriticalSuccessFactors.ToListAsync();
            var kris = await context.KRIs.ToListAsync();
            var ris = await context.RIs.ToListAsync();
            var pis = await context.PIs.ToListAsync();

            var csfKpiRelationships = new List<CSFKPI>();
            var random = new Random();

            // Add KRI relationships - instead of all CSFs to all KRIs, create selective relationships
            // Each CSF will be linked to at most 2 KRIs, and each KRI to at most 3 CSFs
            foreach (var csf in csfs)
            {
                // Take a random subset of KRIs for each CSF (at most 2)
                var kriSubset = kris
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(2, kris.Count))
                    .ToList();

                foreach (var kri in kriSubset)
                {
                    csfKpiRelationships.Add(new CSFKPI
                    {
                        CsfId = csf.Id,
                        KpiId = kri.Id,
                        KpiType = KpiType.KeyResultIndicator,
                        RelationshipStrength = RelationshipStrength.Critical,
                        Weight = 100,
                        ImpactLevel = ImpactLevel.High
                    });
                }
            }

            // Add RI relationships - instead of all CSFs to all RIs, create selective relationships
            // Each CSF will be linked to at most 3 RIs, and each RI to at most 2 CSFs
            foreach (var csf in csfs)
            {
                // Take a random subset of RIs for each CSF (at most 3)
                var riSubset = ris
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(3, ris.Count))
                    .ToList();

                foreach (var ri in riSubset)
                {
                    csfKpiRelationships.Add(new CSFKPI
                    {
                        CsfId = csf.Id,
                        KpiId = ri.Id,
                        KpiType = KpiType.ResultIndicator,
                        RelationshipStrength = RelationshipStrength.Moderate,
                        Weight = 70,
                        ImpactLevel = ImpactLevel.Medium
                    });
                }
            }

            // Add PI relationships - instead of all CSFs to all PIs, create selective relationships
            // Each CSF will be linked to at most 2 PIs, and each PI to at most 1 CSF
            foreach (var csf in csfs)
            {
                // Take a random subset of PIs for each CSF (at most 2)
                var piSubset = pis
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(2, pis.Count))
                    .ToList();

                foreach (var pi in piSubset)
                {
                    csfKpiRelationships.Add(new CSFKPI
                    {
                        CsfId = csf.Id,
                        KpiId = pi.Id,
                        KpiType = KpiType.PerformanceIndicator,
                        RelationshipStrength = RelationshipStrength.Weak,
                        Weight = 50,
                        ImpactLevel = ImpactLevel.Low
                    });
                }
            }

            await context.CSFKPIs.AddRangeAsync(csfKpiRelationships);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeds KPI values
        /// </summary>
        private static async Task SeedKpiValuesAsync(
            ApplicationDbContext context,
            List<KpiBase> kpis)
        {
            var random = new Random();
            var startDate = DateTime.UtcNow.AddMonths(-6);

            // Tạo lịch sử đo lường cho mỗi KPI
            foreach (var kpi in kpis)
            {
                var kpiMeasurements = new List<KpiMeasurement>();

                // Tạo 6 tháng dữ liệu đo lường
                for (int i = 0; i < 6; i++)
                {
                    var measurementDate = startDate.AddMonths(i);
                    decimal value;

                    // Tạo giá trị hợp lý dựa trên loại KPI
                    if (kpi.MeasurementDirection == MeasurementDirection.HigherIsBetter)
                    {
                        // Giá trị tăng dần theo thời gian với một chút ngẫu nhiên
                        var progress = (i / 5.0m); // 0 đến 1 dựa trên vị trí trong chuỗi thời gian
                        var baseValue = kpi.MinimumValue + (kpi.TargetValue - kpi.MinimumValue) * progress;
                        var randomFactor = random.Next(-10, 20) / 100.0m; // -10% đến +20%
                        value = baseValue * (1 + randomFactor);
                    }
                    else // LowerIsBetter
                    {
                        // Giá trị giảm dần theo thời gian với một chút ngẫu nhiên
                        var progress = (i / 5.0m); // 0 đến 1 dựa trên vị trí trong chuỗi thời gian
                        var baseValue = kpi.MaximumValue - (kpi.MaximumValue - kpi.TargetValue) * progress;
                        var randomFactor = random.Next(-10, 20) / 100.0m; // -10% đến +20%
                        value = baseValue * (1 + randomFactor);
                    }

                    // Đảm bảo giá trị nằm trong khoảng min-max
                    value = Math.Min(kpi.MaximumValue, Math.Max(kpi.MinimumValue, value));

                    // Làm tròn giá trị để dễ đọc
                    value = Math.Round(value, 2);

                    // Xác định trạng thái đo lường
                    var status = MeasurementStatus.Recorded;
                    if (i == 5) // Đo lường mới nhất
                    {
                        if (kpi.MeasurementDirection == MeasurementDirection.HigherIsBetter)
                        {
                            if (value >= kpi.TargetValue)
                                status = MeasurementStatus.Verified;
                            else if (value >= kpi.TargetValue * 0.85m)
                                status = MeasurementStatus.Pending;
                            else
                                status = MeasurementStatus.NeedsRevision;
                        }
                        else // LowerIsBetter
                        {
                            if (value <= kpi.TargetValue)
                                status = MeasurementStatus.Verified;
                            else if (value <= kpi.TargetValue * 1.15m)
                                status = MeasurementStatus.Pending;
                            else
                                status = MeasurementStatus.NeedsRevision;
                        }
                    }

                    kpiMeasurements.Add(new KpiMeasurement
                    {
                        KpiId = kpi.Id,
                        Value = value,
                        MeasurementDate = measurementDate,
                        Status = status,
                        Notes = $"Measurement for {measurementDate:MMMM yyyy}"
                    });
                }

                await context.KpiMeasurements.AddRangeAsync(kpiMeasurements);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeds dashboards
        /// </summary>
        private static async Task SeedDashboardsAsync(
            ApplicationDbContext context,
            List<ApplicationUser> users,
            List<KpiBase> kpis,
            List<CriticalSuccessFactor> csfs,
            List<Department> departments)
        {
            var dashboards = new List<CustomDashboard>
            {
                new CustomDashboard {
                    Title = "Revenue Dashboard",
                    UserId = users[0].Id,
                    UserName = users[0].UserName ?? "DefaultUser",
                    LastUpdated = DateTime.UtcNow,
                    IsDefault = true,
                    IsShared = true,
                    RefreshInterval = 15
                },
                new CustomDashboard {
                    Title = "Operations Dashboard",
                    UserId = users[2].Id,
                    UserName = users[2].UserName ?? "DefaultUser",
                    LastUpdated = DateTime.UtcNow,
                    IsDefault = true,
                    IsShared = false,
                    RefreshInterval = 30
                },
                new CustomDashboard {
                    Title = "HR Dashboard",
                    UserId = users[3].Id,
                    UserName = users[3].UserName ?? "DefaultUser",
                    LastUpdated = DateTime.UtcNow,
                    IsDefault = true,
                    IsShared = true,
                    RefreshInterval = 60
                }
            };

            await context.CustomDashboards.AddRangeAsync(dashboards);
            await context.SaveChangesAsync();

            // Create dashboard items for each dashboard
            var dashboardItems = new List<DashboardItem>();

            // Add items to Revenue Dashboard
            dashboardItems.Add(new DashboardItem
            {
                DashboardId = dashboards[0].Id,
                KpiId = kpis.Count > 0 ? kpis[0].Id : null,
                Title = "Customer Satisfaction",
                ChartType = Models.Enums.ChartType.LineChart,
                Width = 12,
                Height = 6,
                X = 0,
                Y = 0,
                Order = 1,
                ItemType = Models.Enums.DashboardItemType.Kpi,
                ShowLegend = true,
                DataConfiguration = string.Empty
            });

            dashboardItems.Add(new DashboardItem
            {
                DashboardId = dashboards[0].Id,
                KpiId = kpis.Count > 1 ? kpis[1].Id : null,
                Title = "Support Resolution Time",
                ChartType = Models.Enums.ChartType.BarChart,
                Width = 12,
                Height = 6,
                X = 0,
                Y = 6,
                Order = 2,
                ItemType = Models.Enums.DashboardItemType.Kpi,
                ShowLegend = false,
                DataConfiguration = string.Empty
            });

            // Add items to Operations Dashboard
            dashboardItems.Add(new DashboardItem
            {
                DashboardId = dashboards[1].Id,
                KpiId = kpis.Count > 2 ? kpis[2].Id : null,
                Title = "Marketing ROI",
                ChartType = Models.Enums.ChartType.LineChart,
                Width = 12,
                Height = 6,
                X = 0,
                Y = 0,
                Order = 1,
                ItemType = Models.Enums.DashboardItemType.Kpi,
                ShowLegend = true,
                DataConfiguration = string.Empty
            });

            // Add items to HR Dashboard
            dashboardItems.Add(new DashboardItem
            {
                DashboardId = dashboards[2].Id,
                KpiId = kpis.Count > 0 ? kpis[0].Id : null,
                Title = "Customer Satisfaction",
                ChartType = Models.Enums.ChartType.PieChart,
                Width = 12,
                Height = 6,
                X = 0,
                Y = 0,
                Order = 1,
                ItemType = Models.Enums.DashboardItemType.Kpi,
                ShowLegend = true,
                DataConfiguration = string.Empty
            });

            await context.DashboardItems.AddRangeAsync(dashboardItems);
            await context.SaveChangesAsync();
        }
    }
}

