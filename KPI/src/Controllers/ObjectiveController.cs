namespace KPISolution.Controllers
{
    [Authorize]
    public class ObjectiveController : Controller
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
        public IActionResult Index(ObjectiveListViewModel model)
        {
            if (model == null)
            {
                model = new ObjectiveListViewModel
                {
                    SearchTerm = string.Empty
                };
            }

            try
            {
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

                // Lấy danh sách objectives (không phân trang khi sử dụng DataTable)
                var objectives = objectivesQuery.OrderBy(o => o.Name).ToList();

                // Map to view model
                var objectiveListItems = this._mapper.Map<List<ObjectiveListItemViewModel>>(objectives);

                // Lấy các dữ liệu cho dropdown
                var departments = this._unitOfWork.Departments.GetAll().ToList();
                this.ViewBag.Departments = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(departments, "Id", "Name");

                // Gán dữ liệu vào model
                model.Objectives = objectiveListItems;
                model.TotalItems = objectiveListItems.Count;

                return View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving objective list");
                return View("Error", new ErrorViewModel { Message = "An error occurred while retrieving objectives." });
            }
        }

        // GET: Objective/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                this._logger.LogInformation("Retrieving objective details for ID: {Id}", id);

                // Tìm objective theo ID
                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found", id);
                    return NotFound();
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
                        Type = sf.IsCritical ? "Yếu tố cốt lõi" : "Yếu tố thường",
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

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving objective details for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while retrieving objective details." });
            }
        }

        // GET: Objective/Create
        public IActionResult Create()
        {
            try
            {
                this._logger.LogInformation("Displaying objective create form");

                // Get departments for dropdown
                var departments = this._unitOfWork.Departments.GetAll().ToList();
                this._logger.LogInformation("Found {count} departments", departments.Count);

                // If no departments, add a sample one for testing
                if (departments.Count == 0)
                {
                    var sampleDepartment = new Department
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ban Điều Hành",
                        Description = "Ban điều hành công ty",
                        Code = "BDH"
                    };
                    departments.Add(sampleDepartment);
                    this._logger.LogWarning("Added sample department for testing");
                }

                // Get parent objectives for dropdown
                var parentObjectives = this._unitOfWork.Objectives.GetAll().ToList();
                this._logger.LogInformation("Found {count} parent objectives", parentObjectives.Count);

                // Get users for responsible person dropdown
                var users = this._userManager.Users.ToList();
                this._logger.LogInformation("Found {count} users for responsible persons dropdown", users.Count);

                // Debug: Check user properties
                foreach (var user in users.Take(3))
                {
                    this._logger.LogInformation("User details - Id: {id}, UserName: {username}, FirstName: {firstname}, LastName: {lastname}",
                        user.Id, user.UserName, user.FirstName, user.LastName);
                }

                // If no users, add a sample one for testing
                if (users.Count == 0)
                {
                    var sampleUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "admin@deha.com",
                        Email = "admin@deha.com",
                        FirstName = "Quản",
                        LastName = "Trị Viên"
                    };
                    users.Add(sampleUser);
                    this._logger.LogWarning("Added sample user for testing");
                }

                // Set up ViewBag with dropdown data
                this.ViewBag.Departments = departments;
                this.ViewBag.ParentObjectives = parentObjectives;
                this.ViewBag.ResponsiblePersons = users;

                // Tạo ViewModel với giá trị mặc định
                var viewModel = new ObjectiveCreateViewModel
                {
                    Code = "", // Để trống để người dùng có thể sử dụng chức năng tự sinh mã
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddMonths(3),
                    Status = ObjectiveStatus.NotStarted,
                    ProgressPercentage = 0,
                    Timeframe = TimeframeType.ShortTerm,
                    Perspective = BusinessPerspective.Financial
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective create form");
                return View("Error", new ErrorViewModel { Message = "An error occurred while loading the create form." });
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
                        var departments = this._unitOfWork.Departments.GetAll().ToList();
                        var parentObjectives = this._unitOfWork.Objectives.GetAll().ToList();
                        var users = this._userManager.Users.ToList();
                        this.ViewBag.Departments = departments;
                        this.ViewBag.ParentObjectives = parentObjectives;
                        this.ViewBag.ResponsiblePersons = users;
                        return View(viewModel);
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

                    return this.RedirectToAction(nameof(this.Details), new { id = objectiveToCreate.Id });
                }

                // If we got this far, something failed, redisplay form
                var depts = this._unitOfWork.Departments.GetAll().ToList();
                var parentObjs = this._unitOfWork.Objectives.GetAll().ToList();
                var respPersons = this._userManager.Users.ToList();
                this.ViewBag.Departments = depts;
                this.ViewBag.ParentObjectives = parentObjs;
                this.ViewBag.ResponsiblePersons = respPersons;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating objective");
                this.ModelState.AddModelError("", "An error occurred while creating the objective.");
                // Lấy lại các dữ liệu cho dropdown
                var departments = this._unitOfWork.Departments.GetAll().ToList();
                var parentObjectives = this._unitOfWork.Objectives.GetAll().ToList();
                var users = this._userManager.Users.ToList();
                this.ViewBag.Departments = departments;
                this.ViewBag.ParentObjectives = parentObjectives;
                this.ViewBag.ResponsiblePersons = users;
                return View(viewModel);
            }
        }

        // GET: Objective/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                this._logger.LogInformation("Displaying objective edit form for ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found for editing", id);
                    return this.NotFound();
                }

                // Chuyển đổi Guid từ ResponsiblePersonId
                Guid responsiblePersonGuid = Guid.Empty;
                if (!string.IsNullOrEmpty(objective.ResponsiblePersonId))
                {
                    Guid.TryParse(objective.ResponsiblePersonId, out responsiblePersonGuid);
                }

                // Map to view model
                var viewModel = this._mapper.Map<ObjectiveCreateViewModel>(objective);

                // Tạo đối tượng mới với giá trị ResponsiblePersonId đã chuyển đổi
                var updatedViewModel = new ObjectiveCreateViewModel
                {
                    Id = viewModel.Id,
                    Code = viewModel.Code,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Status = viewModel.Status,
                    Timeframe = viewModel.Timeframe,
                    Perspective = viewModel.Perspective,
                    DepartmentId = viewModel.DepartmentId,
                    ParentObjectiveId = viewModel.ParentObjectiveId,
                    ProgressPercentage = viewModel.ProgressPercentage,
                    ResponsiblePersonId = responsiblePersonGuid
                };

                // Get departments for dropdown
                var departments = this._unitOfWork.Departments.GetAll().ToList();

                // Get parent objectives for dropdown
                var parentObjectives = this._unitOfWork.Objectives
                    .GetAll()
                    .Where(o => o.Id != id) // Exclude current objective
                    .ToList();

                // Get users for responsible person dropdown
                var users = this._userManager.Users.ToList();

                // Set up ViewBag with dropdown data
                this.ViewBag.Departments = departments;
                this.ViewBag.ParentObjectives = parentObjectives;
                this.ViewBag.ResponsiblePersons = users;

                return View(updatedViewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective edit form for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while loading the edit form." });
            }
        }

        // POST: Objective/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ObjectiveCreateViewModel viewModel)
        {
            try
            {
                if (id.ToString() != viewModel.Id?.ToString())
                {
                    this._logger.LogWarning("Objective ID mismatch: {Id} vs {ViewModelId}", id, viewModel.Id);
                    return this.NotFound();
                }

                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Updating objective with ID: {Id}", id);

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

                    return this.RedirectToAction(nameof(this.Details), new { id = objective.Id });
                }

                // If we got this far, something failed, redisplay form
                var departments = this._unitOfWork.Departments.GetAll().ToList();
                var parentObjectives = this._unitOfWork.Objectives
                    .GetAll()
                    .Where(o => o.Id != id)
                    .ToList();

                this.ViewBag.Departments = departments;
                this.ViewBag.ParentObjectives = parentObjectives;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while updating objective with ID: {Id}", id);
                this.ModelState.AddModelError("", "An error occurred while updating the objective.");
                return View(viewModel);
            }
        }

        // GET: Objective/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                this._logger.LogInformation("Displaying objective delete confirmation for ID: {Id}", id);

                var objective = await this._unitOfWork.Objectives.GetByIdAsync(id);
                if (objective == null)
                {
                    this._logger.LogWarning("Objective with ID {Id} not found for deletion", id);
                    return this.NotFound();
                }

                // Map to view model
                var viewModel = this._mapper.Map<ObjectiveViewModel>(objective);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective delete confirmation for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while loading the delete confirmation." });
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
                var hasChildren = this._unitOfWork.Objectives
                    .GetAll()
                    .Any(o => o.ParentId == id);

                if (hasChildren)
                {
                    this._logger.LogWarning("Cannot delete objective with ID {Id} because it has child objectives", id);
                    this.ModelState.AddModelError("", "Cannot delete this objective because it has child objectives.");
                    var viewModel = this._mapper.Map<ObjectiveViewModel>(objective);
                    return View(viewModel);
                }

                // Check if has success factors
                var hasSuccessFactors = this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Any(sf => sf.ObjectiveId == id);

                if (hasSuccessFactors)
                {
                    this._logger.LogWarning("Cannot delete objective with ID {Id} because it has success factors", id);
                    this.ModelState.AddModelError("", "Cannot delete this objective because it has success factors.");
                    var viewModel = this._mapper.Map<ObjectiveViewModel>(objective);
                    return View(viewModel);
                }

                // Delete and save changes
                this._unitOfWork.Objectives.Delete(objective);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Objective deleted successfully with ID: {Id}", id);

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while deleting objective with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while deleting the objective." });
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

                return View(objectiveTreeViewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying objective tree view");
                return View("Error", new ErrorViewModel { Message = "An error occurred while loading the objective tree view." });
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
                string prefix = "OBJ";
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;

                // Lấy mã số tiếp theo trong tháng
                var latestObjectives = this._unitOfWork.Objectives.GetAll()
                    .Where(o => o.Code.StartsWith($"{prefix}-{year}{month:D2}"))
                    .OrderByDescending(o => o.Code)
                    .ToList();

                int nextNumber = 1;

                if (latestObjectives.Any())
                {
                    // Tách phần số từ mã mới nhất
                    var latestCode = latestObjectives.First().Code;
                    var parts = latestCode.Split('-');
                    if (parts.Length > 1 && parts[1].Length > 6) // OBJ-YYYYMM001
                    {
                        string numPart = parts[1].Substring(6); // Lấy phần số sau YYYYMM
                        if (int.TryParse(numPart, out int currentNum))
                        {
                            nextNumber = currentNum + 1;
                        }
                    }
                }

                string newCode = $"{prefix}-{year}{month:D2}{nextNumber:D3}";

                return Json(new { success = true, code = newCode });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error generating objective code");
                return Json(new { success = false, message = "Lỗi khi tạo mã mục tiêu" });
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
                    return Json(new { exists = false });
                }

                bool exists = await this._unitOfWork.Objectives.ExistsAsync(o => o.Code == code);

                return Json(new { exists });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error checking if objective code exists");
                return Json(new { error = "Lỗi khi kiểm tra mã mục tiêu" });
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
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachMucTieu.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while exporting objectives to Excel");
                return View("Error", new ErrorViewModel { Message = "An error occurred while exporting objectives to Excel." });
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

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying success factor assignment for objective ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while loading success factor assignment." });
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

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while assigning success factors to objective with ID: {Id}", viewModel.ObjectiveId);

                // Rollback transaction
                await this._unitOfWork.RollbackTransactionAsync();

                this.ModelState.AddModelError("", "An error occurred while assigning success factors.");
                return View(viewModel);
            }
        }
    }
}
