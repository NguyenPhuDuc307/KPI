using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.Entities.Identity;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;
using KPISolution.Authorization;
using Microsoft.AspNetCore.Identity;
using KPISolution.Extensions;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller responsible for managing users within the system
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class UserManagementController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserManagementController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<KpiRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the UserManagementController
        /// </summary>
        /// <param name="unitOfWork">Unit of work for data access</param>
        /// <param name="logger">Logger for the UserManagementController</param>
        /// <param name="userManager">User manager for user operations</param>
        /// <param name="roleManager">Role manager for role operations</param>
        public UserManagementController(
            IUnitOfWork unitOfWork,
            ILogger<UserManagementController> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<KpiRole> roleManager)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        /// <summary>
        /// Displays a list of all users
        /// </summary>
        /// <returns>View with list of users</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Fetching users list");

                // Setup page template
                SetupPageTemplate(
                    title: "Quản lý người dùng",
                    subtitle: "Xem và quản lý tất cả người dùng trong hệ thống",
                    icon: "bi-people");

                SetPrimaryButton(
                    text: "Thêm người dùng mới",
                    controller: "UserManagement",
                    action: "Create",
                    icon: "bi-person-plus");

                var users = _userManager.Users.ToList();

                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var userViewModel = new UserViewModel
                    {
                        Id = user.Id ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        UserName = user.UserName ?? string.Empty,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        FullName = user.FullName ?? string.Empty,
                        JobTitle = user.JobTitle ?? string.Empty,
                        IsActive = user.IsActive,
                        Roles = string.Join(", ", roles),
                        DepartmentId = user.DepartmentId,
                        DepartmentName = user.Department?.Name ?? string.Empty,
                        ManagerId = user.ManagerId ?? string.Empty,
                        ManagerName = user.Manager?.FullName ?? string.Empty,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt,
                        IsKpiOwner = user.IsKpiOwner,
                        IsDepartmentAdmin = user.IsDepartmentAdmin
                    };

                    userViewModels.Add(userViewModel);
                }

                return View(userViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                AddErrorAlert("Đã xảy ra lỗi khi tải danh sách người dùng.");
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the form to create a new user
        /// </summary>
        /// <returns>View with the create form</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                _logger.LogInformation("Displaying create user form");

                // Setup page template
                SetupPageTemplate(
                    title: "Thêm người dùng mới",
                    subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                    icon: "bi-person-plus");

                SetSecondaryButton(
                    text: "Quay lại danh sách",
                    controller: "UserManagement",
                    action: "Index",
                    icon: "bi-arrow-left");

                // Set breadcrumb
                List<(string Text, string Controller, string Action, string Id)> breadcrumbs = new List<(string, string, string, string)>
                {
                    ("Quản lý người dùng", "UserManagement", "Index", "")
                };
                SetBreadcrumb(breadcrumbs);

                // Get departments for dropdown
                var departments = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");

                // Get roles for dropdown
                var roles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                ViewBag.Roles = new SelectList(roles, "Name", "Name");

                // Get managers for dropdown
                var managers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                ViewBag.Managers = new SelectList(managers, "Id", "FullName");

                return View(new CreateUserViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing create user form");
                AddErrorAlert("Đã xảy ra lỗi khi tải form tạo người dùng.");
                return View("Error");
            }
        }

        /// <summary>
        /// Processes the create user form submission
        /// </summary>
        /// <param name="model">The user data from the form</param>
        /// <returns>Redirects to Index on success, returns the form with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Creating new user: {Email}", model.Email);

                    // Check if user already exists
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "A user with this email already exists");

                        // Setup page template
                        SetupPageTemplate(
                            title: "Thêm người dùng mới",
                            subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                            icon: "bi-person-plus");

                        SetSecondaryButton(
                            text: "Quay lại danh sách",
                            controller: "UserManagement",
                            action: "Index",
                            icon: "bi-arrow-left");

                        // Set breadcrumb
                        List<(string Text, string Controller, string Action, string Id)> breadcrumbs = new List<(string, string, string, string)>
                        {
                            ("Quản lý người dùng", "UserManagement", "Index", "")
                        };
                        SetBreadcrumb(breadcrumbs);

                        // Repopulate dropdowns
                        var departments = await _unitOfWork.Departments.GetAllAsync();
                        ViewBag.Departments = new SelectList(departments, "Id", "Name");

                        var roles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                        ViewBag.Roles = new SelectList(roles, "Name", "Name");

                        var managers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                        ViewBag.Managers = new SelectList(managers, "Id", "FullName");

                        return View(model);
                    }

                    // Create new user
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        JobTitle = model.JobTitle,
                        DepartmentId = model.DepartmentId,
                        ManagerId = model.ManagerId,
                        IsActive = model.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        IsKpiOwner = model.IsKpiOwner,
                        IsDepartmentAdmin = model.IsDepartmentAdmin
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Assign roles
                        if (!string.IsNullOrEmpty(model.Role))
                        {
                            await _userManager.AddToRoleAsync(user, model.Role);
                        }

                        _logger.LogInformation("User created successfully: {Id}", user.Id);
                        AddSuccessAlert("Người dùng đã được tạo thành công.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        // Setup page template
                        SetupPageTemplate(
                            title: "Thêm người dùng mới",
                            subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                            icon: "bi-person-plus");

                        SetSecondaryButton(
                            text: "Quay lại danh sách",
                            controller: "UserManagement",
                            action: "Index",
                            icon: "bi-arrow-left");

                        // Set breadcrumb
                        List<(string Text, string Controller, string Action, string Id)> breadcrumbs = new List<(string, string, string, string)>
                        {
                            ("Quản lý người dùng", "UserManagement", "Index", "")
                        };
                        SetBreadcrumb(breadcrumbs);

                        // Repopulate dropdowns
                        var departments = await _unitOfWork.Departments.GetAllAsync();
                        ViewBag.Departments = new SelectList(departments, "Id", "Name", model.DepartmentId);

                        var roles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                        ViewBag.Roles = new SelectList(roles, "Name", "Name", model.Role);

                        var managers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                        ViewBag.Managers = new SelectList(managers, "Id", "FullName", model.ManagerId);

                        return View(model);
                    }
                }

                // Setup page template
                SetupPageTemplate(
                    title: "Thêm người dùng mới",
                    subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                    icon: "bi-person-plus");

                SetSecondaryButton(
                    text: "Quay lại danh sách",
                    controller: "UserManagement",
                    action: "Index",
                    icon: "bi-arrow-left");

                // Set breadcrumb
                List<(string Text, string Controller, string Action, string Id)> breadcrumbItems = new List<(string, string, string, string)>
                {
                    ("Quản lý người dùng", "UserManagement", "Index", "")
                };
                SetBreadcrumb(breadcrumbItems);

                // Repopulate dropdowns
                var depts = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.Departments = new SelectList(depts, "Id", "Name", model.DepartmentId);

                var allRoles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                ViewBag.Roles = new SelectList(allRoles, "Name", "Name", model.Role);

                var allManagers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                ViewBag.Managers = new SelectList(allManagers, "Id", "FullName", model.ManagerId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                AddErrorAlert("Đã xảy ra lỗi khi tạo người dùng mới.");
                return View("Error");
            }
        }

        /// <summary>
        /// Displays details of a user
        /// </summary>
        /// <param name="id">The ID of the user to view</param>
        /// <returns>View with user details</returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                _logger.LogInformation("Fetching user details for {Id}", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Id}", id);
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);

                var viewModel = new UserViewModel
                {
                    Id = user.Id ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    JobTitle = user.JobTitle ?? string.Empty,
                    IsActive = user.IsActive,
                    Roles = string.Join(", ", roles),
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name ?? string.Empty,
                    ManagerId = user.ManagerId ?? string.Empty,
                    ManagerName = user.Manager?.FullName ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsKpiOwner = user.IsKpiOwner,
                    IsDepartmentAdmin = user.IsDepartmentAdmin
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user details for {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the form to edit an existing user
        /// </summary>
        /// <param name="id">The ID of the user to edit</param>
        /// <returns>View with the edit form</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                _logger.LogInformation("Fetching user {Id} for editing", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Id}", id);
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);

                var viewModel = new EditUserViewModel
                {
                    Id = user.Id ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    JobTitle = user.JobTitle ?? string.Empty,
                    DepartmentId = user.DepartmentId,
                    ManagerId = user.ManagerId ?? string.Empty,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    IsKpiOwner = user.IsKpiOwner,
                    IsDepartmentAdmin = user.IsDepartmentAdmin
                };

                // Get departments for dropdown
                var departments = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");

                // Get roles for dropdown
                var allRoles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                ViewBag.Roles = new SelectList(allRoles, "Name", "Name");

                // Get managers for dropdown (excluding current user)
                var managers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                var filteredManagers = managers.Where(m => m.Id != id).ToList();
                ViewBag.Managers = new SelectList(filteredManagers, "Id", "FullName");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing edit form for user {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Processes the edit user form submission
        /// </summary>
        /// <param name="id">The ID of the user to edit</param>
        /// <param name="model">The updated user data from the form</param>
        /// <returns>Redirects to Index on success, returns the form with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Updating user: {Id}", id);

                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        _logger.LogWarning("User not found: {Id}", id);
                        return NotFound();
                    }

                    // Update user properties
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.JobTitle = model.JobTitle;
                    user.DepartmentId = model.DepartmentId;
                    user.ManagerId = model.ManagerId;
                    user.IsActive = model.IsActive;
                    user.IsKpiOwner = model.IsKpiOwner;
                    user.IsDepartmentAdmin = model.IsDepartmentAdmin;

                    // Update user in the database
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        // Repopulate dropdowns
                        var departments = await _unitOfWork.Departments.GetAllAsync();
                        ViewBag.Departments = new SelectList(departments, "Id", "Name");

                        var allRoles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                        ViewBag.Roles = new SelectList(allRoles, "Name", "Name");

                        var errorScopeManagers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                        var errorScopeFilteredManagers = errorScopeManagers.Where(m => m.Id != id).ToList();
                        ViewBag.Managers = new SelectList(errorScopeFilteredManagers, "Id", "FullName");

                        return View(model);
                    }

                    // Update user roles
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    _logger.LogInformation("User updated successfully: {Id}", user.Id);
                    TempData["Success"] = "Người dùng đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }

                // If ModelState is invalid, repopulate dropdowns
                var allDepartments = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.Departments = new SelectList(allDepartments, "Id", "Name");

                var roles = _roleManager.Roles.Where(r => r.IsActive).ToList();
                ViewBag.Roles = new SelectList(roles, "Name", "Name");

                var allManagers = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Manager);
                var filteredManagers = allManagers.Where(m => m.Id != id).ToList();
                ViewBag.Managers = new SelectList(filteredManagers, "Id", "FullName");

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the delete confirmation page for a user
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>View with delete confirmation</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Fetching user {Id} for deletion", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Id}", id);
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);

                var viewModel = new UserViewModel
                {
                    Id = user.Id ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    JobTitle = user.JobTitle ?? string.Empty,
                    IsActive = user.IsActive,
                    Roles = string.Join(", ", roles),
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name ?? string.Empty,
                    ManagerName = user.Manager?.FullName ?? string.Empty,
                    CreatedAt = user.CreatedAt
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing delete form for user {Id}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Processes the delete user form submission
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>Redirects to Index on success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                _logger.LogInformation("Deleting user: {Id}", id);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Id}", id);
                    return NotFound();
                }

                // Check if this is the last admin
                var isAdmin = await _userManager.IsInRoleAsync(user, KpiAuthorizationPolicies.RoleNames.Administrator);
                if (isAdmin)
                {
                    var admins = await _userManager.GetUsersInRoleAsync(KpiAuthorizationPolicies.RoleNames.Administrator);
                    if (admins.Count <= 1)
                    {
                        _logger.LogWarning("Cannot delete the last administrator: {Id}", id);
                        TempData["Error"] = "Không thể xóa người quản trị cuối cùng.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User deleted successfully: {Id}", id);
                    TempData["Success"] = "Người dùng đã được xóa thành công.";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogWarning("Error deleting user {Id}: {Error}", id, error.Description);
                    }
                    TempData["Error"] = "Không thể xóa người dùng. Vui lòng thử lại.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                TempData["Error"] = "Đã xảy ra lỗi khi xóa người dùng.";
                return RedirectToAction(nameof(Index));
            }
        }
    }

    /// <summary>
    /// View model for a user in the list and details view
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Roles { get; set; } = string.Empty;
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string ManagerId { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsKpiOwner { get; set; }
        public bool IsDepartmentAdmin { get; set; }
    }

    /// <summary>
    /// View model for creating a new user
    /// </summary>
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [Display(Name = "Tên")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ là bắt buộc")]
        [Display(Name = "Họ")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Chức danh")]
        public string JobTitle { get; set; } = string.Empty;

        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        [Display(Name = "Quản lý")]
        public string ManagerId { get; set; } = string.Empty;

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Là người sở hữu KPI")]
        public bool IsKpiOwner { get; set; }

        [Display(Name = "Là quản trị viên phòng ban")]
        public bool IsDepartmentAdmin { get; set; }
    }

    /// <summary>
    /// View model for editing an existing user
    /// </summary>
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [Display(Name = "Tên")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ là bắt buộc")]
        [Display(Name = "Họ")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Chức danh")]
        public string JobTitle { get; set; } = string.Empty;

        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        [Display(Name = "Quản lý")]
        public string ManagerId { get; set; } = string.Empty;

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Là người sở hữu KPI")]
        public bool IsKpiOwner { get; set; }

        [Display(Name = "Là quản trị viên phòng ban")]
        public bool IsDepartmentAdmin { get; set; }
    }
}