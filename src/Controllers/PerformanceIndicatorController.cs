namespace KPISolution.Controllers
{
    [Authorize]
    public class PerformanceIndicatorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PerformanceIndicatorController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PerformanceIndicatorController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PerformanceIndicatorController> logger, UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: PerformanceIndicator
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10, bool showOnlyKey = false)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                this._logger.LogInformation("Retrieving performance indicator list with search term: {SearchTerm}, page: {Page}, showOnlyKey: {ShowOnlyKey}",
                    searchTerm, page, showOnlyKey);

                var performanceIndicatorsQuery = this._unitOfWork.PerformanceIndicators.GetAll();

                // Filter by key status if requested
                if (showOnlyKey)
                {
                    performanceIndicatorsQuery = performanceIndicatorsQuery.Where(pi => pi.IsKey);
                }

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    performanceIndicatorsQuery = performanceIndicatorsQuery.Where(pi =>
                        pi.Name.Contains(searchTerm) ||
                        (pi.Description != null && pi.Description.Contains(searchTerm)) ||
                        pi.Code.Contains(searchTerm));
                }

                // Include related entities
                performanceIndicatorsQuery = performanceIndicatorsQuery
                    .Include(pi => pi.ResultIndicator!)
                        .ThenInclude(ri => ri!.SuccessFactor)
                    .Include(pi => pi.Department!);

                // Get total count for pagination
                var totalItems = await performanceIndicatorsQuery.CountAsync();

                // Apply pagination
                var performanceIndicators = await performanceIndicatorsQuery
                    .OrderBy(pi => pi.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var performanceIndicatorListItems = this._mapper?.Map<List<PerformanceIndicatorListItemViewModel>>(performanceIndicators)
                    ?? new List<PerformanceIndicatorListItemViewModel>();

                // Create view model
                this.ViewBag.CurrentPage = page;
                this.ViewBag.PageSize = pageSize;
                this.ViewBag.TotalCount = totalItems;
                this.ViewBag.SearchTerm = searchTerm;
                this.ViewBag.ShowOnlyKey = showOnlyKey;
                this.ViewBag.IsKeyPerformanceIndicators = showOnlyKey;

                // Populate dropdowns for result indicators and departments
                await this.PopulateViewBagDropdowns();

                return this.View(performanceIndicatorListItems);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving performance indicator list");
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving the performance indicator list." });
            }
        }

        // GET: PerformanceIndicator/KeyPerformanceIndicators
        [HttpGet]
        public IActionResult KeyPerformanceIndicators(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            // Redirect to the unified Index action with showOnlyKey parameter set to true
            return this.RedirectToAction(nameof(this.Index), new { searchTerm, page, pageSize, showOnlyKey = true });
        }

        // GET: PerformanceIndicator/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                this._logger.LogInformation("Retrieving performance indicator details for ID: {Id}", id);

                var performanceIndicator = await this._unitOfWork.PerformanceIndicators.GetAll()
                    .Include(pi => pi.ResultIndicator!)
                        .ThenInclude(ri => ri!.SuccessFactor)
                    .Include(pi => pi.Department!)
                    .Include(pi => pi.Measurements)
                    .Include(pi => pi.SuccessFactor)
                    .FirstOrDefaultAsync(pi => pi.Id == id);

                if (performanceIndicator == null)
                {
                    this._logger.LogWarning("Performance indicator with ID {Id} not found", id);
                    return this.NotFound();
                }

                // Map entity to view model
                var viewModel = this._mapper.Map<PerformanceIndicatorDetailsViewModel>(performanceIndicator);

                // Set additional properties for view
                this.ViewBag.IsKeyPerformanceIndicator = performanceIndicator.IsKey;
                this.ViewBag.TypeDisplay = performanceIndicator.IsKey ? "KPI" : "PI";
                this.ViewBag.TypeName = performanceIndicator.IsKey ? "Key Performance Indicator" : "Performance Indicator";

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving performance indicator details for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving the performance indicator details." });
            }
        }

        // GET: PerformanceIndicator/Create
        [HttpGet]
        public async Task<IActionResult> Create(bool isKey = false, Guid? resultIndicatorId = null, Guid? successFactorId = null)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                this._logger.LogInformation("Preparing create form for {Type} with Result Indicator ID: {ResultIndicatorId}, Success Factor ID: {SuccessFactorId}",
                    isKey ? "KPI" : "PI", resultIndicatorId, successFactorId);

                // Tạo view model
                var viewModel = new PerformanceIndicatorCreateViewModel
                {
                    ResultIndicatorId = resultIndicatorId,
                    SuccessFactorId = successFactorId
                };

                await this.PopulateViewBagDropdowns(resultIndicatorId, successFactorId);

                this.ViewBag.IsKey = isKey;
                this.ViewBag.TypeDisplay = isKey ? "KPI" : "PI";
                this.ViewBag.TypeName = isKey ? "Key Performance Indicator" : "Performance Indicator";

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing the create form");
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while preparing the create form." });
            }
        }

        // POST: PerformanceIndicator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerformanceIndicatorCreateViewModel viewModel)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Creating new performance indicator: {Name}", viewModel.Name);

                    // Kiểm tra xem mã code đã tồn tại chưa
                    if (!string.IsNullOrWhiteSpace(viewModel.Code))
                    {
                        bool codeExists = await this._unitOfWork.PerformanceIndicators.GetAll()
                            .AnyAsync(pi => pi.Code == viewModel.Code);

                        if (codeExists)
                        {
                            this.ModelState.AddModelError("Code", "Mã này đã tồn tại trong hệ thống");
                            await this.PopulateViewBagDropdowns(viewModel.ResultIndicatorId, viewModel.SuccessFactorId);
                            return this.View(viewModel);
                        }
                    }

                    // Set additional properties
                    var performanceIndicator = new PerformanceIndicator
                    {
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        Code = string.IsNullOrWhiteSpace(viewModel.Code) ? this.GenerateUniqueCode(this.ViewBag.IsKey != null && this.ViewBag.IsKey ? "KPI-" : "PI-") : viewModel.Code,
                        IsKey = this.ViewBag.IsKey ?? false,
                        Formula = viewModel.Formula,
                        Frequency = viewModel.Frequency,
                        MeasurementFrequency = viewModel.Frequency,
                        Unit = viewModel.Unit.ToString(),
                        ReviewFrequency = viewModel.Frequency,
                        ActivityType = viewModel.ActivityType.ToString(),
                        ControlLevel = viewModel.ControlLevel,
                        ActionPlan = viewModel.ActionPlan,
                        DataCollectionMethod = viewModel.DataCollectionMethod.ToString(),
                        ResultIndicatorId = viewModel.ResultIndicatorId,
                        SuccessFactorId = viewModel.SuccessFactorId,
                        ResponsiblePersonId = viewModel.ResponsibleTeamMemberId?.ToString(),
                        CreatedAt = DateTime.Now,
                        CreatedBy = this.User?.Identity?.Name ?? "System",
                        IsActive = true,
                        Status = IndicatorStatus.Active
                    };

                    // Add entity to repository
                    await this._unitOfWork.PerformanceIndicators.AddAsync(performanceIndicator);
                    await this._unitOfWork.CompleteAsync();

                    this._logger.LogInformation("Performance indicator created successfully with ID: {Id}", performanceIndicator.Id);
                    this.AddSuccessAlert("Chỉ số hiệu suất đã được tạo thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If we got this far, something failed, redisplay form
                this._logger.LogWarning("Performance indicator creation failed due to validation errors");
                await this.PopulateViewBagDropdowns(viewModel.ResultIndicatorId, viewModel.SuccessFactorId);

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating performance indicator");
                this.ModelState.AddModelError("", "An error occurred while creating the performance indicator.");
                await this.PopulateViewBagDropdowns(viewModel.ResultIndicatorId, viewModel.SuccessFactorId);
                return this.View(viewModel);
            }
        }

        // GET: PerformanceIndicator/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                this._logger.LogInformation("Preparing edit form for performance indicator with ID: {Id}", id);

                var performanceIndicator = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(id);
                if (performanceIndicator == null)
                {
                    this._logger.LogWarning("Performance indicator with ID {Id} not found for editing", id);
                    return this.NotFound();
                }

                var viewModel = this._mapper.Map<PerformanceIndicatorEditViewModel>(performanceIndicator);

                // Set additional view data
                this.ViewBag.IsKeyPerformanceIndicator = performanceIndicator.IsKey;
                this.ViewBag.TypeDisplay = performanceIndicator.IsKey ? "KPI" : "PI";
                this.ViewBag.TypeName = performanceIndicator.IsKey ? "Key Performance Indicator" : "Performance Indicator";

                // Populate dropdown lists
                await this.PopulateDropdownsForEdit(viewModel);

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing edit form for performance indicator with ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while preparing the edit form." });
            }
        }

        // POST: PerformanceIndicator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PerformanceIndicatorEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                this._logger.LogWarning("Performance indicator ID mismatch: {Id1} vs {Id2}", id, viewModel.Id);
                return this.NotFound();
            }

            try
            {
                if (this.ModelState.IsValid)
                {
                    this._logger.LogInformation("Updating performance indicator with ID: {Id}", id);

                    var performanceIndicator = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(id);
                    if (performanceIndicator == null)
                    {
                        this._logger.LogWarning("Performance indicator with ID {Id} not found for update", id);
                        return this.NotFound();
                    }

                    // Update entity with view model values
                    this._mapper.Map(viewModel, performanceIndicator);

                    // Set audit properties
                    performanceIndicator.UpdatedAt = DateTime.Now;
                    performanceIndicator.UpdatedBy = this.User?.Identity?.Name ?? "System";

                    this._unitOfWork.PerformanceIndicators.Update(performanceIndicator);
                    await this._unitOfWork.CompleteAsync();

                    this._logger.LogInformation("Performance indicator updated successfully with ID: {Id}", performanceIndicator.Id);
                    this.AddSuccessAlert("Chỉ số hiệu suất đã được cập nhật thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If we got this far, something failed, redisplay form
                this._logger.LogWarning("Performance indicator update failed due to validation errors");
                await this.PopulateDropdownsForEdit(viewModel);

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while updating performance indicator with ID: {Id}", id);
                this.ModelState.AddModelError("", "An error occurred while updating the performance indicator.");
                await this.PopulateDropdownsForEdit(viewModel);
                return this.View(viewModel);
            }
        }

        // GET: PerformanceIndicator/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                this._logger.LogInformation("Preparing delete confirmation for performance indicator with ID: {Id}", id);

                var performanceIndicator = await this._unitOfWork.PerformanceIndicators.GetAll()
                    .Include(pi => pi.ResultIndicator)
                    .Include(pi => pi.Measurements)
                    .FirstOrDefaultAsync(pi => pi.Id == id);

                if (performanceIndicator == null)
                {
                    this._logger.LogWarning("Performance indicator with ID {Id} not found for deletion", id);
                    return this.NotFound();
                }

                var viewModel = this._mapper.Map<PerformanceIndicatorDetailsViewModel>(performanceIndicator);

                // Set view data
                this.ViewBag.IsKeyPerformanceIndicator = performanceIndicator.IsKey;
                this.ViewBag.TypeDisplay = performanceIndicator.IsKey ? "KPI" : "PI";
                this.ViewBag.TypeName = performanceIndicator.IsKey ? "Key Performance Indicator" : "Performance Indicator";
                this.ViewBag.HasMeasurements = performanceIndicator.Measurements?.Any() ?? false;
                this.ViewBag.MeasurementsCount = performanceIndicator.Measurements?.Count ?? 0;

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing delete confirmation for performance indicator with ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while preparing the delete confirmation." });
            }
        }

        // POST: PerformanceIndicator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                this._logger.LogInformation("Deleting performance indicator with ID: {Id}", id);

                var performanceIndicator = await this._unitOfWork.PerformanceIndicators.GetAll()
                    .Include(pi => pi.Measurements)
                    .FirstOrDefaultAsync(pi => pi.Id == id);

                if (performanceIndicator == null)
                {
                    this._logger.LogWarning("Performance indicator with ID {Id} not found for deletion confirmation", id);
                    return this.NotFound();
                }

                // Record whether this is a KPI or PI for redirection later
                var isKey = performanceIndicator.IsKey;

                // Delete all related measurements first
                if (performanceIndicator.Measurements != null && performanceIndicator.Measurements.Any())
                {
                    this._logger.LogInformation("Deleting {Count} measurements related to performance indicator with ID: {Id}",
                        performanceIndicator.Measurements.Count, id);

                    foreach (var measurement in performanceIndicator.Measurements.ToList())
                    {
                        await this._unitOfWork.Measurements.DeleteAsync(measurement);
                    }
                }

                // Then delete the performance indicator
                await this._unitOfWork.PerformanceIndicators.DeleteAsync(performanceIndicator);
                await this._unitOfWork.CompleteAsync();

                this._logger.LogInformation("Performance indicator with ID: {Id} deleted successfully", id);
                this.AddSuccessAlert("Chỉ số hiệu suất đã được xóa thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while deleting performance indicator with ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while deleting the performance indicator." });
            }
        }

        // GET: PerformanceIndicator/ByResultIndicator/5
        [HttpGet]
        public async Task<IActionResult> ByResultIndicator(Guid resultIndicatorId, string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                this._logger.LogInformation("Retrieving performance indicators for result indicator ID: {ResultIndicatorId}", resultIndicatorId);

                // Get the result indicator to display its details
                var resultIndicator = await this._unitOfWork.ResultIndicators.GetByIdAsync(resultIndicatorId);
                if (resultIndicator == null)
                {
                    return this.NotFound();
                }

                this.ViewBag.ResultIndicatorName = resultIndicator.Name;
                this.ViewBag.ResultIndicatorId = resultIndicatorId;
                this.ViewBag.ResultIndicatorIsKey = resultIndicator.IsKey;

                // Get performance indicators for this result indicator
                var performanceIndicatorsQuery = this._unitOfWork.PerformanceIndicators.GetAll()
                    .Where(pi => pi.ResultIndicatorId == resultIndicatorId);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    performanceIndicatorsQuery = performanceIndicatorsQuery.Where(pi =>
                        pi.Name.Contains(searchTerm) ||
                        (pi.Description != null && pi.Description.Contains(searchTerm)) ||
                        pi.Code.Contains(searchTerm));
                }

                // Include related entities
                performanceIndicatorsQuery = performanceIndicatorsQuery
                    .Include(pi => pi.Department)
                    .Include(pi => pi.Measurements);

                // Get total count for pagination
                var totalItems = await performanceIndicatorsQuery.CountAsync();

                // Apply pagination
                var performanceIndicators = await performanceIndicatorsQuery
                    .OrderBy(pi => pi.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var performanceIndicatorListItems = this._mapper?.Map<List<PerformanceIndicatorListItemViewModel>>(performanceIndicators)
                    ?? new List<PerformanceIndicatorListItemViewModel>();

                // Set view model properties
                this.ViewBag.CurrentPage = page;
                this.ViewBag.PageSize = pageSize;
                this.ViewBag.TotalCount = totalItems;
                this.ViewBag.SearchTerm = searchTerm;
                this.ViewBag.ShowingByResultIndicator = true;

                return this.View("Index", performanceIndicatorListItems);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving performance indicators for result indicator ID: {ResultIndicatorId}", resultIndicatorId);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving the performance indicators." });
            }
        }

        // GET: PerformanceIndicator/CheckCodeExists
        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return this.Json(false);
                }

                var exists = await this._unitOfWork.PerformanceIndicators.GetAll()
                    .AnyAsync(pi => pi.Code == code);

                return this.Json(exists);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while checking code existence: {Code}", code);
                return this.Json(false);
            }
        }

        // GET: PerformanceIndicator/GenerateCode
        [HttpGet]
        public async Task<IActionResult> GenerateCode(string prefix)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    return this.BadRequest("Prefix is required");
                }

                var code = await this.GetUniqueCode(prefix);
                return this.Content(code);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while generating unique code with prefix: {Prefix}", prefix);
                return this.StatusCode(500, "Error generating code");
            }
        }

        private async Task<string> GetUniqueCode(string prefix)
        {
            var maxAttempts = 10;
            var attempt = 0;

            while (attempt < maxAttempts)
            {
                var code = this.GenerateUniqueCode(prefix);
                var exists = await this._unitOfWork.PerformanceIndicators.GetAll()
                    .AnyAsync(pi => pi.Code == code);

                if (!exists)
                {
                    return code;
                }

                attempt++;
            }

            throw new Exception($"Could not generate unique code after {maxAttempts} attempts");
        }

        private string GenerateUniqueCode(string prefix)
        {
            var now = DateTime.Now;
            var year = now.Year % 100;
            var month = now.Month.ToString("00");
            var day = now.Day.ToString("00");
            var random = new Random();
            var randomNum = random.Next(1000, 10000);

            return $"{prefix}{year}{month}{day}-{randomNum}";
        }

        // Helper methods
        private async Task PopulateViewBagDropdowns(Guid? selectedResultIndicatorId = null, Guid? selectedSuccessFactorId = null)
        {
            try
            {
                // Get departments for dropdown
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                this.ViewBag.Departments = new SelectList(departments ?? new List<Department>(), "Id", "Name");

                // Get result indicators for dropdown
                var resultIndicators = await this._unitOfWork.ResultIndicators.GetAllAsync();
                this.ViewBag.ResultIndicators = new SelectList(resultIndicators ?? new List<ResultIndicator>(), "Id", "Name", selectedResultIndicatorId);

                // Get success factors for dropdown
                var successFactors = await this._unitOfWork.SuccessFactors.GetAllAsync();
                this.ViewBag.SuccessFactors = new SelectList(successFactors ?? new List<SuccessFactor>(), "Id", "Name", selectedSuccessFactorId);

                // Set selected result indicator name if provided
                if (selectedResultIndicatorId.HasValue)
                {
                    var selectedRI = await this._unitOfWork.ResultIndicators.GetByIdAsync(selectedResultIndicatorId.Value);
                    if (selectedRI != null)
                    {
                        this.ViewBag.SelectedResultIndicatorName = selectedRI.Name;
                    }
                }

                // Set selected success factor name if provided
                if (selectedSuccessFactorId.HasValue)
                {
                    var selectedSF = await this._unitOfWork.SuccessFactors.GetByIdAsync(selectedSuccessFactorId.Value);
                    if (selectedSF != null)
                    {
                        this.ViewBag.SelectedSuccessFactorName = selectedSF.Name;
                    }
                }

                // Get active users for responsible team member dropdown
                var users = this._userManager?.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.FullName,
                        Selected = false
                    })
                    .ToList() ?? new List<SelectListItem>();
                this.ViewBag.ResponsibleTeamMembers = new SelectList(users, "Value", "Text");

                // Get enum values for dropdowns
                this.ViewBag.MeasurementUnits = this.EnumToSelectList<MeasurementUnit>();
                this.ViewBag.MeasurementFrequencies = this.EnumToSelectList<MeasurementFrequency>();
                this.ViewBag.ReviewFrequencies = this.EnumToSelectList<ReviewFrequency>();
                this.ViewBag.ActivityTypes = this.EnumToSelectList<ActivityType>();
                this.ViewBag.ControlLevels = this.EnumToSelectList<ControlLevel>();
                this.ViewBag.DataCollectionMethods = this.EnumToSelectList<DataCollectionMethod>();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while populating dropdowns");
                throw;
            }
        }

        private async Task PopulateDropdownsForEdit(PerformanceIndicatorEditViewModel viewModel)
        {
            try
            {
                // Get all result indicators for dropdown
                var resultIndicators = await this._unitOfWork.ResultIndicators.GetAll()
                    .OrderBy(ri => ri.Name)
                    .Select(ri => new SelectListItem
                    {
                        Value = ri.Id.ToString(),
                        Text = $"{ri.Name} ({ri.Code})",
                        Selected = viewModel.ResultIndicatorId.HasValue && ri.Id == viewModel.ResultIndicatorId.Value
                    })
                    .ToListAsync();

                // Get all success factors for dropdown
                var successFactors = await this._unitOfWork.SuccessFactors.GetAll()
                    .OrderBy(sf => sf.Name)
                    .Select(sf => new SelectListItem
                    {
                        Value = sf.Id.ToString(),
                        Text = sf.Name,
                        Selected = sf.Id == viewModel.SuccessFactorId
                    })
                    .ToListAsync();

                // Get active users for responsible team member dropdown
                var users = this._userManager?.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.FullName,
                        Selected = viewModel.ResponsibleTeamMemberId.HasValue && u.Id.ToString() == viewModel.ResponsibleTeamMemberId.Value.ToString()
                    })
                    .ToList() ?? new List<SelectListItem>();

                // Set dropdown options directly on the view model
                viewModel.ResultIndicatorOptions = resultIndicators;
                viewModel.SuccessFactorOptions = successFactors;
                viewModel.ResponsibleTeamMemberOptions = users;
                viewModel.UnitOptions = this.EnumToSelectList<MeasurementUnit>(viewModel.Unit);
                viewModel.FrequencyOptions = this.EnumToSelectList<MeasurementFrequency>(viewModel.Frequency);
                viewModel.ReviewFrequencyOptions = this.EnumToSelectList(viewModel.ReviewFrequency);
                viewModel.ActivityTypeOptions = this.EnumToSelectList(viewModel.ActivityType);
                viewModel.ControlLevelOptions = this.EnumToSelectList(viewModel.ControlLevel);
                viewModel.DataCollectionMethodOptions = this.EnumToSelectList(viewModel.DataCollectionMethod);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while populating dropdowns for edit");
                throw;
            }
        }

        private IEnumerable<SelectListItem> EnumToSelectList<TEnum>(TEnum? selectedValue = null) where TEnum : struct, Enum
        {
            var values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = this.GetEnumDisplayName(e),
                    Selected = selectedValue.HasValue && EqualityComparer<TEnum>.Default.Equals(e, selectedValue.Value)
                });

            return values;
        }

        private string GetEnumDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var displayAttribute = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                                              .Cast<DisplayAttribute>()
                                              .FirstOrDefault();
                if (displayAttribute != null)
                    return displayAttribute.Name ?? enumValue.ToString();
            }
            return enumValue.ToString();
        }
    }
}