using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.ViewModels.Dashboard;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.Dashboard;
using KPISolution.Models.Enums;
using KPISolution.Authorization;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller responsible for managing different types of dashboards including executive, 
    /// department-specific, and custom user dashboards.
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        /// <summary>
        /// Initializes a new instance of the DashboardController.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for data access</param>
        /// <param name="logger">Logger for the DashboardController</param>
        public DashboardController(
            IUnitOfWork unitOfWork,
            ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the executive dashboard
        /// </summary>
        /// <returns>Executive dashboard view</returns>
        [HttpGet]
        [Authorize(Roles = "Executive,Administrator")]
        public async Task<IActionResult> Executive(int departmentPage = 1, int kriPage = 1)
        {
            try
            {
                // Store the pagination parameters for use in the view
                ViewBag.DepartmentPage = departmentPage;
                ViewBag.KriPage = kriPage;

                // Create view model
                var viewModel = new ExecutiveDashboardViewModel
                {
                    Title = "Bảng điều khiển điều hành",
                    LastUpdated = DateTime.Now
                };

                _logger.LogInformation("Loading executive dashboard");

                // Get KRIs and populate KRI summaries
                var kris = await _unitOfWork.KRIs.GetAllAsync();
                viewModel.KriSummaries = kris.Select(k => new KpiSummaryViewModel
                {
                    Id = k.Id,
                    Name = k.Name ?? string.Empty,
                    Code = k.Code ?? string.Empty,
                    TargetValue = k.TargetValue,
                    MeasurementUnit = k.Unit ?? string.Empty,
                    Department = k.Department ?? string.Empty,
                    Status = k.Status
                }).ToList();

                // Get CSFs and populate CSF summaries
                var csfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
                viewModel.CsfSummaries = csfs.Select(c => new CsfSummaryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    ProgressPercentage = c.ProgressPercentage
                }).ToList();

                // Get departments and create performance metrics by department
                var departments = await _unitOfWork.Departments.GetAllAsync();
                viewModel.PerformanceByDepartment = departments.Select(d => new DepartmentPerformanceViewModel
                {
                    DepartmentId = d.Id,
                    Name = d.Name
                }).ToList();

                // Calculate statistics
                viewModel.TotalKpiCount = viewModel.KriSummaries.Count;

                // Count KPIs by status
                foreach (KpiStatus status in Enum.GetValues(typeof(KpiStatus)))
                {
                    viewModel.KpisByStatus[status] = viewModel.KriSummaries.Count(k => k.Status == status);
                }

                // Thêm dữ liệu mẫu cho hiển thị nếu không có KPI nào
                if (viewModel.TotalKpiCount == 0)
                {
                    // Thêm dữ liệu mẫu cho KPI status để hiển thị biểu đồ
                    viewModel.KpisByStatus[KpiStatus.Active] = 12;
                    viewModel.KpisByStatus[KpiStatus.Draft] = 3;
                    viewModel.KpisByStatus[KpiStatus.UnderReview] = 2;
                    viewModel.KpisByStatus[KpiStatus.OnTarget] = 8;
                    viewModel.KpisByStatus[KpiStatus.AtRisk] = 4;
                    viewModel.KpisByStatus[KpiStatus.BelowTarget] = 2;

                    viewModel.TotalKpiCount = viewModel.KpisByStatus.Values.Sum();
                }

                // Thêm dữ liệu mẫu cho hiệu suất theo phòng ban nếu không có dữ liệu
                if (!viewModel.PerformanceByDepartment.Any() || viewModel.PerformanceByDepartment.All(d => d.KpiCount == 0))
                {
                    viewModel.PerformanceByDepartment.Clear();

                    // Thêm các phòng ban mẫu với dữ liệu hiệu suất
                    AddSampleDepartment(viewModel, "IT", 85, 12, 1);
                    AddSampleDepartment(viewModel, "HR", 72, 8, 2);
                    AddSampleDepartment(viewModel, "Finance", 91, 10, 0);
                    AddSampleDepartment(viewModel, "Marketing", 68, 7, 2);
                    AddSampleDepartment(viewModel, "Operations", 79, 15, 3);
                    AddSampleDepartment(viewModel, "Sales", 88, 9, 1);
                    AddSampleDepartment(viewModel, "Legal", 94, 4, 0);
                    AddSampleDepartment(viewModel, "R&D", 82, 6, 1);
                    AddSampleDepartment(viewModel, "Customer Service", 75, 8, 2);
                    AddSampleDepartment(viewModel, "Quality Assurance", 89, 5, 0);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading executive dashboard");
                return View("Error");
            }
        }

        /// <summary>
        /// Helper method to add sample department data
        /// </summary>
        private void AddSampleDepartment(ExecutiveDashboardViewModel viewModel, string name, int performance, int kpiCount, int atRiskCount)
        {
            viewModel.PerformanceByDepartment.Add(new DepartmentPerformanceViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = name,
                PerformancePercentage = performance,
                KpiCount = kpiCount,
                AtRiskCount = atRiskCount,
                PerformanceCssClass = performance > 80 ? "bg-success" : (performance > 60 ? "bg-warning" : "bg-danger")
            });
        }

        /// <summary>
        /// Displays a department-specific dashboard showing KPIs for a particular department.
        /// </summary>
        /// <param name="id">The unique identifier of the department.</param>
        /// <returns>The department dashboard view with department-specific KPI data.</returns>
        [HttpGet]
        public async Task<IActionResult> Department(Guid id)
        {
            try
            {
                _logger.LogInformation("Loading department dashboard for department {DepartmentId}", id);

                // Get the department
                var department = await _unitOfWork.Departments.GetByIdAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department dashboard not found for department {DepartmentId}", id);
                    return NotFound();
                }

                // Check user authorization
                if (!await UserHasAccessToDepartment(User, id))
                {
                    _logger.LogWarning("User {UserId} attempted to access unauthorized department {DepartmentId}",
                        User.Identity?.Name ?? "unknown", id);
                    return Forbid();
                }

                // Create the view model
                var viewModel = new DepartmentDashboardViewModel
                {
                    DepartmentId = department.Id,
                    DepartmentName = department.Name ?? string.Empty,
                    LastUpdated = DateTime.UtcNow
                };

                // Get KPIs for this department
                var allKpis = new List<KpiBase>();
                var kris = await _unitOfWork.KRIs.GetAllAsync(k => k.Department == department.Name);
                var pis = await _unitOfWork.PIs.GetAllAsync(p => p.Department == department.Name);
                var ris = await _unitOfWork.RIs.GetAllAsync(r => r.Department == department.Name);

                allKpis.AddRange(kris);
                allKpis.AddRange(pis);
                allKpis.AddRange(ris);

                // Populate KPI summaries
                viewModel.KpiSummaries = allKpis.Select(k => new KpiSummaryViewModel
                {
                    Id = k.Id,
                    Name = k.Name ?? string.Empty,
                    Code = k.Code ?? string.Empty,
                    TargetValue = k.TargetValue,
                    MeasurementUnit = k.Unit ?? string.Empty,
                    Status = k.Status
                }).ToList();

                // Get CSFs linked to this department
                var csfsByDepartment = await _unitOfWork.CriticalSuccessFactors.GetAllAsync(c => c.Department != null && c.Department.Name == department.Name);
                viewModel.LinkedCsfs = csfsByDepartment.Select(c => new CsfSummaryViewModel
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Code = c.Code ?? string.Empty,
                    ProgressPercentage = c.ProgressPercentage
                }).ToList();

                // Calculate statistics
                viewModel.TotalKpiCount = viewModel.KpiSummaries.Count;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading department dashboard for department {DepartmentId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays a custom dashboard for the current user.
        /// </summary>
        /// <param name="id">The unique identifier of the custom dashboard. If null, displays the user's default dashboard.</param>
        /// <returns>The custom dashboard view with user-specific KPI data.</returns>
        [HttpGet]
        public async Task<IActionResult> Custom(Guid? id)
        {
            try
            {
                var userName = User.Identity?.Name ?? "unknown";
                _logger.LogInformation("Loading custom dashboard {DashboardId} for user {UserId}",
                    id, userName);

                CustomDashboardViewModel viewModel;

                if (!id.HasValue)
                {
                    // Get user's default dashboard
                    var defaultDashboard = await _unitOfWork.CustomDashboards.FirstOrDefaultAsync(
                        d => d.UserId == userName && d.IsDefault);

                    if (defaultDashboard == null)
                    {
                        // Create a new default dashboard if none exists
                        defaultDashboard = new CustomDashboard
                        {
                            Title = "My Dashboard",
                            UserId = userName,
                            UserName = userName,
                            IsDefault = true
                        };
                        await _unitOfWork.CustomDashboards.AddAsync(defaultDashboard);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    viewModel = MapToViewModel(defaultDashboard);
                }
                else
                {
                    // Get specific dashboard
                    var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id.Value);
                    if (dashboard == null)
                    {
                        _logger.LogWarning("Custom dashboard not found {DashboardId}", id);
                        return NotFound();
                    }

                    if (!dashboard.IsShared && dashboard.UserId != userName)
                    {
                        _logger.LogWarning("User {UserId} attempted to access unauthorized dashboard {DashboardId}",
                            userName, id);
                        return Forbid();
                    }

                    viewModel = MapToViewModel(dashboard);
                }

                // Get all KPIs to populate available KPIs
                var allKpis = new List<KpiBase>();
                var kris = await _unitOfWork.KRIs.GetAllAsync();
                var pis = await _unitOfWork.PIs.GetAllAsync();
                var ris = await _unitOfWork.RIs.GetAllAsync();

                allKpis.AddRange(kris);
                allKpis.AddRange(pis);
                allKpis.AddRange(ris);

                // Populate available KPIs
                viewModel.AvailableKpis = allKpis.Select(k => new KpiSummaryViewModel
                {
                    Id = k.Id,
                    Name = k.Name ?? string.Empty,
                    Code = k.Code ?? string.Empty,
                    Department = k.Department ?? string.Empty
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading custom dashboard {DashboardId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the form for creating a new custom dashboard.
        /// </summary>
        /// <returns>The create dashboard view.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var userName = User.Identity?.Name ?? "unknown";
            _logger.LogInformation("Displaying create dashboard form for user {UserId}", userName);
            var viewModel = new CustomDashboardViewModel
            {
                Title = "My Dashboard",
                UserId = userName,
                UserName = userName
            };
            return View(viewModel);
        }

        /// <summary>
        /// Creates a new custom dashboard.
        /// </summary>
        /// <param name="viewModel">The dashboard configuration data.</param>
        /// <returns>Redirects to the newly created dashboard if successful; otherwise returns to the create form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomDashboardViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var userName = User.Identity?.Name ?? "unknown";
                _logger.LogInformation("Creating new dashboard for user {UserId}", userName);

                var dashboard = new CustomDashboard
                {
                    Title = viewModel.Title,
                    UserId = userName,
                    UserName = userName,
                    IsDefault = viewModel.IsDefault,
                    IsShared = viewModel.IsShared,
                    RefreshInterval = viewModel.RefreshInterval,
                    LayoutConfiguration = viewModel.LayoutConfiguration ?? string.Empty
                };

                await _unitOfWork.CustomDashboards.AddAsync(dashboard);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully created dashboard {DashboardId} for user {UserId}",
                    dashboard.Id, userName);

                return RedirectToAction(nameof(Custom), new { id = dashboard.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating custom dashboard for user {UserId}", User.Identity?.Name ?? "unknown");
                ModelState.AddModelError("", "Error creating dashboard. Please try again.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard to edit.</param>
        /// <returns>The edit dashboard view if authorized; otherwise returns a forbidden or not found result.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            _logger.LogInformation("Loading edit form for dashboard {DashboardId}", id);

            var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id);
            if (dashboard == null)
            {
                _logger.LogWarning("Dashboard not found {DashboardId}", id);
                return NotFound();
            }

            var userName = User.Identity?.Name ?? "unknown";
            if (dashboard.UserId != userName)
            {
                _logger.LogWarning("User {UserId} attempted to edit unauthorized dashboard {DashboardId}",
                    userName, id);
                return Forbid();
            }

            var viewModel = MapToViewModel(dashboard);

            // Get all KPIs to populate available KPIs
            var allKpis = new List<KpiBase>();
            var kris = await _unitOfWork.KRIs.GetAllAsync();
            var pis = await _unitOfWork.PIs.GetAllAsync();
            var ris = await _unitOfWork.RIs.GetAllAsync();

            allKpis.AddRange(kris);
            allKpis.AddRange(pis);
            allKpis.AddRange(ris);

            // Populate available KPIs
            viewModel.AvailableKpis = allKpis.Select(k => new KpiSummaryViewModel
            {
                Id = k.Id,
                Name = k.Name ?? string.Empty,
                Code = k.Code ?? string.Empty,
                Department = k.Department ?? string.Empty
            }).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Updates an existing custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard to update.</param>
        /// <param name="viewModel">The updated dashboard configuration data.</param>
        /// <returns>Redirects to the updated dashboard if successful; otherwise returns to the edit form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomDashboardViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    _logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return NotFound();
                }

                var userName = User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    _logger.LogWarning("User {UserId} attempted to update unauthorized dashboard {DashboardId}",
                        userName, id);
                    return Forbid();
                }

                _logger.LogInformation("Updating dashboard {DashboardId}", id);

                // Update dashboard properties
                dashboard.Title = viewModel.Title;
                dashboard.IsDefault = viewModel.IsDefault;
                dashboard.IsShared = viewModel.IsShared;
                dashboard.RefreshInterval = viewModel.RefreshInterval;
                dashboard.LayoutConfiguration = viewModel.LayoutConfiguration ?? string.Empty;
                dashboard.LastModified = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully updated dashboard {DashboardId}", id);
                return RedirectToAction(nameof(Custom), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dashboard {DashboardId}", id);
                ModelState.AddModelError("", "Error updating dashboard. Please try again.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Deletes a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard to delete.</param>
        /// <returns>Redirects to the custom dashboard list if successful.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete dashboard {DashboardId}", id);

                var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    _logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return NotFound();
                }

                var userName = User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    _logger.LogWarning("User {UserId} attempted to delete unauthorized dashboard {DashboardId}",
                        userName, id);
                    return Forbid();
                }

                await _unitOfWork.CustomDashboards.DeleteAsync(dashboard);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted dashboard {DashboardId}", id);
                return RedirectToAction(nameof(Custom));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dashboard {DashboardId}", id);
                return RedirectToAction(nameof(Custom), new { id });
            }
        }

        /// <summary>
        /// Saves the layout configuration for a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard.</param>
        /// <param name="layout">The layout configuration data.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLayout(Guid id, [FromBody] DashboardLayoutViewModel layout)
        {
            try
            {
                _logger.LogInformation("Saving layout for dashboard {DashboardId}", id);

                var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    _logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return NotFound();
                }

                var userName = User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    _logger.LogWarning("User {UserId} attempted to save layout for unauthorized dashboard {DashboardId}",
                        userName, id);
                    return Forbid();
                }

                dashboard.LayoutConfiguration = layout.Configuration;
                dashboard.LastModified = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully saved layout for dashboard {DashboardId}", id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving layout for dashboard {DashboardId}", id);
                return Json(new { success = false, message = "Error saving layout" });
            }
        }

        /// <summary>
        /// Adds a new item to a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard.</param>
        /// <param name="item">The dashboard item to add.</param>
        /// <returns>JSON result containing the new item ID if successful.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] DashboardItemViewModel item)
        {
            try
            {
                _logger.LogInformation("Adding item to dashboard {DashboardId}", id);

                var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    _logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return NotFound();
                }

                var userName = User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    _logger.LogWarning("User {UserId} attempted to add item to unauthorized dashboard {DashboardId}",
                        userName, id);
                    return Forbid();
                }

                var dashboardItem = new DashboardItem
                {
                    DashboardId = id,
                    KpiId = item.KpiId,
                    CsfId = item.CsfId,
                    ChartType = (Models.Enums.ChartType)(int)item.ChartType,
                    Title = item.Title,
                    Width = item.Width,
                    Height = item.Height,
                    X = item.X,
                    Y = item.Y,
                    DataConfiguration = item.DataConfiguration,
                    Order = item.Order,
                    ItemType = (Models.Enums.DashboardItemType)(int)item.ItemType,
                    ShowLegend = item.ShowLegend,
                    TimePeriod = (Models.Enums.TimePeriod)(int)item.TimePeriod
                };

                await _unitOfWork.DashboardItems.AddAsync(dashboardItem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully added item {ItemId} to dashboard {DashboardId}",
                    dashboardItem.Id, id);
                return Json(new { success = true, itemId = dashboardItem.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to dashboard {DashboardId}", id);
                return Json(new { success = false, message = "Error adding item" });
            }
        }

        /// <summary>
        /// Removes an item from a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard.</param>
        /// <param name="itemId">The unique identifier of the item to remove.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
        {
            try
            {
                _logger.LogInformation("Removing item {ItemId} from dashboard {DashboardId}", itemId, id);

                var dashboard = await _unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    _logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return NotFound();
                }

                var userName = User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    _logger.LogWarning("User {UserId} attempted to remove item from unauthorized dashboard {DashboardId}",
                        userName, id);
                    return Forbid();
                }

                var item = await _unitOfWork.DashboardItems.GetByIdAsync(itemId);
                if (item == null || item.DashboardId != id)
                {
                    _logger.LogWarning("Dashboard item not found {ItemId}", itemId);
                    return NotFound();
                }

                await _unitOfWork.DashboardItems.DeleteAsync(item);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully removed item {ItemId} from dashboard {DashboardId}", itemId, id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {ItemId} from dashboard {DashboardId}", itemId, id);
                return Json(new { success = false, message = "Error removing item" });
            }
        }

        /// <summary>
        /// Helper method to check if user has access to a department
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="departmentId">Department ID</param>
        /// <returns>True if user has access, false otherwise</returns>
        private async Task<bool> UserHasAccessToDepartment(System.Security.Claims.ClaimsPrincipal user, Guid departmentId)
        {
            // In a real implementation, this would check the user's roles and assigned departments
            // For now, allow access to administrators and executives, or if user is in the department
            if (user.IsInRole("Administrator") || user.IsInRole("Executive"))
            {
                return true;
            }

            // Implement actual check for user's department assignment
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
            if (department != null)
            {
                // Add logic to check if user belongs to department
                // For now, return true to allow access
            }

            return true; // Temporarily allowing all access
        }

        /// <summary>
        /// Maps a CustomDashboard entity to a CustomDashboardViewModel
        /// </summary>
        private CustomDashboardViewModel MapToViewModel(CustomDashboard dashboard)
        {
            return new CustomDashboardViewModel
            {
                Id = dashboard.Id,
                Title = dashboard.Title ?? string.Empty,
                UserId = dashboard.UserId ?? string.Empty,
                UserName = dashboard.UserName ?? string.Empty,
                LastUpdated = dashboard.LastUpdated,
                LastModified = dashboard.LastModified,
                LayoutConfiguration = dashboard.LayoutConfiguration,
                IsDefault = dashboard.IsDefault,
                IsShared = dashboard.IsShared,
                RefreshInterval = dashboard.RefreshInterval,
                DashboardItems = dashboard.DashboardItems?.Select(item => new DashboardItemViewModel
                {
                    Id = item.Id,
                    KpiId = item.KpiId,
                    CsfId = item.CsfId,
                    ChartType = (Models.ViewModels.Dashboard.ChartType)(int)item.ChartType,
                    Title = item.Title ?? string.Empty,
                    Width = item.Width,
                    Height = item.Height,
                    X = item.X,
                    Y = item.Y,
                    DataConfiguration = item.DataConfiguration,
                    Order = item.Order,
                    ItemType = (Models.ViewModels.Dashboard.DashboardItemType)(int)item.ItemType,
                    ShowLegend = item.ShowLegend,
                    TimePeriod = (Models.ViewModels.Dashboard.TimePeriod)(int)item.TimePeriod
                }).ToList() ?? new List<DashboardItemViewModel>()
            };
        }
    }
}