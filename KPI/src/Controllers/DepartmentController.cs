using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.ViewModels.Department;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;
using KPISolution.Authorization;
using Microsoft.AspNetCore.Identity;
using KPISolution.Models.Entities.Identity;
using KPISolution.Extensions;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller responsible for managing departments within the organization
    /// </summary>
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DepartmentController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the DepartmentController
        /// </summary>
        /// <param name="unitOfWork">Unit of work for data access</param>
        /// <param name="logger">Logger for the DepartmentController</param>
        /// <param name="userManager">User manager for user operations</param>
        public DepartmentController(
            IUnitOfWork unitOfWork,
            ILogger<DepartmentController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Displays a list of all departments
        /// </summary>
        /// <returns>View with list of departments</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Fetching all departments");

                var departments = await _unitOfWork.Departments.GetAllAsync();
                var viewModels = new List<DepartmentViewModel>();

                // Tạo dữ liệu mẫu nếu không có phòng ban nào
                if (!departments.Any())
                {
                    await CreateSampleDepartmentsAsync();
                    departments = await _unitOfWork.Departments.GetAllAsync();
                }

                // Group by Code to remove duplicates (ensuring only one department per unique code)
                var uniqueDepartments = departments
                    .GroupBy(d => d.Code)
                    .Select(g => g.First())
                    .ToList();

                foreach (var dept in uniqueDepartments)
                {
                    var model = new DepartmentViewModel
                    {
                        Id = dept.Id,
                        Name = dept.Name,
                        Code = dept.Code,
                        Description = dept.Description ?? string.Empty,
                        ParentDepartmentId = dept.ParentDepartmentId,
                        Status = dept.IsActive ? "Active" : "Inactive",
                        StatusCssClass = dept.IsActive ? "badge-success" : "badge-danger",
                        CreatedAt = dept.CreatedAt,
                        LastUpdated = dept.UpdatedAt
                    };

                    // Get parent department name if exists
                    if (dept.ParentDepartmentId.HasValue)
                    {
                        var parentDept = await _unitOfWork.Departments.GetByIdAsync(dept.ParentDepartmentId.Value);
                        if (parentDept != null)
                        {
                            model.ParentDepartmentName = parentDept.Name;
                        }
                    }

                    // Get manager name if exists
                    if (!string.IsNullOrEmpty(dept.DepartmentHeadId))
                    {
                        var manager = await _userManager.FindByIdAsync(dept.DepartmentHeadId);
                        if (manager != null)
                        {
                            model.ManagerName = $"{manager.FirstName} {manager.LastName}";
                            model.ManagerId = manager.Id;
                        }
                    }

                    // Count employees
                    var employees = (await _userManager.GetUsersInRoleAsync("Employee"))
                        .Where(u => u.DepartmentId == dept.Id).ToList();
                    model.EmployeeCount = employees.Count;

                    // Count KPIs
                    var krisWithDept = await _unitOfWork.KRIs.GetAllAsync(k => k.Department == dept.Name);
                    var pisWithDept = await _unitOfWork.PIs.GetAllAsync(k => k.Department == dept.Name);
                    var risWithDept = await _unitOfWork.RIs.GetAllAsync(k => k.Department == dept.Name);
                    model.KpiCount = krisWithDept.Count() + pisWithDept.Count() + risWithDept.Count();

                    viewModels.Add(model);
                }

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching departments");
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the form to create a new department
        /// </summary>
        /// <returns>View with the create form</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            try
            {
                _logger.LogInformation("Displaying create department form");

                // Get departments for dropdown
                var departments = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.ParentDepartments = new SelectList(departments, "Id", "Name");

                // Get users for manager dropdown
                var users = await _userManager.GetUsersInRoleAsync("Manager");
                ViewBag.Users = new SelectList(users, "Id", "FullName");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing create department form");
                return View("Error");
            }
        }

        /// <summary>
        /// Processes the create department form submission
        /// </summary>
        /// <param name="model">The department data from the form</param>
        /// <returns>Redirects to Index on success, returns the form with errors otherwise</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Creating new department: {Name}", model.Name);

                    // Check if a department with the same code already exists
                    var existingDept = await _unitOfWork.Departments.FirstOrDefaultAsync(d => d.Code == model.Code);
                    if (existingDept != null)
                    {
                        ModelState.AddModelError("Code", "A department with this code already exists");

                        // Repopulate dropdowns
                        var departments = await _unitOfWork.Departments.GetAllAsync();
                        ViewBag.ParentDepartments = new SelectList(departments, "Id", "Name");

                        var users = await _userManager.GetUsersInRoleAsync("Manager");
                        ViewBag.Users = new SelectList(users, "Id", "FullName");

                        return View(model);
                    }

                    // Create new department entity
                    var department = new Department
                    {
                        Name = model.Name,
                        Code = model.Code,
                        Description = model.Description,
                        ParentDepartmentId = model.ParentDepartmentId,
                        DepartmentHeadId = model.ManagerId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.Identity?.Name ?? "system",
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = User.Identity?.Name ?? "system"
                    };

                    // Calculate hierarchy level
                    if (model.ParentDepartmentId.HasValue)
                    {
                        var parentDept = await _unitOfWork.Departments.GetByIdAsync(model.ParentDepartmentId.Value);
                        if (parentDept != null)
                        {
                            department.HierarchyLevel = parentDept.HierarchyLevel + 1;
                        }
                    }

                    await _unitOfWork.Departments.AddAsync(department);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Department created successfully: {Id}", department.Id);

                    return RedirectToAction(nameof(Index));
                }

                // If ModelState is invalid, repopulate dropdowns
                var depts = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.ParentDepartments = new SelectList(depts, "Id", "Name");

                var allUsers = await _userManager.GetUsersInRoleAsync("Manager");
                ViewBag.Users = new SelectList(allUsers, "Id", "FullName");

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the details of a specific department
        /// </summary>
        /// <param name="id">The ID of the department</param>
        /// <returns>View with the department details</returns>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching details for department {Id}", id);

                var department = await _unitOfWork.Departments.GetByIdAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department not found: {Id}", id);
                    return NotFound();
                }

                var viewModel = new DepartmentViewModel
                {
                    Id = department.Id,
                    Name = department.Name,
                    Code = department.Code,
                    Description = department.Description ?? string.Empty,
                    ParentDepartmentId = department.ParentDepartmentId,
                    Status = department.IsActive ? "Active" : "Inactive",
                    StatusCssClass = department.IsActive ? "badge-success" : "badge-danger",
                    CreatedAt = department.CreatedAt,
                    LastUpdated = department.UpdatedAt
                };

                // Get parent department name if exists
                if (department.ParentDepartmentId.HasValue)
                {
                    var parentDept = await _unitOfWork.Departments.GetByIdAsync(department.ParentDepartmentId.Value);
                    if (parentDept != null)
                    {
                        viewModel.ParentDepartmentName = parentDept.Name;
                    }
                }

                // Get manager name if exists
                if (!string.IsNullOrEmpty(department.DepartmentHeadId))
                {
                    var manager = await _userManager.FindByIdAsync(department.DepartmentHeadId);
                    if (manager != null)
                    {
                        viewModel.ManagerName = $"{manager.FirstName} {manager.LastName}";
                        viewModel.ManagerId = manager.Id;
                    }
                }

                // Count employees
                var employees = (await _userManager.GetUsersInRoleAsync("Employee"))
                    .Where(u => u.DepartmentId == department.Id).ToList();
                viewModel.EmployeeCount = employees.Count;

                // Count KPIs
                var krisWithDept = await _unitOfWork.KRIs.GetAllAsync(k => k.Department == department.Name);
                var pisWithDept = await _unitOfWork.PIs.GetAllAsync(k => k.Department == department.Name);
                var risWithDept = await _unitOfWork.RIs.GetAllAsync(k => k.Department == department.Name);
                viewModel.KpiCount = krisWithDept.Count() + pisWithDept.Count() + risWithDept.Count();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department details for {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the form to edit an existing department
        /// </summary>
        /// <param name="id">The ID of the department to edit</param>
        /// <returns>View with the edit form</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching department {Id} for editing", id);

                var department = await _unitOfWork.Departments.GetByIdAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department not found: {Id}", id);
                    return NotFound();
                }

                var viewModel = new DepartmentViewModel
                {
                    Id = department.Id,
                    Name = department.Name,
                    Code = department.Code,
                    Description = department.Description ?? string.Empty,
                    ParentDepartmentId = department.ParentDepartmentId,
                    ManagerId = department.DepartmentHeadId ?? string.Empty
                };

                // Get departments for dropdown
                var departments = await _unitOfWork.Departments.GetAllAsync();
                var departmentsExceptCurrent = departments.Where(d => d.Id != id).ToList();
                ViewBag.ParentDepartments = new SelectList(departmentsExceptCurrent, "Id", "Name");

                // Get users for manager dropdown
                var users = await _userManager.GetUsersInRoleAsync("Manager");
                ViewBag.Users = new SelectList(users, "Id", "FullName");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing edit form for department {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Processes the edit department form submission
        /// </summary>
        /// <param name="id">The ID of the department to edit</param>
        /// <param name="model">The updated department data from the form</param>
        /// <returns>Redirects to Index on success, returns the form with errors otherwise</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DepartmentViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    _logger.LogWarning("ID mismatch in Edit: {FormId} vs {RouteId}", model.Id, id);
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Updating department: {Id}", id);

                    var department = await _unitOfWork.Departments.GetByIdAsync(id);
                    if (department == null)
                    {
                        _logger.LogWarning("Department not found: {Id}", id);
                        return NotFound();
                    }

                    // Check if a department with the same code already exists (excluding current)
                    var existingDept = await _unitOfWork.Departments.FirstOrDefaultAsync(d => d.Code == model.Code && d.Id != id);
                    if (existingDept != null)
                    {
                        ModelState.AddModelError("Code", "A department with this code already exists");

                        // Repopulate dropdowns
                        var departments = await _unitOfWork.Departments.GetAllAsync();
                        var departmentsExceptCurrent = departments.Where(d => d.Id != id).ToList();
                        ViewBag.ParentDepartments = new SelectList(departmentsExceptCurrent, "Id", "Name");

                        var users = await _userManager.GetUsersInRoleAsync("Manager");
                        ViewBag.Users = new SelectList(users, "Id", "FullName");

                        return View(model);
                    }

                    // Update department properties
                    department.Name = model.Name;
                    department.Code = model.Code;
                    department.Description = model.Description;
                    department.ParentDepartmentId = model.ParentDepartmentId;
                    department.DepartmentHeadId = model.ManagerId;
                    department.UpdatedAt = DateTime.UtcNow;
                    department.UpdatedBy = User.Identity?.Name ?? "system";

                    // Calculate hierarchy level
                    if (model.ParentDepartmentId.HasValue)
                    {
                        var parentDept = await _unitOfWork.Departments.GetByIdAsync(model.ParentDepartmentId.Value);
                        if (parentDept != null)
                        {
                            department.HierarchyLevel = parentDept.HierarchyLevel + 1;
                        }
                    }
                    else
                    {
                        department.HierarchyLevel = 0;
                    }

                    _unitOfWork.Departments.Update(department);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Department updated successfully: {Id}", department.Id);

                    return RedirectToAction(nameof(Index));
                }

                // If ModelState is invalid, repopulate dropdowns
                var depts = await _unitOfWork.Departments.GetAllAsync();
                var deptsExceptCurrent = depts.Where(d => d.Id != id).ToList();
                ViewBag.ParentDepartments = new SelectList(deptsExceptCurrent, "Id", "Name");

                var allUsers = await _userManager.GetUsersInRoleAsync("Manager");
                ViewBag.Users = new SelectList(allUsers, "Id", "FullName");

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the delete confirmation page for a department
        /// </summary>
        /// <param name="id">The ID of the department to delete</param>
        /// <returns>View with delete confirmation</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching department {Id} for deletion", id);

                var department = await _unitOfWork.Departments.GetByIdAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department not found: {Id}", id);
                    return NotFound();
                }

                var viewModel = new DepartmentViewModel
                {
                    Id = department.Id,
                    Name = department.Name,
                    Code = department.Code,
                    Description = department.Description ?? string.Empty,
                    Status = department.IsActive ? "Active" : "Inactive",
                    CreatedAt = department.CreatedAt,
                    LastUpdated = department.UpdatedAt
                };

                // Check for child departments
                var childDepts = await _unitOfWork.Departments.GetAllAsync(d => d.ParentDepartmentId == id);
                if (childDepts.Any())
                {
                    ViewBag.HasChildDepartments = true;
                    ViewBag.ChildDepartmentsCount = childDepts.Count();
                }

                // Check for employees
                var employees = (await _userManager.GetUsersInRoleAsync("Employee"))
                    .Where(u => u.DepartmentId == department.Id).ToList();
                if (employees.Any())
                {
                    ViewBag.HasEmployees = true;
                    ViewBag.EmployeesCount = employees.Count;
                }

                // Check for KPIs
                var krisWithDept = await _unitOfWork.KRIs.GetAllAsync(k => k.Department == department.Name);
                var pisWithDept = await _unitOfWork.PIs.GetAllAsync(k => k.Department == department.Name);
                var risWithDept = await _unitOfWork.RIs.GetAllAsync(k => k.Department == department.Name);
                var kpiCount = krisWithDept.Count() + pisWithDept.Count() + risWithDept.Count();
                if (kpiCount > 0)
                {
                    ViewBag.HasKpis = true;
                    ViewBag.KpisCount = kpiCount;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing delete view for department {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Processes the department deletion
        /// </summary>
        /// <param name="id">The ID of the department to delete</param>
        /// <returns>Redirects to Index on success</returns>
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting department: {Id}", id);

                var department = await _unitOfWork.Departments.GetByIdAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department not found: {Id}", id);
                    return NotFound();
                }

                // Check for child departments
                var childDepts = await _unitOfWork.Departments.GetAllAsync(d => d.ParentDepartmentId == id);
                if (childDepts.Any())
                {
                    _logger.LogWarning("Cannot delete department {Id} because it has child departments", id);
                    TempData["ErrorMessage"] = "Cannot delete this department because it has child departments.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Check for employees
                var employees = (await _userManager.GetUsersInRoleAsync("Employee"))
                    .Where(u => u.DepartmentId == department.Id).ToList();
                if (employees.Any())
                {
                    _logger.LogWarning("Cannot delete department {Id} because it has employees", id);
                    TempData["ErrorMessage"] = "Cannot delete this department because it has employees.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Check for KPIs
                var krisWithDept = await _unitOfWork.KRIs.GetAllAsync(k => k.Department == department.Name);
                var pisWithDept = await _unitOfWork.PIs.GetAllAsync(k => k.Department == department.Name);
                var risWithDept = await _unitOfWork.RIs.GetAllAsync(k => k.Department == department.Name);
                var kpiCount = krisWithDept.Count() + pisWithDept.Count() + risWithDept.Count();
                if (kpiCount > 0)
                {
                    _logger.LogWarning("Cannot delete department {Id} because it has KPIs", id);
                    TempData["ErrorMessage"] = "Cannot delete this department because it has KPIs.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _unitOfWork.Departments.Delete(department);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Department deleted successfully: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the organizational hierarchy
        /// </summary>
        /// <returns>View with organizational hierarchy</returns>
        [HttpGet]
        public async Task<IActionResult> Hierarchy()
        {
            try
            {
                _logger.LogInformation("Fetching organizational hierarchy");

                var allDepartments = await _unitOfWork.Departments.GetAllAsync();

                // Find root departments (those without a parent)
                var rootDepartments = allDepartments.Where(d => !d.ParentDepartmentId.HasValue).ToList();

                // Build hierarchy for each root department
                var hierarchyViewModel = new List<DepartmentHierarchyViewModel>();

                foreach (var rootDept in rootDepartments)
                {
                    var rootViewModel = new DepartmentHierarchyViewModel
                    {
                        DepartmentId = rootDept.Id,
                        DepartmentName = rootDept.Name,
                        HierarchyLevel = 0,
                        DepartmentPath = rootDept.Name,
                        Description = rootDept.Description ?? string.Empty,
                        ChildDepartments = new List<DepartmentHierarchyViewModel>()
                    };

                    // Get manager name if exists
                    if (!string.IsNullOrEmpty(rootDept.DepartmentHeadId))
                    {
                        var manager = await _userManager.FindByIdAsync(rootDept.DepartmentHeadId);
                        if (manager != null)
                        {
                            rootViewModel.DepartmentHead = $"{manager.FirstName} {manager.LastName}";
                        }
                    }

                    // Build child departments recursively
                    await BuildDepartmentHierarchy(rootViewModel, allDepartments, rootDept.Id);

                    hierarchyViewModel.Add(rootViewModel);
                }

                return View(hierarchyViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching organizational hierarchy");
                return View("Error");
            }
        }

        /// <summary>
        /// Recursive method to build department hierarchy
        /// </summary>
        /// <param name="parentViewModel">The parent view model</param>
        /// <param name="allDepartments">All departments from the database</param>
        /// <param name="parentId">The ID of the parent department</param>
        private async Task BuildDepartmentHierarchy(
            DepartmentHierarchyViewModel parentViewModel,
            IEnumerable<Department> allDepartments,
            Guid parentId)
        {
            // Find child departments
            var childDepartments = allDepartments.Where(d => d.ParentDepartmentId.HasValue && d.ParentDepartmentId.Value == parentId).ToList();

            if (childDepartments.Any())
            {
                parentViewModel.HasChildren = true;

                foreach (var childDept in childDepartments)
                {
                    var childViewModel = new DepartmentHierarchyViewModel
                    {
                        DepartmentId = childDept.Id,
                        DepartmentName = childDept.Name,
                        ParentDepartmentId = childDept.ParentDepartmentId,
                        ParentDepartmentName = parentViewModel.DepartmentName,
                        HierarchyLevel = parentViewModel.HierarchyLevel + 1,
                        DepartmentPath = $"{parentViewModel.DepartmentPath} > {childDept.Name}",
                        Description = childDept.Description ?? string.Empty,
                        ChildDepartments = new List<DepartmentHierarchyViewModel>()
                    };

                    // Get manager name if exists
                    if (!string.IsNullOrEmpty(childDept.DepartmentHeadId))
                    {
                        var manager = await _userManager.FindByIdAsync(childDept.DepartmentHeadId);
                        if (manager != null)
                        {
                            childViewModel.DepartmentHead = $"{manager.FirstName} {manager.LastName}";
                        }
                    }

                    // Recursively build child departments
                    await BuildDepartmentHierarchy(childViewModel, allDepartments, childDept.Id);

                    parentViewModel.ChildDepartments.Add(childViewModel);
                }
            }
        }

        /// <summary>
        /// Creates sample department data for demonstration purposes
        /// </summary>
        /// <returns>Task to await</returns>
        private async Task CreateSampleDepartmentsAsync()
        {
            _logger.LogInformation("Creating sample departments");

            var departments = new List<Department>
            {
                new Department
                {
                    Name = "Operations",
                    Code = "OPS",
                    Description = "Operations Department",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "system",
                    HierarchyLevel = 0
                },
                new Department
                {
                    Name = "Finance",
                    Code = "FIN",
                    Description = "Finance Department",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "system",
                    HierarchyLevel = 0
                },
                new Department
                {
                    Name = "Human Resources",
                    Code = "HR",
                    Description = "Human Resources Department",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "system",
                    HierarchyLevel = 0
                },
                new Department
                {
                    Name = "Marketing",
                    Code = "MKT",
                    Description = "Marketing Department",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "system",
                    HierarchyLevel = 0
                },
                new Department
                {
                    Name = "Information Technology",
                    Code = "IT",
                    Description = "Information Technology Department",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "system",
                    HierarchyLevel = 0
                }
            };

            foreach (var dept in departments)
            {
                // Check if department with same code already exists
                var existingDept = await _unitOfWork.Departments.FirstOrDefaultAsync(d => d.Code == dept.Code);
                if (existingDept == null)
                {
                    await _unitOfWork.Departments.AddAsync(dept);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}