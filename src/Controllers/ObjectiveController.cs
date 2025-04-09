namespace KPISolution.Controllers
{
    [Authorize]
    public class ObjectiveController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork = default!;
        private readonly IMapper _mapper = default!;
        private readonly ILogger<ObjectiveController> _logger = default!;
        private readonly UserManager<ApplicationUser> _userManager = default!;

        public ObjectiveController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ObjectiveController> logger,
            UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: Objective
        [HttpGet]
        public async Task<IActionResult> Index(ObjectiveListViewModel? model = null)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                if (model == null)
                {
                    model = new ObjectiveListViewModel
                    {
                        SearchTerm = string.Empty
                    };
                }

                this._logger.LogInformation("Retrieving objective list with search term: {SearchTerm}", model.SearchTerm);

                var objectivesQuery = this._unitOfWork.Objectives.GetAll();

                // Apply filters
                if (!string.IsNullOrEmpty(model.SearchTerm))
                {
                    objectivesQuery = objectivesQuery.Where(o =>
                        o.Name.Contains(model.SearchTerm) ||
                        o.Description.Contains(model.SearchTerm) ||
                        o.Code.Contains(model.SearchTerm));
                }

                if (model.SelectedDepartmentId.HasValue)
                {
                    objectivesQuery = objectivesQuery.Where(o => o.DepartmentId == model.SelectedDepartmentId);
                }

                if (model.SelectedStatus.HasValue)
                {
                    objectivesQuery = objectivesQuery.Where(o => o.Status == model.SelectedStatus);
                }

                if (model.SelectedPerspective.HasValue)
                {
                    objectivesQuery = objectivesQuery.Where(o => o.BusinessPerspective == model.SelectedPerspective);
                }

                if (model.SelectedTimeframe.HasValue)
                {
                    objectivesQuery = objectivesQuery.Where(o => o.Timeframe == model.SelectedTimeframe);
                }

                if (model.SelectedYear.HasValue)
                {
                    int year = model.SelectedYear.Value;
                    objectivesQuery = objectivesQuery.Where(o =>
                        (o.StartDate.Year == year || o.TargetDate.Year == year) ||
                        (o.FiscalYear != null && o.FiscalYear.Contains(year.ToString())));
                }

                if (model.ShowOnlyTopLevel)
                {
                    objectivesQuery = objectivesQuery.Where(o => o.ParentId == null);
                }

                // Get objectives with pagination
                var objectives = await objectivesQuery
                    .OrderBy(o => o.Name)
                    .ToListAsync();

                // Map to view model
                var objectiveListItems = this._mapper.Map<List<ObjectiveListItemViewModel>>(objectives);

                // Get departments for dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments ?? new List<Department>(), "Id", "Name");

                // Assign data to model
                model.Objectives = objectiveListItems;
                model.TotalItems = objectiveListItems.Count;

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving objective list");
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving objectives." });
            }
        }

        // GET: Objective/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                this._logger.LogInformation("Retrieving objective details for ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found", id);
                    return this.NotFound();
                }

                // Lấy thông tin người phụ trách
                string responsiblePersonName = "Không có";
                if (!string.IsNullOrEmpty(objective.ResponsiblePersonId))
                {
                    var user = await this._userManager.FindByIdAsync(objective.ResponsiblePersonId);
                    if (user != null)
                    {
                        responsiblePersonName = $"{user.FirstName} {user.LastName}";
                    }
                }

                // Lấy tên phòng ban
                string departmentName = "Không có";
                if (objective.DepartmentId.HasValue)
                {
                    var department = await this._unitOfWork.Departments.GetByIdAsync(objective.DepartmentId.Value);
                    if (department != null)
                    {
                        departmentName = department.Name;
                    }
                }

                // Lấy danh sách các yếu tố thành công của mục tiêu
                var successFactors = await this._unitOfWork.SuccessFactors.GetAllAsync(sf => sf.ObjectiveId == id);
                this._logger.LogInformation("Found {count} success factors for objective {id}", successFactors.Count(), id);

                // Debug: check one success factor if available
                if (successFactors.Any())
                {
                    var firstSF = successFactors.First();
                    this._logger.LogInformation("First success factor - Id: {Id}, Name: {Name}, Code: {Code}, Status: {Status}",
                                               firstSF.Id, firstSF.Name, firstSF.Code, firstSF.Status);
                }

                // Lấy danh sách objective con
                var childObjectives = await this._unitOfWork.Objectives.GetAllAsync(o => o.ParentId == id);

                // Tạo view model cho child objectives
                var childObjectiveViewModels = childObjectives.Select(child => new ObjectiveViewModel
                {
                    Id = child.Id,
                    Name = child.Name,
                    Code = child.Code,
                    ProgressPercentage = child.ProgressPercentage,
                    Status = child.Status,
                    StartDate = child.StartDate,
                    EndDate = child.TargetDate,
                    Timeframe = child.Timeframe,
                    Perspective = child.BusinessPerspective
                }).ToList();

                // Tạo view model chính với tất cả thông tin
                var viewModel = new ObjectiveViewModel
                {
                    Id = objective.Id,
                    Code = objective.Code,
                    Name = objective.Name,
                    Description = objective.Description,
                    StartDate = objective.StartDate,
                    EndDate = objective.TargetDate,
                    Status = objective.Status,
                    ProgressPercentage = objective.ProgressPercentage,
                    Timeframe = objective.Timeframe,
                    Perspective = objective.BusinessPerspective,
                    Department = departmentName,
                    ResponsiblePerson = responsiblePersonName,
                    ParentObjectiveId = objective.ParentId,
                    // Ánh xạ thủ công thay vì dùng AutoMapper để tránh lỗi
                    SuccessFactors = successFactors.Select(sf => new ObjectiveSuccessFactorViewModel
                    {
                        Id = sf.Id,
                        Code = sf.Code,
                        Name = sf.Name ?? string.Empty,
                        Description = sf.Description ?? string.Empty,
                        Type = sf.IsCritical ? "Critical Success Factor" : "Yếu tố thường",
                        ProgressPercentage = sf.ProgressPercentage,
                        Status = sf.Status.ToString()
                    }).ToList(),
                    ChildObjectives = childObjectiveViewModels
                };

                // Nếu có mục tiêu cha, tìm tên mục tiêu cha
                if (objective.ParentId.HasValue)
                {
                    var parentObjective = await this._unitOfWork.Objectives.GetByIdAsync(objective.ParentId.Value);
                    if (parentObjective != null)
                    {
                        // Tạo đối tượng mới với ParentObjectiveName được cập nhật
                        var updatedViewModel = new ObjectiveViewModel
                        {
                            Id = viewModel.Id,
                            Code = viewModel.Code,
                            Name = viewModel.Name,
                            Description = viewModel.Description,
                            StartDate = viewModel.StartDate,
                            EndDate = viewModel.EndDate,
                            Status = viewModel.Status,
                            ProgressPercentage = viewModel.ProgressPercentage,
                            Timeframe = viewModel.Timeframe,
                            Perspective = viewModel.Perspective,
                            Department = viewModel.Department,
                            ResponsiblePerson = viewModel.ResponsiblePerson,
                            ParentObjectiveId = viewModel.ParentObjectiveId,
                            ParentObjectiveName = parentObjective.Name,
                            SuccessFactors = viewModel.SuccessFactors,
                            ChildObjectives = viewModel.ChildObjectives
                        };
                        viewModel = updatedViewModel;
                    }
                }

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving objective details for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving objective details." });
            }
        }

        // GET: Objective/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                this._logger.LogInformation("Displaying objective create form");

                // Get departments for dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                var parentObjectives = await this._unitOfWork.Objectives.GetAllAsync();

                // Get responsible persons (users) for dropdown
                var responsiblePersons = await this._userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.LastName + " " + u.FirstName })
                    .ToListAsync();
                this.ViewBag.ResponsiblePersons = responsiblePersons;

                // Set up ViewBag for dropdowns
                this.ViewBag.Departments = new SelectList(departments ?? new List<Department>(), "Id", "Name");
                this.ViewBag.ParentObjectives = new SelectList(parentObjectives ?? new List<Objective>(), "Id", "Name");

                return this.View(new ObjectiveCreateViewModel());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective create form");
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while loading the create form." });
            }
        }

        // POST: Objective/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ObjectiveCreateViewModel viewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(viewModel.Code))
                {
                    this.ModelState.AddModelError("Code", "Vui lòng nhập mã mục tiêu");
                }

                if (viewModel.ResponsiblePersonId == Guid.Empty)
                {
                    this.ModelState.AddModelError("ResponsiblePersonId", "Vui lòng chọn người phụ trách");
                }

                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Creating new objective: {ObjectiveName}", viewModel.Name);

                    // Kiểm tra xem mã đã tồn tại chưa
                    bool codeExists = await this._unitOfWork.Objectives.ExistsAsync(o => o.Code == viewModel.Code);
                    if (codeExists)
                    {
                        this.ModelState.AddModelError("Code", "Mã mục tiêu này đã tồn tại trong hệ thống");
                        // Lấy lại các dữ liệu cho dropdown
                        var departments = await this._unitOfWork.Departments.GetAllAsync();
                        var parentObjectives = await this._unitOfWork.Objectives.GetAllAsync();
                        this.ViewBag.Departments = new SelectList(departments, "Id", "Name");
                        this.ViewBag.ParentObjectives = new SelectList(parentObjectives, "Id", "Name");

                        // Get responsible persons for dropdown
                        var responsiblePersons = await this._userManager.Users
                            .Where(u => u.IsActive)
                            .OrderBy(u => u.LastName)
                            .ThenBy(u => u.FirstName)
                            .Select(u => new SelectListItem { Value = u.Id, Text = u.LastName + " " + u.FirstName })
                            .ToListAsync();
                        this.ViewBag.ResponsiblePersons = responsiblePersons;

                        return this.View(viewModel);
                    }

                    // Nếu viewModel.ResponsiblePersonId là Guid, lấy user ID tương ứng (string)
                    string? responsiblePersonId = null;
                    if (viewModel.ResponsiblePersonId != Guid.Empty)
                    {
                        var user = await this._userManager.FindByIdAsync(viewModel.ResponsiblePersonId.ToString());
                        if (user != null)
                        {
                            responsiblePersonId = user.Id;
                        }
                    }

                    // Clone các thuộc tính từ viewModel, gán giá trị responsiblePersonId đã chuyển đổi
                    var objectiveToCreate = new Objective
                    {
                        Code = viewModel.Code,
                        Name = viewModel.Name,
                        Description = viewModel.Description ?? string.Empty,
                        BusinessPerspective = viewModel.Perspective,
                        Timeframe = viewModel.Timeframe,
                        Status = viewModel.Status,
                        ProgressPercentage = viewModel.ProgressPercentage,
                        StartDate = viewModel.StartDate,
                        TargetDate = viewModel.EndDate,
                        DepartmentId = viewModel.DepartmentId,
                        ParentId = viewModel.ParentObjectiveId,
                        ResponsiblePersonId = responsiblePersonId,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Add and save changes
                    await this._unitOfWork.Objectives.AddAsync(objectiveToCreate);
                    await this._unitOfWork.SaveChangesAsync();

                    this._logger.LogInformation("Objective created successfully with ID: {Id}", objectiveToCreate.Id);
                    this.AddSuccessAlert("Mục tiêu đã được tạo thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If we got this far, something failed, redisplay form
                var depts = await this._unitOfWork.Departments.GetAllAsync();
                var parentObjs = await this._unitOfWork.Objectives.GetAllAsync();
                this.ViewBag.Departments = new SelectList(depts, "Id", "Name");
                this.ViewBag.ParentObjectives = new SelectList(parentObjs, "Id", "Name");

                // Get responsible persons for dropdown
                var respPersons = await this._userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.LastName + " " + u.FirstName })
                    .ToListAsync();
                this.ViewBag.ResponsiblePersons = respPersons;

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating objective");
                this.ModelState.AddModelError("", "An error occurred while creating the objective.");
                // Lấy lại các dữ liệu cho dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                var parentObjectives = await this._unitOfWork.Objectives.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");
                this.ViewBag.ParentObjectives = new SelectList(parentObjectives, "Id", "Name");

                // Get responsible persons for dropdown
                var respUsers = await this._userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.LastName + " " + u.FirstName })
                    .ToListAsync();
                this.ViewBag.ResponsiblePersons = respUsers;

                return this.View(viewModel);
            }
        }

        // GET: Objective/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                this._logger.LogInformation("Displaying objective edit form for ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found for editing", id);
                    return this.NotFound();
                }

                // Map to view model
                var viewModel = this._mapper.Map<ObjectiveCreateViewModel>(objective);

                // Prepare drop-downs
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                var parentObjectives = await this._unitOfWork.Objectives
                    .GetAll()
                    .Where(o => o.Id != id) // Exclude this objective from potential parents
                    .ToListAsync();

                // Set up ViewBag for dropdowns
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name", objective.DepartmentId);
                this.ViewBag.ParentObjectives = new SelectList(parentObjectives, "Id", "Name", objective.ParentId);

                // Get responsible persons (users) for dropdown
                var responsiblePersons = await this._userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.LastName + " " + u.FirstName, Selected = u.Id == objective.ResponsiblePersonId })
                    .ToListAsync();
                this.ViewBag.ResponsiblePersons = responsiblePersons;

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective edit form for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while loading the edit form." });
            }
        }

        // POST: Objective/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ObjectiveCreateViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    this._logger.LogWarning("ID mismatch in Edit: {FormId} vs {RouteId}", viewModel.Id, id);
                    return this.NotFound();
                }

                if (this.ModelState.IsValid)
                {
                    var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                    if (objective == null)
                    {
                        this._logger.LogWarning("Objective with ID {Id} not found for update", id);
                        return this.NotFound();
                    }

                    // Update properties
                    this._mapper.Map(viewModel, objective);

                    // Set updated date
                    objective.UpdatedAt = DateTime.UtcNow;

                    // Update and save changes
                    this._unitOfWork.Objectives.Update(objective);
                    await this._unitOfWork.SaveChangesAsync();

                    this._logger.LogInformation("Objective updated successfully with ID: {Id}", id);
                    this.AddSuccessAlert("Mục tiêu đã được cập nhật thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If we got this far, something failed, redisplay form
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                var parentObjectives = await this._unitOfWork.Objectives
                    .GetAll()
                    .Where(o => o.Id != id)
                    .ToListAsync();

                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");
                this.ViewBag.ParentObjectives = new SelectList(parentObjectives, "Id", "Name");

                // Get responsible persons (users) for dropdown
                var responsiblePersons = await this._userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.LastName + " " + u.FirstName, Selected = u.Id == viewModel.ResponsiblePersonId.ToString() })
                    .ToListAsync();
                this.ViewBag.ResponsiblePersons = responsiblePersons;

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while updating objective with ID: {Id}", id);
                this.ModelState.AddModelError("", "An error occurred while updating the objective.");
                return this.View(viewModel);
            }
        }

        // GET: Objective/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                this._logger.LogInformation("Displaying objective delete confirmation for ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found for deletion", id);
                    return this.NotFound();
                }

                // Map to view model
                var viewModel = this._mapper.Map<ObjectiveViewModel>(objective);

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective delete confirmation for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while loading the delete confirmation." });
            }
        }

        // POST: Objective/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                this._logger.LogInformation("Deleting objective with ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found for deletion confirmation", id);
                    return this.NotFound();
                }

                // Check if has child objectives
                var hasChildren = await this._unitOfWork.Objectives
                    .GetAll()
                    .AnyAsync(o => o.ParentId == id);

                if (hasChildren)
                {
                    this._logger.LogWarning("Cannot delete objective with ID {Id} because it has child objectives", id);
                    this.ModelState.AddModelError("", "Cannot delete this objective because it has child objectives.");
                    var viewModel = this._mapper.Map<ObjectiveViewModel>(objective);
                    return this.View(viewModel);
                }

                // Check if has success factors
                var hasSuccessFactors = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .AnyAsync(sf => sf.ObjectiveId == id);

                if (hasSuccessFactors)
                {
                    this._logger.LogWarning("Cannot delete objective with ID {Id} because it has success factors", id);
                    this.ModelState.AddModelError("", "Cannot delete this objective because it has success factors.");
                    var viewModel = this._mapper.Map<ObjectiveViewModel>(objective);
                    return this.View(viewModel);
                }

                // Delete and save changes
                this._unitOfWork.Objectives.Delete(objective);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Objective deleted successfully with ID: {Id}", id);
                this.AddSuccessAlert("Mục tiêu đã được xóa thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while deleting objective with ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while deleting the objective." });
            }
        }

        // GET: Objective/TreeView
        public IActionResult TreeView()
        {
            try
            {
                this._logger.LogInformation("Displaying objective tree view");

                // Get all objectives
                var allObjectives = this._unitOfWork.Objectives.GetAll().ToList();

                // Get top level objectives (those without parent)
                var rootObjectives = allObjectives.Where(o => o.ParentId == null).ToList();

                // Create tree structure
                var objectiveTreeViewModel = new List<ObjectiveTreeNodeViewModel>();

                foreach (var root in rootObjectives)
                {
                    var treeNode = this.MapToTreeNode(root, allObjectives);
                    objectiveTreeViewModel.Add(treeNode);
                }

                return this.View(objectiveTreeViewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective tree view");
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while loading the objective tree view." });
            }
        }

        // Helper method to recursively build the tree structure
        private ObjectiveTreeNodeViewModel MapToTreeNode(Objective objective, List<Objective> allObjectives)
        {
            var childObjectives = allObjectives.Where(o => o.ParentId == objective.Id).ToList();

            var treeNode = new ObjectiveTreeNodeViewModel
            {
                Id = objective.Id,
                Name = objective.Name,
                Description = objective.Description,
                BusinessPerspective = objective.BusinessPerspective,
                Priority = objective.Priority,
                Status = objective.Status,
                ProgressPercentage = objective.ProgressPercentage,
                Timeframe = objective.Timeframe,
                DepartmentId = objective.DepartmentId,
                DepartmentName = objective.Department?.Name,
                Children = []
            };

            foreach (var child in childObjectives)
            {
                treeNode.Children.Add(this.MapToTreeNode(child, allObjectives));
            }

            return treeNode;
        }

        // GET: Objective/GenerateCode
        [HttpGet]
        public IActionResult GenerateCode()
        {
            try
            {
                this._logger.LogInformation("Generating objective code");

                string prefix = "OBJ";
                var now = DateTime.Now;
                int year = now.Year % 100; // Lấy 2 chữ số cuối của năm
                int month = now.Month;

                // Lấy mã số tiếp theo trong tháng
                var latestObjectives = this._unitOfWork.Objectives.GetAll()
                    .Where(o => o.Code.StartsWith($"{prefix}-{year:D2}{month:D2}"))
                    .OrderByDescending(o => o.Code)
                    .ToList();

                int nextNumber = 1;

                if (latestObjectives.Any())
                {
                    // Tách mã thành các phần: OBJ-YYMM-XXX
                    string[] parts = latestObjectives.First().Code.Split('-');

                    if (parts.Length > 2 && parts[1].Length == 4) // OBJ-YYMM-001
                    {
                        try
                        {
                            string numPart = parts[2]; // Lấy phần số sau dấu gạch ngang thứ hai
                            if (int.TryParse(numPart, out int lastNumber))
                            {
                                nextNumber = lastNumber + 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, "Lỗi khi tạo mã mục tiêu tiếp theo");
                        }
                    }
                }

                string newCode = $"{prefix}-{year:D2}{month:D2}-{nextNumber:D3}";

                return this.Json(new { success = true, code = newCode });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error generating objective code");
                return this.Json(new { success = false, message = "Lỗi khi tạo mã mục tiêu" });
            }
        }

        // GET: Objective/CheckCodeExists/{code}
        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return this.Json(new { exists = false });
                }

                bool exists = await this._unitOfWork.Objectives.ExistsAsync(o => o.Code == code);

                return this.Json(new { exists });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error checking if objective code exists");
                return this.Json(new { error = "Lỗi khi kiểm tra mã mục tiêu" });
            }
        }

        // GET: Objective/ExportToExcel
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            try
            {
                this._logger.LogInformation("Exporting objectives to Excel");

                var objectives = this._unitOfWork.Objectives.GetAll().OrderBy(o => o.Name).ToList();
                var objectiveListItems = this._mapper.Map<List<ObjectiveListItemViewModel>>(objectives);

                // Tạo một bảng dữ liệu để xuất sang Excel
                System.Data.DataTable dt = new System.Data.DataTable("Mục tiêu");
                dt.Columns.AddRange(new System.Data.DataColumn[]
                {
                    new System.Data.DataColumn("Mã"),
                    new System.Data.DataColumn("Tên mục tiêu"),
                    new System.Data.DataColumn("Mô tả"),
                    new System.Data.DataColumn("Đơn vị"),
                    new System.Data.DataColumn("Khung thời gian"),
                    new System.Data.DataColumn("Khía cạnh"),
                    new System.Data.DataColumn("Ngày bắt đầu", typeof(DateTime)),
                    new System.Data.DataColumn("Ngày kết thúc", typeof(DateTime)),
                    new System.Data.DataColumn("Tiến độ"),
                    new System.Data.DataColumn("Trạng thái")
                });

                // Thêm dữ liệu vào bảng
                foreach (var item in objectiveListItems)
                {
                    dt.Rows.Add(
                        item.Code,
                        item.Name,
                        item.Description,
                        item.Department,
                        item.TimeframeType,
                        item.Perspective,
                        item.StartDate,
                        item.EndDate,
                        item.ProgressPercentage + "%",
                        item.Status
                    );
                }

                // Sử dụng thư viện ClosedXML để tạo file Excel
                using (var wb = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = wb.Worksheets.Add(dt);

                    // Định dạng tiêu đề
                    var headerRow = ws.Row(1);
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

                    // Tự động điều chỉnh độ rộng cột
                    ws.Columns().AdjustToContents();

                    // Chuyển đổi thành mảng byte để trả về
                    using (var ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return this.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachMucTieu.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while exporting objectives to Excel");
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while exporting objectives to Excel." });
            }
        }

        // GET: Objective/AssignSuccessFactors/5
        public async Task<IActionResult> AssignSuccessFactors(Guid id)
        {
            try
            {
                this._logger.LogInformation("Displaying success factor assignment for objective ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found for success factor assignment", id);
                    return this.NotFound();
                }

                // Get all success factors that are either unassigned or assigned to this objective
                var availableSuccessFactors = this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => sf.ObjectiveId == null || sf.ObjectiveId == id)
                    .ToList();

                // Get all success factors currently assigned to this objective
                var assignedSuccessFactors = this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => sf.ObjectiveId == id)
                    .ToList();

                // Map to view model
                var viewModel = new ObjectiveAssignSuccessFactorsViewModel
                {
                    ObjectiveId = id,
                    ObjectiveName = objective.Name,
                    AvailableSuccessFactors = this._mapper.Map<List<ObjectiveSuccessFactorViewModel>>(availableSuccessFactors),
                    SelectedSuccessFactorIds = assignedSuccessFactors.Select(sf => sf.Id).ToList()
                };

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying success factor assignment for objective ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while loading success factor assignment." });
            }
        }

        // POST: Objective/AssignSuccessFactors/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSuccessFactors(ObjectiveAssignSuccessFactorsViewModel viewModel)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Assigning success factors to objective with ID: {Id}", viewModel.ObjectiveId);

                    // Begin transaction
                    await this._unitOfWork.BeginTransactionAsync();

                    // Get all success factors currently assigned to this objective
                    var currentlyAssigned = this._unitOfWork.SuccessFactors
                        .GetAll()
                        .Where(sf => sf.ObjectiveId == viewModel.ObjectiveId)
                        .ToList();

                    // Unassign all
                    foreach (var sf in currentlyAssigned)
                    {
                        sf.ObjectiveId = null;
                        this._unitOfWork.SuccessFactors.Update(sf);
                    }

                    // Assign selected success factors
                    if (viewModel.SelectedSuccessFactorIds != null)
                    {
                        foreach (var sfId in viewModel.SelectedSuccessFactorIds)
                        {
                            var successFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(sfId);
                            if (successFactor != null)
                            {
                                successFactor.ObjectiveId = viewModel.ObjectiveId;
                                this._unitOfWork.SuccessFactors.Update(successFactor);
                            }
                        }
                    }

                    // Save changes
                    await this._unitOfWork.SaveChangesAsync();

                    // Commit transaction
                    await this._unitOfWork.CommitTransactionAsync();

                    this._logger.LogInformation("Success factors assigned successfully to objective with ID: {Id}", viewModel.ObjectiveId);

                    return this.RedirectToAction(nameof(this.Details), new { id = viewModel.ObjectiveId });
                }

                // If we got this far, something failed, redisplay form
                var objective = await this._unitOfWork.Objectives.GetByIdAsync(viewModel.ObjectiveId);
                if (objective == null)
                {
                    return this.NotFound();
                }

                // Get all available success factors
                var allSuccessFactors = this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => sf.ObjectiveId == null || sf.ObjectiveId == viewModel.ObjectiveId)
                    .ToList();

                viewModel.ObjectiveName = objective.Name;
                viewModel.AvailableSuccessFactors = this._mapper.Map<List<ObjectiveSuccessFactorViewModel>>(allSuccessFactors);

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while assigning success factors to objective with ID: {Id}", viewModel.ObjectiveId);

                // Rollback transaction
                await this._unitOfWork.RollbackTransactionAsync();

                this.ModelState.AddModelError("", "An error occurred while assigning success factors.");
                return this.View(viewModel);
            }
        }
    }
}
