using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Controllers
{
    [Authorize]
    public class SuccessFactorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SuccessFactorController> _logger;

        public SuccessFactorController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SuccessFactorController> logger)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: SuccessFactor
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                this._logger.LogInformation("Retrieving success factor list with search term: {SearchTerm}, page: {Page}", searchTerm, page);

                var successFactorsQuery = this._unitOfWork.SuccessFactors.GetAll();

                // Log số lượng yếu tố thành công trước khi lọc
                var totalBefore = successFactorsQuery.Count();
                this._logger.LogInformation("Total success factors before filtering: {Total}", totalBefore);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    successFactorsQuery = successFactorsQuery.Where(sf =>
                        sf.Name.Contains(searchTerm) ||
                        (sf.Description != null && sf.Description.Contains(searchTerm)) ||
                        sf.Code.Contains(searchTerm));
                }

                // Include Department for DepartmentName
                successFactorsQuery = successFactorsQuery.Include(sf => sf.Department);

                // Get total count for pagination
                var totalItems = await successFactorsQuery.CountAsync();

                // Apply pagination
                var successFactors = await successFactorsQuery
                    .OrderBy(sf => sf.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var successFactorListItems = this._mapper?.Map<List<SuccessFactorListItemViewModel>>(successFactors) ?? new List<SuccessFactorListItemViewModel>();

                // Create view model with pagination
                var model = new SuccessFactorListViewModel
                {
                    Items = successFactorListItems,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalItems,
                    SearchTerm = searchTerm
                };

                // Populate dropdowns for departments and objectives
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                var objectives = await this._unitOfWork.Objectives.GetAllAsync();

                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name");

                return View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving success factor list");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất danh sách yếu tố thành công." });
            }
        }

        // GET: SuccessFactor/ByObjective/{objectiveId}
        [HttpGet]
        public async Task<IActionResult> ByObjective(Guid objectiveId, string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                this._logger.LogInformation("Retrieving success factors for objective ID: {ObjectiveId}", objectiveId);

                // Lấy Objective để hiển thị thông tin
                var objective = await this._unitOfWork.Objectives.GetByIdAsync(objectiveId);
                if (objective == null)
                {
                    return NotFound("Không tìm thấy mục tiêu");
                }

                // Lọc theo ObjectiveId
                var successFactorsQuery = this._unitOfWork.SuccessFactors.GetAll()
                    .Where(sf => sf.ObjectiveId == objectiveId);

                // Log số lượng yếu tố thành công cho objective này
                var totalBeforeOther = successFactorsQuery.Count();
                this._logger.LogInformation("Total success factors for objective {ObjectiveId}: {Total}", objectiveId, totalBeforeOther);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    successFactorsQuery = successFactorsQuery.Where(sf =>
                        sf.Name.Contains(searchTerm) ||
                        (sf.Description != null && sf.Description.Contains(searchTerm)) ||
                        sf.Code.Contains(searchTerm));
                }

                // Include Department for DepartmentName
                successFactorsQuery = successFactorsQuery.Include(sf => sf.Department);

                // Get total count for pagination
                var totalItems = await successFactorsQuery.CountAsync();

                // Apply pagination
                var successFactors = await successFactorsQuery
                    .OrderBy(sf => sf.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var successFactorListItems = this._mapper?.Map<List<SuccessFactorListItemViewModel>>(successFactors) ?? new List<SuccessFactorListItemViewModel>();

                // Create view model with pagination
                var model = new SuccessFactorListViewModel
                {
                    Items = successFactorListItems,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalItems,
                    SearchTerm = searchTerm,
                    SelectedObjectiveId = objectiveId
                };

                ViewBag.ObjectiveName = objective.Name;
                ViewBag.ObjectiveId = objectiveId;

                return View("Index", model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving success factors for objective");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất danh sách yếu tố thành công cho mục tiêu này." });
            }
        }

        // GET: SuccessFactor/CriticalSuccessFactors
        [HttpGet]
        public async Task<IActionResult> CriticalSuccessFactors(string searchTerm = "", Guid? selectedDepartmentId = null,
            SuccessFactorStatus? selectedStatus = null, SuccessFactorCategory? selectedCategory = null,
            Guid? selectedObjectiveId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                this._logger.LogInformation("Retrieving critical success factor list with filters");

                // Start with critical success factors only
                var query = this._unitOfWork.SuccessFactors.GetAll().Where(sf => sf.IsCritical);

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(sf =>
                        sf.Name.Contains(searchTerm) ||
                        (sf.Description != null && sf.Description.Contains(searchTerm)) ||
                        sf.Code.Contains(searchTerm));
                }

                if (selectedDepartmentId.HasValue)
                {
                    query = query.Where(sf => sf.DepartmentId == selectedDepartmentId.Value);
                }

                if (selectedStatus.HasValue)
                {
                    query = query.Where(sf => sf.Status == selectedStatus.Value);
                }

                if (selectedCategory.HasValue)
                {
                    query = query.Where(sf => sf.Category == selectedCategory.Value);
                }

                if (selectedObjectiveId.HasValue)
                {
                    query = query.Where(sf => sf.ObjectiveId == selectedObjectiveId.Value);
                }

                // Include Department for DepartmentName
                query = query.Include(sf => sf.Department);

                // Get total count for pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var criticalSuccessFactors = await query
                    .OrderBy(sf => sf.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var items = this._mapper?.Map<List<SuccessFactorListItemViewModel>>(criticalSuccessFactors)
                    ?? new List<SuccessFactorListItemViewModel>();

                // Create view model with pagination and filter values
                var model = new SuccessFactorListViewModel
                {
                    Items = items,
                    SearchTerm = searchTerm,
                    SelectedDepartmentId = selectedDepartmentId,
                    SelectedStatus = selectedStatus,
                    SelectedCategory = selectedCategory ?? SuccessFactorCategory.Quality,
                    SelectedObjectiveId = selectedObjectiveId,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                // Populate dropdowns for departments and objectives
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                var objectives = await this._unitOfWork.Objectives.GetAllAsync();

                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name");

                return View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving critical success factor list");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất danh sách yếu tố cốt lõi." });
            }
        }

        // GET: SuccessFactor/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                this._logger.LogInformation("Retrieving success factor details for ID: {Id}", id);

                var successFactor = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Include(sf => sf.Department)
                    .FirstOrDefaultAsync(sf => sf.Id == id);

                if (successFactor == null)
                {
                    this._logger.LogWarning("Success factor with ID {Id} not found", id);
                    return this.NotFound();
                }

                // Map to view model
                var viewModel = this._mapper?.Map<SuccessFactorDetailsViewModel>(successFactor) ?? new SuccessFactorDetailsViewModel();

                // Load related data
                if (successFactor.ObjectiveId.HasValue)
                {
                    var objective = await this._unitOfWork.Objectives.GetByIdAsync(successFactor.ObjectiveId.Value);
                    viewModel.ObjectiveName = objective?.Name ?? string.Empty;
                }

                if (successFactor.ParentId.HasValue)
                {
                    var parent = await this._unitOfWork.SuccessFactors.GetByIdAsync(successFactor.ParentId.Value);
                    viewModel.ParentName = parent?.Name ?? string.Empty;
                }

                // Lấy lịch sử cập nhật tiến độ
                var progressUpdates = await _unitOfWork.ProgressUpdates
                    .GetAll()
                    .Where(p => p.SuccessFactorId == id)
                    .OrderByDescending(p => p.UpdateDate)
                    .Take(10) // Lấy 10 cập nhật gần nhất
                    .ToListAsync();

                // Chuyển đổi sang view model
                viewModel.ProgressHistory = new List<ProgressUpdateViewModel>();
                foreach (var update in progressUpdates)
                {
                    viewModel.ProgressHistory.Add(new ProgressUpdateViewModel
                    {
                        Id = update.Id,
                        UpdateDate = update.UpdateDate,
                        ProgressPercentage = update.ProgressPercentage,
                        PreviousPercentage = update.PreviousPercentage ?? 0,
                        Status = (SuccessFactorStatus)update.Status,
                        PreviousStatus = update.PreviousStatus.HasValue ? (SuccessFactorStatus)update.PreviousStatus.Value : SuccessFactorStatus.NotStarted,
                        RiskLevel = update.RiskLevel,
                        PreviousRiskLevel = update.PreviousRiskLevel ?? RiskLevel.Low,
                        Comments = update.Comments,
                        Issues = update.Issues,
                        CreatedBy = update.CreatedBy,
                        CreatedAt = update.CreatedAt
                    });
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving success factor details for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi xem chi tiết yếu tố thành công." });
            }
        }

        // GET: SuccessFactor/Create
        public async Task<IActionResult> Create(Guid? objectiveId = null)
        {
            try
            {
                this._logger.LogInformation("Displaying success factor create form");

                // Get departments for dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                // Get objectives for dropdown
                var objectives = await this._unitOfWork.Objectives.GetAllAsync();
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name");

                // Get parent success factors for dropdown (non-critical only as parents)
                var parentSuccessFactors = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => !sf.IsCritical)
                    .ToListAsync();
                this.ViewBag.ParentSuccessFactors = new SelectList(parentSuccessFactors, "Id", "Name");

                // Create view model and set objective ID if provided
                var viewModel = new SuccessFactorCreateViewModel();
                if (objectiveId.HasValue)
                {
                    var objective = await this._unitOfWork.Objectives.GetByIdAsync(objectiveId.Value);
                    if (objective != null)
                    {
                        viewModel.ObjectiveId = objectiveId.Value;
                        this.ViewBag.ObjectiveName = objective.Name;
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying success factor create form");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi tải form tạo yếu tố thành công." });
            }
        }

        // POST: SuccessFactor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuccessFactorCreateViewModel viewModel)
        {
            try
            {
                // Log form data for debugging
                this._logger.LogInformation("Attempting to create success factor: {Name}, Code: {Code}, ObjectiveId: {ObjectiveId}",
                    viewModel.Name, viewModel.Code, viewModel.ObjectiveId);

                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Model is valid. Creating new success factor: {Name}", viewModel.Name);

                    // Create entity directly without using AutoMapper
                    var successFactor = new SuccessFactor
                    {
                        Name = viewModel.Name,
                        Description = viewModel.Description ?? string.Empty,
                        Code = viewModel.Code,
                        IsCritical = viewModel.IsCritical,
                        Priority = viewModel.Priority,
                        Weight = (int?)viewModel.Weight,
                        Status = viewModel.Status,
                        ParentId = viewModel.ParentId,
                        ObjectiveId = viewModel.ObjectiveId,
                        DepartmentId = viewModel.DepartmentId,
                        StartDate = viewModel.StartDate ?? DateTime.UtcNow,
                        TargetDate = viewModel.TargetDate ?? DateTime.UtcNow.AddMonths(3),
                        CreatedAt = DateTime.UtcNow
                    };

                    // Add and save changes
                    await this._unitOfWork.SuccessFactors.AddAsync(successFactor);
                    await this._unitOfWork.SaveChangesAsync();

                    this._logger.LogInformation("Success factor created successfully with ID: {Id}", successFactor.Id);

                    // If this was created from an objective, redirect back to that objective
                    if (viewModel.ObjectiveId != Guid.Empty)
                    {
                        return this.RedirectToAction("Details", "Objective", new { id = viewModel.ObjectiveId });
                    }

                    return this.RedirectToAction(nameof(this.Details), new { id = successFactor.Id });
                }
                else
                {
                    // Log validation errors
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            this._logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                        }
                    }
                }

                // If we got this far, something failed, redisplay form
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                var objectives = await this._unitOfWork.Objectives.GetAllAsync();
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name");

                var parentSuccessFactors = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => !sf.IsCritical)
                    .ToListAsync();
                this.ViewBag.ParentSuccessFactors = new SelectList(parentSuccessFactors, "Id", "Name");

                // Restore objective name for display if ObjectiveId is provided
                if (viewModel.ObjectiveId != Guid.Empty)
                {
                    var objective = await this._unitOfWork.Objectives.GetByIdAsync(viewModel.ObjectiveId);
                    if (objective != null)
                    {
                        this.ViewBag.ObjectiveName = objective.Name;
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating success factor: {ExceptionMessage}", ex.Message);
                if (ex.InnerException != null)
                {
                    this._logger.LogError("Inner exception: {InnerExceptionMessage}", ex.InnerException.Message);
                }

                this.ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo yếu tố thành công: " + ex.Message);

                // Re-populate dropdowns
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                var objectives = await this._unitOfWork.Objectives.GetAllAsync();
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name");

                var parentSuccessFactors = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => !sf.IsCritical)
                    .ToListAsync();
                this.ViewBag.ParentSuccessFactors = new SelectList(parentSuccessFactors, "Id", "Name");

                // Restore objective name for display if ObjectiveId is provided
                if (viewModel.ObjectiveId != Guid.Empty)
                {
                    var objective = await this._unitOfWork.Objectives.GetByIdAsync(viewModel.ObjectiveId);
                    if (objective != null)
                    {
                        this.ViewBag.ObjectiveName = objective.Name;
                    }
                }

                return View(viewModel);
            }
        }

        // GET: SuccessFactor/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                this._logger.LogInformation("Editing success factor with ID: {Id}", id);

                var successFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null)
                {
                    this._logger.LogWarning("Success factor with ID {Id} not found for edit", id);
                    return this.NotFound();
                }

                // Map to view model
                var viewModel = this._mapper?.Map<SuccessFactorEditViewModel>(successFactor);

                // Prepare drop-downs
                var objectives = await this._unitOfWork.Objectives.GetAllAsync();
                var parentSuccessFactors = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => sf.Id != id) // Exclude this SF from potential parents
                    .ToListAsync();
                var departments = await this._unitOfWork.Departments.GetAllAsync();

                // Set up ViewBag for dropdowns
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name", successFactor.ObjectiveId);
                this.ViewBag.ParentSuccessFactors = new SelectList(parentSuccessFactors, "Id", "Name", successFactor.ParentId);
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name", successFactor.DepartmentId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while editing success factor with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi sửa yếu tố thành công." });
            }
        }

        // POST: SuccessFactor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SuccessFactorEditViewModel model)
        {
            try
            {
                this._logger.LogInformation("Processing edit for success factor with ID: {Id}", id);

                if (id != model.Id)
                {
                    this._logger.LogWarning("ID mismatch in the edit form. URL ID: {UrlId}, Form ID: {FormId}", id, model.Id);
                    return this.BadRequest();
                }

                if (this.ModelState.IsValid)
                {
                    try
                    {
                        // Retrieve the existing success factor
                        var existingSuccessFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(id);
                        if (existingSuccessFactor == null)
                        {
                            this._logger.LogWarning("Success factor with ID {Id} not found for update", id);
                            return this.NotFound();
                        }

                        // Bây giờ chúng ta có thể cập nhật tất cả các thuộc tính vì đã chuyển từ init sang set
                        existingSuccessFactor.Name = model.Name;
                        existingSuccessFactor.Description = model.Description;
                        // Không cập nhật Code vì đó là định danh duy nhất
                        existingSuccessFactor.IsCritical = model.IsCritical;
                        existingSuccessFactor.Priority = model.Priority;
                        existingSuccessFactor.Weight = (int?)model.Weight;
                        existingSuccessFactor.Status = model.Status;
                        existingSuccessFactor.ParentId = model.ParentId;
                        existingSuccessFactor.ObjectiveId = model.ObjectiveId;
                        existingSuccessFactor.DepartmentId = model.DepartmentId;
                        existingSuccessFactor.ProgressPercentage = model.ProgressPercentage;
                        existingSuccessFactor.StartDate = model.StartDate ?? DateTime.Now;
                        existingSuccessFactor.TargetDate = model.TargetDate ?? DateTime.Now.AddYears(1);
                        existingSuccessFactor.UpdatedAt = DateTime.Now;

                        // Update in database
                        this._unitOfWork.SuccessFactors.Update(existingSuccessFactor);
                        await this._unitOfWork.CompleteAsync();

                        this._logger.LogInformation("Success factor with ID {Id} updated successfully", id);
                        return this.RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!await this.SuccessFactorExists(id))
                        {
                            this._logger.LogWarning("Success factor with ID {Id} not found after concurrency check", id);
                            return this.NotFound();
                        }
                        else
                        {
                            this._logger.LogError(ex, "Concurrency error occurred while updating success factor with ID: {Id}", id);
                            throw;
                        }
                    }
                }

                // If we got this far, something failed, redisplay form
                this._logger.LogWarning("Model state is invalid. Redisplaying the form for success factor with ID: {Id}", id);

                // Recreate ViewBag for dropdowns
                var objectives = await this._unitOfWork.Objectives.GetAllAsync();
                var parentSuccessFactors = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => sf.Id != id) // Exclude this SF from potential parents
                    .ToListAsync();

                // Set up ViewBag for dropdowns
                this.ViewBag.Objectives = new SelectList(objectives, "Id", "Name", model.ObjectiveId);
                this.ViewBag.ParentSuccessFactors = new SelectList(parentSuccessFactors, "Id", "Name", model.ParentId);

                return View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while processing edit for success factor with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi cập nhật yếu tố thành công." });
            }
        }

        // GET: SuccessFactor/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                this._logger.LogInformation("Displaying success factor delete confirmation for ID: {Id}", id);

                var successFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null)
                {
                    this._logger.LogWarning("Success factor with ID {Id} not found for deletion", id);
                    return this.NotFound();
                }

                // Map to view model
                var viewModel = this._mapper?.Map<SuccessFactorDetailsViewModel>(successFactor) ?? new SuccessFactorDetailsViewModel();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying success factor delete confirmation for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi tải trang xác nhận xóa yếu tố thành công." });
            }
        }

        // POST: SuccessFactor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                this._logger.LogInformation("Deleting success factor with ID: {Id}", id);

                var successFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null)
                {
                    this._logger.LogWarning("Success factor with ID {Id} not found for deletion confirmation", id);
                    return this.NotFound();
                }

                // Check if has child success factors
                var hasChildren = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .AnyAsync(sf => sf.ParentId == id);

                if (hasChildren)
                {
                    this._logger.LogWarning("Cannot delete success factor with ID {Id} because it has child success factors", id);
                    this.ModelState.AddModelError("", "Không thể xóa yếu tố thành công này vì nó có các yếu tố thành công con.");
                    var viewModel = this._mapper?.Map<SuccessFactorDetailsViewModel>(successFactor) ?? new SuccessFactorDetailsViewModel();
                    return View(viewModel);
                }

                // Check if has related indicators
                var hasPerformanceIndicators = await this._unitOfWork.PerformanceIndicators
                    .GetAll()
                    .AnyAsync(pi => pi.ResultIndicatorId.HasValue && pi.ResultIndicator != null && pi.ResultIndicator.SuccessFactorId == id);

                var hasResultIndicators = await this._unitOfWork.ResultIndicators
                    .GetAll()
                    .AnyAsync(ri => ri.SuccessFactorId == id);

                if (hasPerformanceIndicators || hasResultIndicators)
                {
                    this._logger.LogWarning("Cannot delete success factor with ID {Id} because it has related indicators", id);
                    this.ModelState.AddModelError("", "Không thể xóa yếu tố thành công này vì nó có các chỉ số hiệu suất hoặc chỉ số kết quả liên quan.");
                    var viewModel = this._mapper?.Map<SuccessFactorDetailsViewModel>(successFactor) ?? new SuccessFactorDetailsViewModel();
                    return View(viewModel);
                }

                // Delete and save changes
                this._unitOfWork.SuccessFactors.Delete(successFactor);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Success factor deleted successfully with ID: {Id}", id);

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while deleting success factor with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi xóa yếu tố thành công." });
            }
        }

        /// <summary>
        /// Hiển thị form cập nhật tiến độ cho Success Factor
        /// </summary>
        /// <param name="id">ID của Success Factor cần cập nhật tiến độ</param>
        /// <returns>View cập nhật tiến độ</returns>
        [HttpGet]
        public async Task<IActionResult> UpdateProgress(Guid id)
        {
            try
            {
                _logger.LogInformation("Truy cập trang cập nhật tiến độ cho Success Factor ID: {SuccessFactorId}", id);

                // Lấy thông tin Success Factor từ repository
                var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null)
                {
                    _logger.LogWarning("Không tìm thấy Success Factor với ID: {SuccessFactorId}", id);
                    return NotFound();
                }

                // Tạo view model
                var model = new SuccessFactorProgressViewModel
                {
                    SuccessFactorId = successFactor.Id,
                    SuccessFactorName = successFactor.Name,
                    SuccessFactorCode = successFactor.Code,
                    PreviousPercentage = successFactor.ProgressPercentage,
                    PreviousStatus = successFactor.Status,
                    PreviousRiskLevel = successFactor.RiskLevel ?? RiskLevel.Low,
                    ProgressPercentage = successFactor.ProgressPercentage,
                    Status = successFactor.Status,
                    RiskLevel = successFactor.RiskLevel ?? RiskLevel.Low,
                    UpdateDate = DateTime.Today,
                    TargetDate = successFactor.TargetDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị trang cập nhật tiến độ cho Success Factor ID: {SuccessFactorId}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin cập nhật tiến độ. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Xử lý cập nhật tiến độ cho Success Factor
        /// </summary>
        /// <param name="model">Model chứa thông tin cập nhật tiến độ</param>
        /// <returns>Chuyển hướng đến trang chi tiết Success Factor sau khi cập nhật thành công</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(SuccessFactorProgressViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu không hợp lệ khi cập nhật tiến độ cho Success Factor ID: {SuccessFactorId}", model.SuccessFactorId);
                    return View(model);
                }

                // Lấy thông tin Success Factor từ repository
                var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(model.SuccessFactorId);
                if (successFactor == null)
                {
                    _logger.LogWarning("Không tìm thấy Success Factor với ID: {SuccessFactorId}", model.SuccessFactorId);
                    ModelState.AddModelError(string.Empty, "Không tìm thấy yếu tố thành công này.");
                    return View(model);
                }

                // Lưu thông tin cập nhật tiến độ
                var progressUpdate = new ProgressUpdate
                {
                    SuccessFactorId = model.SuccessFactorId,
                    UpdateDate = model.UpdateDate,
                    ProgressPercentage = model.ProgressPercentage,
                    PreviousPercentage = model.PreviousPercentage,
                    Status = model.Status,
                    PreviousStatus = model.PreviousStatus,
                    RiskLevel = model.RiskLevel,
                    PreviousRiskLevel = model.PreviousRiskLevel,
                    Comments = model.Comments,
                    Issues = model.Issues,
                    Actions = model.Actions,
                    NextSteps = model.NextSteps,
                    NextUpdateDate = model.NextUpdateDate,
                    NeedsAttention = model.NeedsAttention,
                    AttentionReason = model.AttentionReason,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System"
                };

                // Cập nhật thông tin mới cho Success Factor
                successFactor.ProgressPercentage = model.ProgressPercentage;
                successFactor.Status = model.Status;
                successFactor.RiskLevel = model.RiskLevel;
                successFactor.LastUpdated = DateTime.UtcNow;

                // Lưu thay đổi vào database
                _unitOfWork.SuccessFactors.Update(successFactor);
                await _unitOfWork.ProgressUpdates.AddAsync(progressUpdate);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Đã cập nhật tiến độ thành công cho Success Factor ID: {SuccessFactorId}", model.SuccessFactorId);
                TempData["SuccessMessage"] = "Cập nhật tiến độ thành công.";

                return RedirectToAction(nameof(Details), new { id = model.SuccessFactorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tiến độ cho Success Factor ID: {SuccessFactorId}", model.SuccessFactorId);
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật tiến độ. Vui lòng thử lại.");
                return View(model);
            }
        }

        // GET: SuccessFactor/CheckCodeExists/{code}
        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Json(new { exists = false });
                }

                bool exists = await this._unitOfWork.SuccessFactors.ExistsAsync(sf => sf.Code == code);

                return Json(new { exists });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error checking if success factor code exists");
                return Json(new { error = "Lỗi khi kiểm tra mã yếu tố thành công" });
            }
        }

        private async Task<bool> SuccessFactorExists(Guid id)
        {
            return await this._unitOfWork.SuccessFactors.ExistsAsync(sf => sf.Id == id);
        }

        // GET: SuccessFactor/AssignDepartments
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDepartments()
        {
            try
            {
                // Lấy danh sách phòng ban
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

                // Lấy danh sách yếu tố chưa có phòng ban
                var successFactorsWithoutDepartment = await this._unitOfWork.SuccessFactors
                    .GetAll()
                    .Where(sf => sf.DepartmentId == null)
                    .ToListAsync();

                var model = this._mapper?.Map<List<SuccessFactorListItemViewModel>>(successFactorsWithoutDepartment)
                    ?? new List<SuccessFactorListItemViewModel>();

                return View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while loading assign departments page");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi tải trang gán phòng ban." });
            }
        }

        // POST: SuccessFactor/AssignDepartments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDepartment(Guid id, Guid departmentId)
        {
            try
            {
                var successFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null)
                {
                    return NotFound();
                }

                successFactor.DepartmentId = departmentId;
                this._unitOfWork.SuccessFactors.Update(successFactor);
                await this._unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(AssignDepartments));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while assigning department");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi gán phòng ban." });
            }
        }

        /// <summary>
        /// Xóa một bản ghi cập nhật tiến độ
        /// </summary>
        /// <param name="id">ID của bản ghi cập nhật cần xóa</param>
        /// <param name="successFactorId">ID của Success Factor liên quan</param>
        /// <returns>Chuyển hướng đến trang chi tiết của Success Factor</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProgressUpdate(Guid id, Guid successFactorId)
        {
            try
            {
                _logger.LogInformation("Xóa bản ghi cập nhật tiến độ ID: {ProgressUpdateId}", id);

                // Kiểm tra sự tồn tại của bản ghi cập nhật
                var progressUpdate = await _unitOfWork.ProgressUpdates
                    .FirstOrDefaultAsync(p => p.Id == id && p.SuccessFactorId == successFactorId);

                if (progressUpdate == null)
                {
                    _logger.LogWarning("Không tìm thấy bản ghi cập nhật tiến độ ID: {ProgressUpdateId}", id);
                    TempData["ErrorMessage"] = "Không tìm thấy bản ghi cập nhật tiến độ.";
                    return RedirectToAction(nameof(Details), new { id = successFactorId });
                }

                // Kiểm tra xem có phải là bản ghi mới nhất không
                var isLatestUpdate = await _unitOfWork.ProgressUpdates
                    .GetAll()
                    .Where(p => p.SuccessFactorId == successFactorId)
                    .OrderByDescending(p => p.UpdateDate)
                    .ThenByDescending(p => p.CreatedAt)
                    .FirstAsync() == progressUpdate;

                // Nếu là bản ghi mới nhất, cần cập nhật lại thông tin Success Factor
                if (isLatestUpdate)
                {
                    // Tìm bản ghi cũ nhất trước bản ghi hiện tại
                    var previousUpdate = await _unitOfWork.ProgressUpdates
                        .GetAll()
                        .Where(p => p.SuccessFactorId == successFactorId && p.Id != id)
                        .OrderByDescending(p => p.UpdateDate)
                        .ThenByDescending(p => p.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (previousUpdate != null)
                    {
                        // Lấy Success Factor và cập nhật lại thông tin
                        var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(successFactorId);
                        if (successFactor != null)
                        {
                            successFactor.ProgressPercentage = previousUpdate.ProgressPercentage;
                            successFactor.Status = previousUpdate.Status;
                            successFactor.RiskLevel = previousUpdate.RiskLevel;
                            successFactor.LastUpdated = DateTime.UtcNow;

                            _unitOfWork.SuccessFactors.Update(successFactor);
                        }
                    }
                }

                // Xóa bản ghi cập nhật
                await _unitOfWork.Repository<ProgressUpdate>().DeleteAsync(progressUpdate);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Đã xóa bản ghi cập nhật tiến độ ID: {ProgressUpdateId}", id);
                TempData["SuccessMessage"] = "Đã xóa bản ghi cập nhật tiến độ thành công.";

                return RedirectToAction(nameof(Details), new { id = successFactorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bản ghi cập nhật tiến độ ID: {ProgressUpdateId}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa bản ghi cập nhật tiến độ.";
                return RedirectToAction(nameof(Details), new { id = successFactorId });
            }
        }
    }
}
