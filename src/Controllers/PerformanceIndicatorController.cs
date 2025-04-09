using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using KPISolution.Extensions;
using Microsoft.EntityFrameworkCore;

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
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: PerformanceIndicator
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10, bool showOnlyKey = false)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                _logger.LogInformation("Retrieving performance indicator list with search term: {SearchTerm}, page: {Page}, showOnlyKey: {ShowOnlyKey}", 
                    searchTerm, page, showOnlyKey);

                var performanceIndicatorsQuery = _unitOfWork.PerformanceIndicators.GetAll();

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
                var performanceIndicatorListItems = _mapper?.Map<List<PerformanceIndicatorListItemViewModel>>(performanceIndicators)
                    ?? new List<PerformanceIndicatorListItemViewModel>();

                // Create view model
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalItems;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ShowOnlyKey = showOnlyKey;
                ViewBag.IsKeyPerformanceIndicators = showOnlyKey;

                // Populate dropdowns for result indicators and departments
                await PopulateViewBagDropdowns();

                return View(performanceIndicatorListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving performance indicator list");
                return View("Error", new ErrorViewModel { Message = "An error occurred while retrieving the performance indicator list." });
            }
        }

        // GET: PerformanceIndicator/KeyPerformanceIndicators
        [HttpGet]
        public IActionResult KeyPerformanceIndicators(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            // Redirect to the unified Index action with showOnlyKey parameter set to true
            return RedirectToAction(nameof(Index), new { searchTerm, page, pageSize, showOnlyKey = true });
        }

        // GET: PerformanceIndicator/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                _logger.LogInformation("Retrieving performance indicator details for ID: {Id}", id);

                var performanceIndicator = await _unitOfWork.PerformanceIndicators.GetAll()
                    .Include(pi => pi.ResultIndicator!)
                        .ThenInclude(ri => ri!.SuccessFactor)
                    .Include(pi => pi.Department!)
                    .Include(pi => pi.Measurements)
                    .FirstOrDefaultAsync(pi => pi.Id == id);

                if (performanceIndicator?.ResultIndicator == null)
                {
                    _logger.LogWarning("Performance indicator with ID {Id} or its ResultIndicator not found", id);
                    return NotFound();
                }

                // Map entity to view model
                var viewModel = _mapper.Map<PerformanceIndicatorDetailsViewModel>(performanceIndicator);

                // Set additional properties for view
                ViewBag.IsKeyPerformanceIndicator = performanceIndicator.IsKey;
                ViewBag.TypeDisplay = performanceIndicator.IsKey ? "KPI" : "PI";
                ViewBag.TypeName = performanceIndicator.IsKey ? "Key Performance Indicator" : "Performance Indicator";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving performance indicator details for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while retrieving the performance indicator details." });
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

                _logger.LogInformation("Preparing create form for {Type} with Result Indicator ID: {ResultIndicatorId}, Success Factor ID: {SuccessFactorId}",
                    isKey ? "KPI" : "PI", resultIndicatorId, successFactorId);

                // Tạo view model
                var viewModel = new PerformanceIndicatorCreateViewModel
                {
                    ResultIndicatorId = resultIndicatorId,
                    SuccessFactorId = successFactorId
                };

                await PopulateViewBagDropdowns(resultIndicatorId, successFactorId);

                ViewBag.IsKey = isKey;
                ViewBag.TypeDisplay = isKey ? "KPI" : "PI";
                ViewBag.TypeName = isKey ? "Key Performance Indicator" : "Performance Indicator";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing the create form");
                return View("Error", new ErrorViewModel { Message = "An error occurred while preparing the create form." });
            }
        }

        // POST: PerformanceIndicator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerformanceIndicatorCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Creating new performance indicator: {Name}", viewModel.Name);

                    // Kiểm tra xem mã code đã tồn tại chưa
                    if (!string.IsNullOrWhiteSpace(viewModel.Code))
                    {
                        bool codeExists = await _unitOfWork.PerformanceIndicators.GetAll()
                            .AnyAsync(pi => pi.Code == viewModel.Code);

                        if (codeExists)
                        {
                            ModelState.AddModelError("Code", "Mã này đã tồn tại trong hệ thống");
                            await PopulateViewBagDropdowns(viewModel.ResultIndicatorId, viewModel.SuccessFactorId);
                            return View(viewModel);
                        }
                    }

                    // Set additional properties
                    var performanceIndicator = new PerformanceIndicator
                    {
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        Code = string.IsNullOrWhiteSpace(viewModel.Code) ? GenerateUniqueCode(ViewBag.IsKey != null && ViewBag.IsKey ? "KPI-" : "PI-") : viewModel.Code,
                        IsKey = ViewBag.IsKey ?? false,
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
                        CreatedBy = User?.Identity?.Name ?? "System",
                        IsActive = true,
                        Status = IndicatorStatus.Active
                    };

                    // Add entity to repository
                    await _unitOfWork.PerformanceIndicators.AddAsync(performanceIndicator);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation("Performance indicator created successfully with ID: {Id}", performanceIndicator.Id);
                    this.AddSuccessAlert("Chỉ số hiệu suất đã được tạo thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If we got this far, something failed, redisplay form
                _logger.LogWarning("Performance indicator creation failed due to validation errors");
                await PopulateViewBagDropdowns(viewModel.ResultIndicatorId, viewModel.SuccessFactorId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating performance indicator");
                ModelState.AddModelError("", "An error occurred while creating the performance indicator.");
                await PopulateViewBagDropdowns(viewModel.ResultIndicatorId, viewModel.SuccessFactorId);
                return View(viewModel);
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

                _logger.LogInformation("Preparing edit form for performance indicator with ID: {Id}", id);

                var performanceIndicator = await _unitOfWork.PerformanceIndicators.GetByIdAsync(id);
                if (performanceIndicator == null)
                {
                    _logger.LogWarning("Performance indicator with ID {Id} not found for editing", id);
                    return NotFound();
                }

                var viewModel = _mapper.Map<PerformanceIndicatorEditViewModel>(performanceIndicator);

                // Set additional view data
                ViewBag.IsKeyPerformanceIndicator = performanceIndicator.IsKey;
                ViewBag.TypeDisplay = performanceIndicator.IsKey ? "KPI" : "PI";
                ViewBag.TypeName = performanceIndicator.IsKey ? "Key Performance Indicator" : "Performance Indicator";

                // Populate dropdown lists
                await PopulateDropdownsForEdit(viewModel);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing edit form for performance indicator with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while preparing the edit form." });
            }
        }

        // POST: PerformanceIndicator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PerformanceIndicatorEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                _logger.LogWarning("Performance indicator ID mismatch: {Id1} vs {Id2}", id, viewModel.Id);
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Updating performance indicator with ID: {Id}", id);

                    var performanceIndicator = await _unitOfWork.PerformanceIndicators.GetByIdAsync(id);
                    if (performanceIndicator == null)
                    {
                        _logger.LogWarning("Performance indicator with ID {Id} not found for update", id);
                        return NotFound();
                    }

                    // Update entity with view model values
                    _mapper.Map(viewModel, performanceIndicator);

                    // Set audit properties
                    performanceIndicator.UpdatedAt = DateTime.Now;
                    performanceIndicator.UpdatedBy = User?.Identity?.Name ?? "System";

                    _unitOfWork.PerformanceIndicators.Update(performanceIndicator);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation("Performance indicator updated successfully with ID: {Id}", performanceIndicator.Id);
                    this.AddSuccessAlert("Chỉ số hiệu suất đã được cập nhật thành công.");
                    return this.RedirectToPreviousPage();
                }

                // If we got this far, something failed, redisplay form
                _logger.LogWarning("Performance indicator update failed due to validation errors");
                await PopulateDropdownsForEdit(viewModel);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating performance indicator with ID: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the performance indicator.");
                await PopulateDropdownsForEdit(viewModel);
                return View(viewModel);
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

                _logger.LogInformation("Preparing delete confirmation for performance indicator with ID: {Id}", id);

                var performanceIndicator = await _unitOfWork.PerformanceIndicators.GetAll()
                    .Include(pi => pi.ResultIndicator)
                    .Include(pi => pi.Measurements)
                    .FirstOrDefaultAsync(pi => pi.Id == id);

                if (performanceIndicator == null)
                {
                    _logger.LogWarning("Performance indicator with ID {Id} not found for deletion", id);
                    return NotFound();
                }

                var viewModel = _mapper.Map<PerformanceIndicatorDetailsViewModel>(performanceIndicator);

                // Set view data
                ViewBag.IsKeyPerformanceIndicator = performanceIndicator.IsKey;
                ViewBag.TypeDisplay = performanceIndicator.IsKey ? "KPI" : "PI";
                ViewBag.TypeName = performanceIndicator.IsKey ? "Key Performance Indicator" : "Performance Indicator";
                ViewBag.HasMeasurements = performanceIndicator.Measurements?.Any() ?? false;
                ViewBag.MeasurementsCount = performanceIndicator.Measurements?.Count ?? 0;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing delete confirmation for performance indicator with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while preparing the delete confirmation." });
            }
        }

        // POST: PerformanceIndicator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting performance indicator with ID: {Id}", id);

                var performanceIndicator = await _unitOfWork.PerformanceIndicators.GetAll()
                    .Include(pi => pi.Measurements)
                    .FirstOrDefaultAsync(pi => pi.Id == id);

                if (performanceIndicator == null)
                {
                    _logger.LogWarning("Performance indicator with ID {Id} not found for deletion confirmation", id);
                    return NotFound();
                }

                // Record whether this is a KPI or PI for redirection later
                var isKey = performanceIndicator.IsKey;

                // Delete all related measurements first
                if (performanceIndicator.Measurements != null && performanceIndicator.Measurements.Any())
                {
                    _logger.LogInformation("Deleting {Count} measurements related to performance indicator with ID: {Id}",
                        performanceIndicator.Measurements.Count, id);

                    foreach (var measurement in performanceIndicator.Measurements.ToList())
                    {
                        await _unitOfWork.Measurements.DeleteAsync(measurement);
                    }
                }

                // Then delete the performance indicator
                await _unitOfWork.PerformanceIndicators.DeleteAsync(performanceIndicator);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Performance indicator with ID: {Id} deleted successfully", id);
                this.AddSuccessAlert("Chỉ số hiệu suất đã được xóa thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting performance indicator with ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "An error occurred while deleting the performance indicator." });
            }
        }

        // GET: PerformanceIndicator/ByResultIndicator/5
        [HttpGet]
        public async Task<IActionResult> ByResultIndicator(Guid resultIndicatorId, string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving performance indicators for result indicator ID: {ResultIndicatorId}", resultIndicatorId);

                // Get the result indicator to display its details
                var resultIndicator = await _unitOfWork.ResultIndicators.GetByIdAsync(resultIndicatorId);
                if (resultIndicator == null)
                {
                    return NotFound();
                }

                ViewBag.ResultIndicatorName = resultIndicator.Name;
                ViewBag.ResultIndicatorId = resultIndicatorId;
                ViewBag.ResultIndicatorIsKey = resultIndicator.IsKey;

                // Get performance indicators for this result indicator
                var performanceIndicatorsQuery = _unitOfWork.PerformanceIndicators.GetAll()
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
                var performanceIndicatorListItems = _mapper?.Map<List<PerformanceIndicatorListItemViewModel>>(performanceIndicators)
                    ?? new List<PerformanceIndicatorListItemViewModel>();

                // Set view model properties
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalItems;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ShowingByResultIndicator = true;

                return View("Index", performanceIndicatorListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving performance indicators for result indicator ID: {ResultIndicatorId}", resultIndicatorId);
                return View("Error", new ErrorViewModel { Message = "An error occurred while retrieving the performance indicators." });
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
                    return Json(false);
                }

                var exists = await _unitOfWork.PerformanceIndicators.GetAll()
                    .AnyAsync(pi => pi.Code == code);

                return Json(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking code existence: {Code}", code);
                return Json(false);
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
                    return BadRequest("Prefix is required");
                }

                var code = await GetUniqueCode(prefix);
                return Content(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating unique code with prefix: {Prefix}", prefix);
                return StatusCode(500, "Error generating code");
            }
        }

        private async Task<string> GetUniqueCode(string prefix)
        {
            var maxAttempts = 10;
            var attempt = 0;

            while (attempt < maxAttempts)
            {
                var code = GenerateUniqueCode(prefix);
                var exists = await _unitOfWork.PerformanceIndicators.GetAll()
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
                var departments = await _unitOfWork.Departments.GetAllAsync();
                ViewBag.Departments = new SelectList(departments ?? new List<Department>(), "Id", "Name");

                // Get result indicators for dropdown
                var resultIndicators = await _unitOfWork.ResultIndicators.GetAllAsync();
                ViewBag.ResultIndicators = new SelectList(resultIndicators ?? new List<ResultIndicator>(), "Id", "Name", selectedResultIndicatorId);

                // Get success factors for dropdown
                var successFactors = await _unitOfWork.SuccessFactors.GetAllAsync();
                ViewBag.SuccessFactors = new SelectList(successFactors ?? new List<SuccessFactor>(), "Id", "Name", selectedSuccessFactorId);

                // Set selected result indicator name if provided
                if (selectedResultIndicatorId.HasValue)
                {
                    var selectedRI = await _unitOfWork.ResultIndicators.GetByIdAsync(selectedResultIndicatorId.Value);
                    if (selectedRI != null)
                    {
                        ViewBag.SelectedResultIndicatorName = selectedRI.Name;
                    }
                }

                // Set selected success factor name if provided
                if (selectedSuccessFactorId.HasValue)
                {
                    var selectedSF = await _unitOfWork.SuccessFactors.GetByIdAsync(selectedSuccessFactorId.Value);
                    if (selectedSF != null)
                    {
                        ViewBag.SelectedSuccessFactorName = selectedSF.Name;
                    }
                }

                // Get active users for responsible team member dropdown
                var users = _userManager?.Users
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
                ViewBag.ResponsibleTeamMembers = new SelectList(users, "Value", "Text");

                // Get enum values for dropdowns
                ViewBag.MeasurementUnits = EnumToSelectList<MeasurementUnit>();
                ViewBag.MeasurementFrequencies = EnumToSelectList<MeasurementFrequency>();
                ViewBag.ReviewFrequencies = EnumToSelectList<ReviewFrequency>();
                ViewBag.ActivityTypes = EnumToSelectList<ActivityType>();
                ViewBag.ControlLevels = EnumToSelectList<ControlLevel>();
                ViewBag.DataCollectionMethods = EnumToSelectList<DataCollectionMethod>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while populating dropdowns");
                throw;
            }
        }

        private async Task PopulateDropdownsForEdit(PerformanceIndicatorEditViewModel viewModel)
        {
            try
            {
                // Get all result indicators for dropdown
                var resultIndicators = await _unitOfWork.ResultIndicators.GetAll()
                    .OrderBy(ri => ri.Name)
                    .Select(ri => new SelectListItem
                    {
                        Value = ri.Id.ToString(),
                        Text = $"{ri.Name} ({ri.Code})",
                        Selected = viewModel.ResultIndicatorId.HasValue && ri.Id == viewModel.ResultIndicatorId.Value
                    })
                    .ToListAsync();

                // Get all success factors for dropdown
                var successFactors = await _unitOfWork.SuccessFactors.GetAll()
                    .OrderBy(sf => sf.Name)
                    .Select(sf => new SelectListItem
                    {
                        Value = sf.Id.ToString(),
                        Text = sf.Name,
                        Selected = sf.Id == viewModel.SuccessFactorId
                    })
                    .ToListAsync();

                // Get active users for responsible team member dropdown
                var users = _userManager?.Users
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
                viewModel.UnitOptions = EnumToSelectList<MeasurementUnit>(viewModel.Unit);
                viewModel.FrequencyOptions = EnumToSelectList<MeasurementFrequency>(viewModel.Frequency);
                viewModel.ReviewFrequencyOptions = EnumToSelectList(viewModel.ReviewFrequency);
                viewModel.ActivityTypeOptions = EnumToSelectList(viewModel.ActivityType);
                viewModel.ControlLevelOptions = EnumToSelectList(viewModel.ControlLevel);
                viewModel.DataCollectionMethodOptions = EnumToSelectList(viewModel.DataCollectionMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while populating dropdowns for edit");
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
                    Text = GetEnumDisplayName(e),
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