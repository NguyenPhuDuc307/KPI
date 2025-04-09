using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using KPISolution.Extensions;

namespace KPISolution.Controllers
{
    [Authorize]
    public class ResultIndicatorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ResultIndicatorController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResultIndicatorController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ResultIndicatorController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        private string GenerateUniqueCode(string prefix)
        {
            var timestamp = DateTime.Now.ToString("yyMMdd");
            var random = new Random();
            var randomPart = random.Next(1000, 9999).ToString();
            return $"{prefix}{timestamp}-{randomPart}";
        }

        private async Task<bool> IsCodeUnique(string code)
        {
            return !await _unitOfWork.ResultIndicators.GetAll()
                .AnyAsync(ri => ri.Code == code);
        }

        private async Task<string> GetUniqueCode(string prefix)
        {
            string code;
            do
            {
                code = GenerateUniqueCode(prefix);
            } while (!await IsCodeUnique(code));
            return code;
        }

        // GET: ResultIndicator
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10, bool showOnlyKey = false)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                _logger.LogInformation("Retrieving result indicator list with search term: {SearchTerm}, page: {Page}, showOnlyKey: {ShowOnlyKey}", 
                    searchTerm, page, showOnlyKey);

                var resultIndicatorsQuery = _unitOfWork.ResultIndicators.GetAll();

                // Filter by key status if requested
                if (showOnlyKey)
                {
                    resultIndicatorsQuery = resultIndicatorsQuery.Where(ri => ri.IsKey);
                }

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    resultIndicatorsQuery = resultIndicatorsQuery.Where(ri =>
                        ri.Name.Contains(searchTerm) ||
                        (ri.Description != null && ri.Description.Contains(searchTerm)) ||
                        ri.Code.Contains(searchTerm));
                }

                // Include related entities
                resultIndicatorsQuery = resultIndicatorsQuery
                    .Include(ri => ri.SuccessFactor)
                    .Include(ri => ri.Department);

                // Get total count for pagination
                var totalItems = await resultIndicatorsQuery.CountAsync();

                // Apply pagination
                var resultIndicators = await resultIndicatorsQuery
                    .OrderBy(ri => ri.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var resultIndicatorListItems = _mapper?.Map<List<ResultIndicatorListItemViewModel>>(resultIndicators)
                    ?? new List<ResultIndicatorListItemViewModel>();

                // Create view model
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalItems;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ShowOnlyKey = showOnlyKey;
                ViewBag.IsKeyResultIndicators = showOnlyKey;

                // Populate dropdowns for success factors and departments
                await PopulateViewBagDropdowns();

                return View(resultIndicatorListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving result indicator list");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất danh sách chỉ số kết quả." });
            }
        }

        // GET: ResultIndicator/KeyResultIndicators
        [HttpGet]
        public IActionResult KeyResultIndicators(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            // Redirect to the unified Index action with showOnlyKey parameter set to true
            return RedirectToAction(nameof(Index), new { searchTerm, page, pageSize, showOnlyKey = true });
        }

        // GET: ResultIndicator/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                _logger.LogInformation("Retrieving result indicator details for ID: {Id}", id);

                // Load the main entity and related collections using AsSplitQuery to prevent cartesian explosion
                var resultIndicator = await _unitOfWork.ResultIndicators.GetAll()
                    .Include(ri => ri.SuccessFactor)
                    .Include(ri => ri.Department)
                    .Include(ri => ri.Measurements) // Include measurements
                    .Include(ri => ri.PerformanceIndicators) // Include performance indicators
                    .AsSplitQuery() // <-- Add this to optimize loading multiple collections
                    .FirstOrDefaultAsync(ri => ri.Id == id);

                if (resultIndicator == null)
                {
                    _logger.LogWarning("Result indicator with ID {Id} not found", id);
                    return NotFound();
                }

                // Mapping will now handle the included collections correctly
                var viewModel = _mapper.Map<ResultIndicatorDetailsViewModel>(resultIndicator);

                // Populate RecentMeasurements in the ViewModel after mapping (if mapping doesn't handle it fully)
                // Example: viewModel.RecentMeasurements = _mapper.Map<List<MeasurementViewModel>>(resultIndicator.Measurements.OrderByDescending(m => m.MeasurementDate).Take(5));
                // Ensure your mapping profile or this manual step correctly populates RecentMeasurements.

                // Set additional properties for view
                ViewBag.IsKeyResultIndicator = resultIndicator.IsKey;
                ViewBag.TypeDisplay = resultIndicator.IsKey ? "KRI" : "RI";
                ViewBag.TypeName = resultIndicator.IsKey ? "Key Result Indicator" : "Result Indicator";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving result indicator details for ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất chi tiết chỉ số kết quả." });
            }
        }

        // GET: ResultIndicator/Create
        [HttpGet]
        public async Task<IActionResult> Create(bool? isKey, Guid? successFactorId)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                var model = new ResultIndicatorCreateViewModel
                {
                    IsKey = isKey ?? false
                };

                // Lấy danh sách yếu tố thành công
                var successFactors = await _unitOfWork.SuccessFactors.GetAllAsync();
                ViewBag.SuccessFactors = new SelectList(successFactors, "Id", "Name");

                // Lấy danh sách người dùng đang hoạt động
                var activeUsers = _userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new
                    {
                        Id = u.Id,
                        FullName = $"{u.LastName} {u.FirstName}"
                    })
                    .ToList();
                ViewBag.ResponsibleUsers = new SelectList(activeUsers, "Id", "FullName");
                ViewBag.UserCount = activeUsers.Count;

                // Lấy danh sách PI
                var performanceIndicators = await _unitOfWork.PerformanceIndicators.GetAllAsync();
                ViewBag.PerformanceIndicators = performanceIndicators;

                if (successFactorId.HasValue)
                {
                    model.SuccessFactorId = successFactorId.Value;
                    ViewBag.SuccessFactorId = successFactorId.Value;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing create form");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: ResultIndicator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultIndicatorCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Generate unique code if not provided
                    if (string.IsNullOrWhiteSpace(viewModel.Code))
                    {
                        viewModel.Code = await GetUniqueCode(viewModel.IsKey ? "KRI-" : "RI-");
                    }
                    else if (!await IsCodeUnique(viewModel.Code))
                    {
                        ModelState.AddModelError("Code", "Mã này đã tồn tại trong hệ thống");
                        await PopulateViewBagDropdowns(viewModel.SuccessFactorId);
                        return View(viewModel);
                    }

                    var resultIndicator = _mapper.Map<ResultIndicator>(viewModel);
                    await _unitOfWork.ResultIndicators.AddAsync(resultIndicator);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation("{Type} created successfully with ID: {Id}",
                        viewModel.IsKey ? "KRI" : "RI", resultIndicator.Id);

                    this.AddSuccessAlert("Chỉ số kết quả đã được tạo thành công.");
                    return this.RedirectToPreviousPage();
                }

                _logger.LogWarning("Result indicator creation failed due to validation errors");
                await PopulateViewBagDropdowns(viewModel.SuccessFactorId);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating result indicator");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo chỉ số kết quả.");
                await PopulateViewBagDropdowns(viewModel.SuccessFactorId);
                return View(viewModel);
            }
        }

        // GET: ResultIndicator/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                _logger.LogInformation("Preparing edit form for result indicator ID: {Id}", id);

                var resultIndicator = await _unitOfWork.ResultIndicators.GetAll()
                    .Include(ri => ri.SuccessFactor)
                    .FirstOrDefaultAsync(ri => ri.Id == id);

                if (resultIndicator == null)
                {
                    _logger.LogWarning("Result indicator with ID {Id} not found", id);
                    return NotFound();
                }

                var viewModel = _mapper.Map<ResultIndicatorEditViewModel>(resultIndicator);

                await PopulateDropdownsForEdit(viewModel);

                ViewBag.IsKeyResultIndicator = resultIndicator.IsKey;
                ViewBag.TypeDisplay = resultIndicator.IsKey ? "KRI" : "RI";
                ViewBag.TypeName = resultIndicator.IsKey ? "Key Result Indicator" : "Result Indicator";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing edit form for result indicator ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi chuẩn bị biểu mẫu chỉnh sửa." });
            }
        }

        // POST: ResultIndicator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ResultIndicatorEditViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    _logger.LogWarning("Result indicator ID mismatch. URL ID: {UrlId}, Model ID: {ModelId}", id, viewModel.Id);
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    await PopulateDropdownsForEdit(viewModel);
                    ViewBag.IsKeyResultIndicator = viewModel.IsKey;
                    ViewBag.TypeDisplay = viewModel.IsKey ? "KRI" : "RI";
                    ViewBag.TypeName = viewModel.IsKey ? "Key Result Indicator" : "Result Indicator";
                    return View(viewModel);
                }

                _logger.LogInformation("Updating {Type} with ID: {Id}",
                    viewModel.IsKey ? "KRI" : "RI", viewModel.Id);

                // Get existing entity
                var existingResultIndicator = await _unitOfWork.ResultIndicators.GetByIdAsync(id);
                if (existingResultIndicator == null)
                {
                    _logger.LogWarning("Result indicator with ID {Id} not found during edit", id);
                    return NotFound();
                }

                // Map view model to entity, preserving original values that shouldn't change
                _mapper.Map(viewModel, existingResultIndicator);

                // Set last updated date
                existingResultIndicator.LastUpdated = DateTime.UtcNow;

                // Update in database
                _unitOfWork.ResultIndicators.Update(existingResultIndicator);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("{Type} updated successfully with ID: {Id}",
                    viewModel.IsKey ? "KRI" : "RI", viewModel.Id);

                this.AddSuccessAlert("Chỉ số kết quả đã được cập nhật thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating result indicator");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật chỉ số kết quả.");
                await PopulateDropdownsForEdit(viewModel);
                return View(viewModel);
            }
        }

        // GET: ResultIndicator/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                _logger.LogInformation("Preparing delete confirmation for result indicator ID: {Id}", id);

                var resultIndicator = await _unitOfWork.ResultIndicators.GetAll()
                    .Include(ri => ri.SuccessFactor)
                    .Include(ri => ri.Department)
                    .FirstOrDefaultAsync(ri => ri.Id == id);

                if (resultIndicator == null)
                {
                    _logger.LogWarning("Result indicator with ID {Id} not found", id);
                    return NotFound();
                }

                var viewModel = _mapper.Map<ResultIndicatorDetailsViewModel>(resultIndicator);

                ViewBag.IsKeyResultIndicator = resultIndicator.IsKey;
                ViewBag.TypeDisplay = resultIndicator.IsKey ? "KRI" : "RI";
                ViewBag.TypeName = resultIndicator.IsKey ? "Key Result Indicator" : "Result Indicator";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing delete confirmation for result indicator ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi chuẩn bị xác nhận xóa." });
            }
        }

        // POST: ResultIndicator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting result indicator with ID: {Id}", id);

                var resultIndicator = await _unitOfWork.ResultIndicators.GetByIdAsync(id);
                if (resultIndicator == null)
                {
                    _logger.LogWarning("Result indicator with ID {Id} not found during delete", id);
                    return NotFound();
                }

                var isKey = resultIndicator.IsKey;

                // Check for dependencies
                var hasPerformanceIndicators = await _unitOfWork.PerformanceIndicators.GetAll()
                    .AnyAsync(pi => pi.ResultIndicatorId == id);

                if (hasPerformanceIndicators)
                {
                    _logger.LogWarning("Cannot delete result indicator ID {Id} because it has related performance indicators", id);
                    return View("Error", new ErrorViewModel
                    {
                        Message = "Không thể xóa chỉ số kết quả này vì nó có các chỉ số hiệu suất liên quan."
                    });
                }

                // Delete from database
                _unitOfWork.ResultIndicators.Delete(resultIndicator);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("{Type} deleted successfully with ID: {Id}",
                    isKey ? "KRI" : "RI", id);

                this.AddSuccessAlert("Chỉ số kết quả đã được xóa thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting result indicator ID: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi xóa chỉ số kết quả." });
            }
        }

        // GET: ResultIndicator/BySuccessFactor/5
        [HttpGet]
        public async Task<IActionResult> BySuccessFactor(Guid successFactorId, string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving result indicators for success factor ID: {SuccessFactorId}", successFactorId);

                // Get success factor to display information
                var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(successFactorId);
                if (successFactor == null)
                {
                    return NotFound("Success factor not found");
                }

                // Filter by success factor ID
                var resultIndicatorsQuery = _unitOfWork.ResultIndicators.GetAll()
                    .Where(ri => ri.SuccessFactorId == successFactorId);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    resultIndicatorsQuery = resultIndicatorsQuery.Where(ri =>
                        ri.Name.Contains(searchTerm) ||
                        (ri.Description != null && ri.Description.Contains(searchTerm)) ||
                        ri.Code.Contains(searchTerm));
                }

                // Include related entities
                resultIndicatorsQuery = resultIndicatorsQuery
                    .Include(ri => ri.Department);

                // Get total count for pagination
                var totalItems = await resultIndicatorsQuery.CountAsync();

                // Apply pagination
                var resultIndicators = await resultIndicatorsQuery
                    .OrderBy(ri => ri.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to view model
                var resultIndicatorListItems = _mapper?.Map<List<ResultIndicatorListItemViewModel>>(resultIndicators)
                    ?? new List<ResultIndicatorListItemViewModel>();

                // Create view model
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalItems;
                ViewBag.SearchTerm = searchTerm;

                ViewBag.SuccessFactorName = successFactor.Name;
                ViewBag.SuccessFactorId = successFactorId;
                ViewBag.IsCritical = successFactor.IsCritical;

                return View("Index", resultIndicatorListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving result indicators for success factor");
                return View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất chỉ số kết quả cho yếu tố thành công này." });
            }
        }

        // GET: ResultIndicator/CheckCodeExists
        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return Json(false);
                }

                var exists = await _unitOfWork.ResultIndicators.GetAll()
                    .AnyAsync(ri => ri.Code == code);

                return Json(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking code existence: {Code}", code);
                return Json(false);
            }
        }

        // GET: ResultIndicator/GenerateCode
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

        // Helper methods
        private async Task PopulateViewBagDropdowns(Guid? selectedSuccessFactorId = null)
        {
            // Get all success factors
            var successFactors = await _unitOfWork.SuccessFactors.GetAll()
                .OrderBy(sf => sf.Name)
                .ToListAsync();

            ViewBag.SuccessFactors = new SelectList(successFactors, "Id", "Name", selectedSuccessFactorId);

            // Get all departments
            var departments = await _unitOfWork.Departments.GetAll()
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            // Get active users for responsible team members dropdown
            var users = _userManager.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    Id = u.Id,
                    FullName = $"{u.LastName} {u.FirstName}"
                })
                .ToList();

            ViewBag.ResponsibleUsers = new SelectList(users, "Id", "FullName");
            ViewBag.UserCount = users.Count;

            if (users.Count == 0)
            {
                _logger.LogWarning("No active users found for responsible user dropdown");
            }
        }

        private async Task PopulateDropdownsForEdit(ResultIndicatorEditViewModel viewModel)
        {
            // Get all success factors
            var successFactors = await _unitOfWork.SuccessFactors.GetAll()
                .OrderBy(sf => sf.Name)
                .ToListAsync();

            viewModel.SuccessFactorOptions = new SelectList(successFactors, "Id", "Name", viewModel.SuccessFactorId);

            // Get all departments
            var departments = await _unitOfWork.Departments.GetAll()
                .OrderBy(d => d.Name)
                .ToListAsync();

            // Get active users for responsible user dropdown
            var users = _userManager.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    Id = u.Id,
                    FullName = $"{u.LastName} {u.FirstName}"
                })
                .ToList();

            viewModel.ResponsibleUserOptions = new SelectList(users, "Id", "FullName", viewModel.ResponsibleUserId);

            // Populate enum dropdowns 
            viewModel.UnitOptions = EnumToSelectList<MeasurementUnit>(viewModel.Unit);
            viewModel.FrequencyOptions = EnumToSelectList<MeasurementFrequency>(viewModel.Frequency);

            if (viewModel.MeasurementScope.HasValue)
                viewModel.MeasurementScopeOptions = EnumToSelectList<MeasurementScope>(viewModel.MeasurementScope.Value);
            else
                viewModel.MeasurementScopeOptions = EnumToSelectList<MeasurementScope>();

            if (viewModel.ProcessArea.HasValue)
                viewModel.ProcessAreaOptions = EnumToSelectList<ProcessArea>(viewModel.ProcessArea.Value);
            else
                viewModel.ProcessAreaOptions = EnumToSelectList<ProcessArea>();

            if (viewModel.TimeFrame.HasValue)
                viewModel.TimeFrameOptions = EnumToSelectList<TimeFrame>(viewModel.TimeFrame.Value);
            else
                viewModel.TimeFrameOptions = EnumToSelectList<TimeFrame>();

            if (viewModel.DataSource.HasValue)
                viewModel.DataSourceOptions = EnumToSelectList<DataSource>(viewModel.DataSource.Value);
            else
                viewModel.DataSourceOptions = EnumToSelectList<DataSource>();

            if (viewModel.ResultType.HasValue)
                viewModel.ResultTypeOptions = EnumToSelectList<ResultType>(viewModel.ResultType.Value);
            else
                viewModel.ResultTypeOptions = EnumToSelectList<ResultType>();
        }

        private IEnumerable<SelectListItem> EnumToSelectList<TEnum>(TEnum? selectedValue = null) where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString(),
                    Selected = selectedValue.HasValue && EqualityComparer<TEnum>.Default.Equals(e, selectedValue.Value)
                });
        }
    }
}