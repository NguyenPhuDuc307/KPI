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
        private readonly RoleManager<IndicatorRole> _roleManager;

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
            RoleManager<IndicatorRole> roleManager)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
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
                this._logger.LogInformation("Fetching users list");

                // Setup page template
                this.SetupPageTemplate(
                    title: "Quản lý người dùng",
                    subtitle: "Xem và quản lý tất cả người dùng trong hệ thống",
                    icon: "bi-people");

                this.SetPrimaryButton(
                    text: "Thêm người dùng mới",
                    controller: "UserManagement",
                    action: "Create",
                    icon: "bi-person-plus");

                var users = await this._userManager.Users
                                              .Include(u => u.Department)
                                              .Include(u => u.Manager)
                                              .ToListAsync();

                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    var roles = await this._userManager.GetRolesAsync(user);

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
                        IsIndicatorOwner = user.IsKpiOwner,
                        IsDepartmentAdmin = user.IsDepartmentAdmin
                    };

                    userViewModels.Add(userViewModel);
                }

                return this.View(userViewModels);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error fetching users");
                this.AddErrorAlert("Đã xảy ra lỗi khi tải danh sách người dùng.");
                return this.View("Error");
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
                this._logger.LogInformation("Displaying create user form");

                // Save return URL before displaying form
                this.SaveReturnUrl();

                // Setup page template
                this.SetupPageTemplate(
                    title: "Thêm người dùng mới",
                    subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                    icon: "bi-person-plus");

                this.SetSecondaryButton(
                    text: "Quay lại danh sách",
                    controller: "UserManagement",
                    action: "Index",
                    icon: "bi-arrow-left");

                // Set breadcrumb
                List<(string Text, string Controller, string Action, string Id)> breadcrumbs = new List<(string, string, string, string)>
                {
                    ("Quản lý người dùng", "UserManagement", "Index", "")
                };
                this.SetBreadcrumb(breadcrumbs);

                // Get departments for dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                // Get roles for dropdown
                var roles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                this.ViewBag.Roles = new SelectList(roles, "Name", "Name");

                // Get managers for dropdown
                var managers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                this.ViewBag.Managers = new SelectList(managers, "Id", "FullName");

                return this.View(new CreateUserViewModel());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error preparing create user form");
                this.AddErrorAlert("Đã xảy ra lỗi khi tải form tạo người dùng.");
                return this.View("Error");
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
                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Creating new user: {Email}", model.Email);

                    // Check if user already exists
                    var existingUser = await this._userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        this.ModelState.AddModelError("Email", "A user with this email already exists");

                        // Setup page template
                        this.SetupPageTemplate(
                            title: "Thêm người dùng mới",
                            subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                            icon: "bi-person-plus");

                        this.SetSecondaryButton(
                            text: "Quay lại danh sách",
                            controller: "UserManagement",
                            action: "Index",
                            icon: "bi-arrow-left");

                        // Set breadcrumb
                        List<(string Text, string Controller, string Action, string Id)> breadcrumbs = new List<(string, string, string, string)>
                        {
                            ("Quản lý người dùng", "UserManagement", "Index", "")
                        };
                        this.SetBreadcrumb(breadcrumbs);

                        // Repopulate dropdowns
                        var departments = await this._unitOfWork.Departments.GetAllAsync();
                        this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                        var roles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                        this.ViewBag.Roles = new SelectList(roles, "Name", "Name");

                        var managers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                        this.ViewBag.Managers = new SelectList(managers, "Id", "FullName");

                        return this.View(model);
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
                        IsIndicatorOwner = model.IsIndicatorOwner,
                        IsDepartmentAdmin = model.IsDepartmentAdmin,
                        IsKpiOwner = model.IsKpiOwner
                    };

                    var result = await this._userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Assign roles
                        if (!string.IsNullOrEmpty(model.Role))
                        {
                            await this._userManager.AddToRoleAsync(user, model.Role);
                        }

                        this._logger.LogInformation("User created successfully: {Id}", user.Id);
                        this.AddSuccessAlert("Người dùng đã được tạo thành công.");
                        return this.RedirectToPreviousPage();
                    }

                    {
                        foreach (var error in result.Errors)
                        {
                            this.ModelState.AddModelError("", error.Description);
                        }

                        // Setup page template
                        this.SetupPageTemplate(
                            title: "Thêm người dùng mới",
                            subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                            icon: "bi-person-plus");

                        this.SetSecondaryButton(
                            text: "Quay lại danh sách",
                            controller: "UserManagement",
                            action: "Index",
                            icon: "bi-arrow-left");

                        // Set breadcrumb
                        List<(string Text, string Controller, string Action, string Id)> breadcrumbs = new List<(string, string, string, string)>
                        {
                            ("Quản lý người dùng", "UserManagement", "Index", "")
                        };
                        this.SetBreadcrumb(breadcrumbs);

                        // Repopulate dropdowns
                        var departments = await this._unitOfWork.Departments.GetAllAsync();
                        this.ViewBag.Departments = new SelectList(departments, "Id", "Name", model.DepartmentId);

                        var roles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                        this.ViewBag.Roles = new SelectList(roles, "Name", "Name", model.Role);

                        var managers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                        this.ViewBag.Managers = new SelectList(managers, "Id", "FullName", model.ManagerId);

                        return this.View(model);
                    }
                }

                // Setup page template
                this.SetupPageTemplate(
                    title: "Thêm người dùng mới",
                    subtitle: "Tạo tài khoản người dùng mới trong hệ thống",
                    icon: "bi-person-plus");

                this.SetSecondaryButton(
                    text: "Quay lại danh sách",
                    controller: "UserManagement",
                    action: "Index",
                    icon: "bi-arrow-left");

                // Set breadcrumb
                List<(string Text, string Controller, string Action, string Id)> breadcrumbItems = new List<(string, string, string, string)>
                {
                    ("Quản lý người dùng", "UserManagement", "Index", "")
                };
                this.SetBreadcrumb(breadcrumbItems);

                // Repopulate dropdowns
                var depts = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(depts, "Id", "Name", model.DepartmentId);

                var allRoles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                this.ViewBag.Roles = new SelectList(allRoles, "Name", "Name", model.Role);

                var allManagers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                this.ViewBag.Managers = new SelectList(allManagers, "Id", "FullName", model.ManagerId);

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error creating user");
                this.AddErrorAlert("Đã xảy ra lỗi khi tạo người dùng mới.");
                return this.View("Error");
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
                this._logger.LogInformation("Fetching user details for {Id}", id);

                var user = await this._userManager.FindByIdAsync(id);
                if (user == null)
                {
                    this._logger.LogWarning("User not found: {Id}", id);
                    return this.NotFound();
                }

                var roles = await this._userManager.GetRolesAsync(user);

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
                    IsIndicatorOwner = user.IsKpiOwner,
                    IsDepartmentAdmin = user.IsDepartmentAdmin
                };

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error fetching user details for {Id}", id);
                return this.View("Error");
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
                this._logger.LogInformation("Fetching user {Id} for editing", id);

                // Save return URL before displaying form
                this.SaveReturnUrl();

                var user = await this._userManager.FindByIdAsync(id);
                if (user == null)
                {
                    this._logger.LogWarning("User not found: {Id}", id);
                    return this.NotFound();
                }

                var roles = await this._userManager.GetRolesAsync(user);

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
                    IsIndicatorOwner = user.IsKpiOwner,
                    IsDepartmentAdmin = user.IsDepartmentAdmin,
                    IsKpiOwner = user.IsKpiOwner
                };

                // Get departments for dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                // Get roles for dropdown
                var allRoles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                this.ViewBag.Roles = new SelectList(allRoles, "Name", "Name");

                // Get managers for dropdown (excluding current user)
                var managers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                var filteredManagers = managers.Where(m => m.Id != id).ToList();
                this.ViewBag.Managers = new SelectList(filteredManagers, "Id", "FullName");

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error preparing edit form for user {Id}", id);
                return this.View("Error");
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
                    return this.NotFound();
                }

                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Updating user: {Id}", id);

                    var user = await this._userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        this._logger.LogWarning("User not found: {Id}", id);
                        return this.NotFound();
                    }

                    // Update user properties
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.JobTitle = model.JobTitle;
                    user.DepartmentId = model.DepartmentId;
                    user.ManagerId = model.ManagerId;
                    user.IsActive = model.IsActive;
                    user.IsIndicatorOwner = model.IsIndicatorOwner;
                    user.IsDepartmentAdmin = model.IsDepartmentAdmin;
                    user.IsKpiOwner = model.IsKpiOwner;

                    // Update user in the database
                    var updateResult = await this._userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            this.ModelState.AddModelError("", error.Description);
                        }

                        // Repopulate dropdowns
                        var departments = await this._unitOfWork.Departments.GetAllAsync();
                        this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                        var allRoles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                        this.ViewBag.Roles = new SelectList(allRoles, "Name", "Name");

                        var errorScopeManagers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                        var errorScopeFilteredManagers = errorScopeManagers.Where(m => m.Id != id).ToList();
                        this.ViewBag.Managers = new SelectList(errorScopeFilteredManagers, "Id", "FullName");

                        return this.View(model);
                    }

                    // Update user roles
                    var currentRoles = await this._userManager.GetRolesAsync(user);
                    await this._userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await this._userManager.AddToRoleAsync(user, model.Role);
                    }

                    this._logger.LogInformation("User updated successfully: {Id}", user.Id);
                    this.AddSuccessAlert("Người dùng đã được cập nhật thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If ModelState is invalid, repopulate dropdowns
                var allDepartments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(allDepartments, "Id", "Name");

                var roles = this._roleManager.Roles.Where(r => r.IsActive).ToList();
                this.ViewBag.Roles = new SelectList(roles, "Name", "Name");

                var allManagers = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Manager);
                var filteredManagers = allManagers.Where(m => m.Id != id).ToList();
                this.ViewBag.Managers = new SelectList(filteredManagers, "Id", "FullName");

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error updating user {Id}", id);
                return this.View("Error");
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
                this._logger.LogInformation("Fetching user {Id} for deletion", id);

                // Save return URL before displaying form
                this.SaveReturnUrl();

                var user = await this._userManager.FindByIdAsync(id);
                if (user == null)
                {
                    this._logger.LogWarning("User not found: {Id}", id);
                    return this.NotFound();
                }

                var roles = await this._userManager.GetRolesAsync(user);

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

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error preparing delete form for user {Id}", id);
                return this.View("Error");
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
                this._logger.LogInformation("Deleting user: {Id}", id);

                var user = await this._userManager.FindByIdAsync(id);
                if (user == null)
                {
                    this._logger.LogWarning("User not found: {Id}", id);
                    return this.NotFound();
                }

                // Check if this is the last admin
                var isAdmin = await this._userManager.IsInRoleAsync(user, IndicatorAuthorizationPolicies.RoleNames.Administrator);
                if (isAdmin)
                {
                    var admins = await this._userManager.GetUsersInRoleAsync(IndicatorAuthorizationPolicies.RoleNames.Administrator);
                    if (admins.Count <= 1)
                    {
                        this._logger.LogWarning("Cannot delete the last administrator: {Id}", id);
                        this.TempData["Error"] = "Không thể xóa người quản trị cuối cùng.";
                        return this.RedirectToAction(nameof(this.Index));
                    }
                }

                var result = await this._userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    this._logger.LogInformation("User deleted successfully: {Id}", id);
                    this.AddSuccessAlert("Người dùng đã được xóa thành công.");
                    return this.RedirectToPreviousPage();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        this._logger.LogWarning("Error deleting user {Id}: {Error}", id, error.Description);
                    }

                    this.TempData["Error"] = "Không thể xóa người dùng. Vui lòng thử lại.";
                }

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error deleting user {Id}", id);
                this.TempData["Error"] = "Đã xảy ra lỗi khi xóa người dùng.";
                return this.RedirectToAction(nameof(this.Index));
            }
        }

        /// <summary>
        /// Displays a list of all roles
        /// </summary>
        /// <returns>View with list of roles</returns>
        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            try
            {
                this._logger.LogInformation("Fetching roles list");

                // Setup page template
                this.SetupPageTemplate(
                    title: "Role Management",
                    subtitle: "View and manage user roles",
                    icon: "bi-person-rolodex");

                this.SetPrimaryButton(
                    text: "Add New Role",
                    controller: "UserManagement", // Assuming role creation/edit is within this controller for now
                    action: "CreateRole", // Need to create this action later
                    icon: "bi-plus-circle");

                this.SetSecondaryButton(
                    text: "User List",
                    controller: "UserManagement",
                    action: "Index",
                    icon: "bi-people");

                // Get all roles
                var roles = await this._roleManager.Roles.ToListAsync();

                // For simplicity, passing the raw IndicatorRole list for now
                // Could create a RoleViewModel later if more complex data is needed
                return this.View(roles);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error fetching roles");
                this.AddErrorAlert("An error occurred while loading the roles list.");
                return this.View("Error");
            }
        }

        #region Role Management Actions

        /// <summary>
        /// Displays the form to create a new role.
        /// </summary>
        [HttpGet]
        public IActionResult CreateRole()
        {
            this._logger.LogInformation("Displaying create role form");
            this.SetupPageTemplate("Create New Role", "Define a new user role", "bi-shield-plus");
            this.SetSecondaryButton("Back to Roles", "UserManagement", "Roles", icon: "bi-arrow-left");
            this.SetBreadcrumb(new List<(string, string, string, string)> { ("Role Management", "UserManagement", "Roles", "") });

            return this.View(new RoleViewModel());
        }

        /// <summary>
        /// Processes the create role form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            this._logger.LogInformation("Attempting to create new role: {RoleName}", model.Name);

            if (this.ModelState.IsValid)
            {
                // Check if role already exists (case-insensitive check might be better depending on requirements)
                bool roleExists = await this._roleManager.RoleExistsAsync(model.Name);
                if (roleExists)
                {
                    this._logger.LogWarning("Role creation failed: Role '{RoleName}' already exists.", model.Name);
                    this.ModelState.AddModelError("Name", "A role with this name already exists.");
                }
                else
                {
                    var role = new IndicatorRole
                    {
                        Name = model.Name,
                        Description = model.Description,
                        IsActive = model.IsActive,
                        NormalizedName = this._roleManager.NormalizeKey(model.Name) // Important for Identity
                    };

                    var result = await this._roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        this._logger.LogInformation("Role '{RoleName}' created successfully with ID {RoleId}", role.Name, role.Id);
                        this.AddSuccessAlert("Role created successfully.");
                        return this.RedirectToAction(nameof(this.Roles));
                    }

                    // If creation failed, add errors to ModelState
                    foreach (var error in result.Errors)
                    {
                        this._logger.LogError("Role creation failed for '{RoleName}': {ErrorCode} - {ErrorDescription}", model.Name, error.Code, error.Description);
                        this.ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            this._logger.LogWarning("Role creation failed for '{RoleName}' due to validation errors or other issues.", model.Name);
            this.SetupPageTemplate("Create New Role", "Define a new user role", "bi-shield-plus");
            this.SetSecondaryButton("Back to Roles", "UserManagement", "Roles", icon: "bi-arrow-left");
            this.SetBreadcrumb(new List<(string, string, string, string)> { ("Role Management", "UserManagement", "Roles", "") });

            return this.View(model);
        }

        /// <summary>
        /// Displays the form to edit an existing role.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            this._logger.LogInformation("Fetching role {RoleId} for editing", id);

            var role = await this._roleManager.FindByIdAsync(id);
            if (role == null)
            {
                this._logger.LogWarning("Role not found: {RoleId}", id);
                return this.NotFound();
            }

            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description,
                IsActive = role.IsActive
            };

            this.SetupPageTemplate("Edit Role", $"Edit details for role: {model.Name}", "bi-shield-shaded");
            this.SetSecondaryButton("Back to Roles", "UserManagement", "Roles", icon: "bi-arrow-left");
            this.SetBreadcrumb(new List<(string, string, string, string)> { ("Role Management", "UserManagement", "Roles", ""), ($"Edit: {model.Name}", "", "", "") });

            return this.View(model);
        }

        /// <summary>
        /// Processes the edit role form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, RoleViewModel model)
        {
            this._logger.LogInformation("Attempting to update role: {RoleId}", id);

            if (id != model.Id)
            {
                this._logger.LogWarning("Role ID mismatch during update. Route ID: {RouteId}, Model ID: {ModelId}", id, model.Id);
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                var role = await this._roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    this._logger.LogWarning("Role not found during update: {RoleId}", id);
                    return this.NotFound();
                }

                // Prevent editing the name of the Administrator role (optional but recommended)
                if (role.Name == IndicatorAuthorizationPolicies.RoleNames.Administrator && model.Name != IndicatorAuthorizationPolicies.RoleNames.Administrator)
                {
                    this._logger.LogWarning("Attempted to rename Administrator role (ID: {RoleId})", id);
                    this.ModelState.AddModelError("Name", "Cannot rename the Administrator role.");
                }
                // Check if new name conflicts with another existing role
                else if (!string.Equals(role.Name, model.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var existingRoleWithNewName = await this._roleManager.FindByNameAsync(model.Name);
                    if (existingRoleWithNewName != null && existingRoleWithNewName.Id != id)
                    {
                        this._logger.LogWarning("Role update failed for {RoleId}: New name '{NewName}' conflicts with existing role {ExistingRoleId}", id, model.Name, existingRoleWithNewName.Id);
                        this.ModelState.AddModelError("Name", "A role with this name already exists.");
                    }
                    else
                    {
                        role.Name = model.Name;
                        role.NormalizedName = this._roleManager.NormalizeKey(model.Name);
                    }
                }

                if (this.ModelState.IsValid) // Re-check after potential name validation errors
                {
                    role.Description = model.Description;
                    role.IsActive = model.IsActive;

                    var result = await this._roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        this._logger.LogInformation("Role {RoleId} updated successfully.", id);
                        this.AddSuccessAlert("Role updated successfully.");
                        return this.RedirectToAction(nameof(this.Roles));
                    }

                    foreach (var error in result.Errors)
                    {
                        this._logger.LogError("Role update failed for {RoleId}: {ErrorCode} - {ErrorDescription}", id, error.Code, error.Description);
                        this.ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            this._logger.LogWarning("Role update failed for {RoleId} due to validation errors or other issues.", id);
            this.SetupPageTemplate("Edit Role", $"Edit details for role: {model.Name}", "bi-shield-shaded");
            this.SetSecondaryButton("Back to Roles", "UserManagement", "Roles", icon: "bi-arrow-left");
            this.SetBreadcrumb(new List<(string, string, string, string)> { ("Role Management", "UserManagement", "Roles", ""), ($"Edit: {model.Name}", "", "", "") });

            return this.View(model);
        }

        /// <summary>
        /// Displays the delete confirmation page for a role.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            this._logger.LogInformation("Fetching role {RoleId} for deletion confirmation", id);

            var role = await this._roleManager.FindByIdAsync(id);
            if (role == null)
            {
                this._logger.LogWarning("Role not found for deletion: {RoleId}", id);
                return this.NotFound();
            }

            // Prevent deletion of Administrator role
            if (role.Name == IndicatorAuthorizationPolicies.RoleNames.Administrator)
            {
                this._logger.LogWarning("Attempted to delete Administrator role (ID: {RoleId})", id);
                this.AddErrorAlert("Cannot delete the Administrator role.");
                return this.RedirectToAction(nameof(this.Roles));
            }

            // Check if any users are assigned to this role
            var usersInRole = await this._userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
            if (usersInRole.Any())
            {
                this._logger.LogWarning("Deletion prevented for role {RoleId}: Role is assigned to {UserCount} users.", id, usersInRole.Count);
                this.AddErrorAlert($"Cannot delete role '{role.Name}' as it is currently assigned to users.");
                return this.RedirectToAction(nameof(this.Roles));
            }

            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description,
                IsActive = role.IsActive
            };

            this.SetupPageTemplate("Delete Role", $"Confirm deletion of role: {model.Name}", "bi-shield-x");
            this.SetSecondaryButton("Back to Roles", "UserManagement", "Roles", icon: "bi-arrow-left");
            this.SetBreadcrumb(new List<(string, string, string, string)> { ("Role Management", "UserManagement", "Roles", ""), ($"Delete: {model.Name}", "", "", "") });

            return this.View(model);
        }

        /// <summary>
        /// Processes the role deletion.
        /// </summary>
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            this._logger.LogInformation("Attempting to delete role: {RoleId}", id);

            var role = await this._roleManager.FindByIdAsync(id);
            if (role == null)
            {
                this._logger.LogWarning("Role not found for deletion confirmation: {RoleId}", id);
                return this.NotFound();
            }

            // Double-check critical role deletion and user assignment before proceeding
            if (role.Name == IndicatorAuthorizationPolicies.RoleNames.Administrator)
            {
                this._logger.LogError("Critical error: Attempt to delete Administrator role bypassed initial check (ID: {RoleId})", id);
                this.AddErrorAlert("Cannot delete the Administrator role.");
                return this.RedirectToAction(nameof(this.Roles));
            }
            var usersInRole = await this._userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
            if (usersInRole.Any())
            {
                this._logger.LogError("Critical error: Attempt to delete role {RoleId} with assigned users bypassed initial check.", id);
                this.AddErrorAlert($"Cannot delete role '{role.Name}' as it is currently assigned to users.");
                return this.RedirectToAction(nameof(this.Roles));
            }

            var result = await this._roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                this._logger.LogInformation("Role {RoleId} deleted successfully.", id);
                this.AddSuccessAlert("Role deleted successfully.");
                return this.RedirectToAction(nameof(this.Roles));
            }

            this._logger.LogError("Role deletion failed for {RoleId}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            this.AddErrorAlert("An error occurred while deleting the role. Please try again.");
            return this.RedirectToAction(nameof(this.Roles));
        }

        #endregion
    }
}
