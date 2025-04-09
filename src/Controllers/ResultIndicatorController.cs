using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using KPISolution.Extensions;
using KPISolution.Models.Enums.Measurement;

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
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            return !await this._unitOfWork.ResultIndicators.GetAll()
                .AnyAsync(ri => ri.Code == code);
        }

        private async Task<string> GetUniqueCode(string prefix)
        {
            string code;
            do
            {
                code = this.GenerateUniqueCode(prefix);
            } while (!await this.IsCodeUnique(code));
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

                this._logger.LogInformation("Retrieving result indicator list with search term: {SearchTerm}, page: {Page}, showOnlyKey: {ShowOnlyKey}",
                    searchTerm, page, showOnlyKey);

                var resultIndicatorsQuery = this._unitOfWork.ResultIndicators.GetAll();

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
                var resultIndicatorListItems = this._mapper?.Map<List<ResultIndicatorListItemViewModel>>(resultIndicators)
                    ?? new List<ResultIndicatorListItemViewModel>();

                // Create view model
                this.ViewBag.CurrentPage = page;
                this.ViewBag.PageSize = pageSize;
                this.ViewBag.TotalCount = totalItems;
                this.ViewBag.SearchTerm = searchTerm;
                this.ViewBag.ShowOnlyKey = showOnlyKey;
                this.ViewBag.IsKeyResultIndicators = showOnlyKey;

                // Populate dropdowns for success factors and departments
                await this.PopulateViewBagDropdowns();

                return this.View(resultIndicatorListItems);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving result indicator list");
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất danh sách chỉ số kết quả." });
            }
        }

        // GET: ResultIndicator/KeyResultIndicators
        [HttpGet]
        public IActionResult KeyResultIndicators(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            // Redirect to the unified Index action with showOnlyKey parameter set to true
            return this.RedirectToAction(nameof(this.Index), new { searchTerm, page, pageSize, showOnlyKey = true });
        }

        // GET: ResultIndicator/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                this._logger.LogInformation("Retrieving result indicator details for ID: {Id}", id);

                // Load the main entity and related collections using AsSplitQuery to prevent cartesian explosion
                var resultIndicator = await this._unitOfWork.ResultIndicators.GetAll()
                    .Include(ri => ri.SuccessFactor)
                    .Include(ri => ri.Department)
                    .Include(ri => ri.ResponsiblePerson)
                    .Include(ri => ri.Measurements) // Load all related measurements first
                    .Include(ri => ri.PerformanceIndicators!) // Use ! to satisfy nullability check for ThenInclude
                        .ThenInclude(pi => pi.Department)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(ri => ri.Id == id);

                if (resultIndicator == null)
                {
                    this._logger.LogWarning("Result indicator with ID {Id} not found", id);
                    return this.NotFound();
                }

                // Map the main entity to view model
                var viewModel = this._mapper.Map<ResultIndicatorDetailsViewModel>(resultIndicator);

                // Explicitly map measurements and apply ordering/limiting here
                if (resultIndicator.Measurements != null)
                {
                    viewModel.RecentMeasurements = this._mapper.Map<List<MeasurementViewModel>>(
                        resultIndicator.Measurements // Now order and take after loading
                            .OrderByDescending(m => m.MeasurementDate)
                            .Take(10)
                    );

                    // Set additional properties for measurements
                    foreach (var measurement in viewModel.RecentMeasurements)
                    {
                        measurement.IndicatorName = resultIndicator.Name;
                        measurement.IndicatorCode = resultIndicator.Code;
                        measurement.IndicatorUnit = resultIndicator.Unit;
                        measurement.Unit = Enum.TryParse<MeasurementUnit>(resultIndicator.Unit, out var unit) ? unit : MeasurementUnit.Percentage;
                        measurement.TargetValue = resultIndicator.TargetValue;
                    }
                }
                else // Ensure RecentMeasurements is initialized even if Measurements is null
                {
                    viewModel.RecentMeasurements = new List<MeasurementViewModel>();
                }

                // Set additional properties for view
                this.ViewBag.IsKeyResultIndicator = resultIndicator.IsKey;
                this.ViewBag.TypeDisplay = resultIndicator.IsKey ? "KRI" : "RI";
                this.ViewBag.TypeName = resultIndicator.IsKey ? "Key Result Indicator" : "Result Indicator";

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving result indicator details for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất chi tiết chỉ số kết quả." });
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
                var successFactors = await this._unitOfWork.SuccessFactors.GetAllAsync();
                this.ViewBag.SuccessFactors = new SelectList(successFactors, "Id", "Name");

                // Lấy danh sách người dùng đang hoạt động
                var activeUsers = this._userManager.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new
                    {
                        Id = u.Id,
                        FullName = $"{u.LastName} {u.FirstName}"
                    })
                    .ToList();
                this.ViewBag.ResponsibleUsers = new SelectList(activeUsers, "Id", "FullName");
                this.ViewBag.UserCount = activeUsers.Count;

                // Lấy danh sách PI
                var performanceIndicators = await this._unitOfWork.PerformanceIndicators.GetAllAsync();
                this.ViewBag.PerformanceIndicators = performanceIndicators;

                if (successFactorId.HasValue)
                {
                    model.SuccessFactorId = successFactorId.Value;
                    this.ViewBag.SuccessFactorId = successFactorId.Value;
                }

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing create form");
                return this.RedirectToAction("Error", "Home");
            }
        }

        // POST: ResultIndicator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultIndicatorCreateViewModel viewModel)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    // Generate unique code if not provided
                    if (string.IsNullOrWhiteSpace(viewModel.Code))
                    {
                        viewModel.Code = await this.GetUniqueCode(viewModel.IsKey ? "KRI-" : "RI-");
                    }
                    else if (!await this.IsCodeUnique(viewModel.Code))
                    {
                        this.ModelState.AddModelError("Code", "Mã này đã tồn tại trong hệ thống");
                        await this.PopulateViewBagDropdowns(viewModel.SuccessFactorId);
                        return this.View(viewModel);
                    }

                    var resultIndicator = this._mapper.Map<ResultIndicator>(viewModel);
                    await this._unitOfWork.ResultIndicators.AddAsync(resultIndicator);
                    await this._unitOfWork.CompleteAsync();

                    this._logger.LogInformation("{Type} created successfully with ID: {Id}",
                        viewModel.IsKey ? "KRI" : "RI", resultIndicator.Id);

                    this.AddSuccessAlert("Chỉ số kết quả đã được tạo thành công.");
                    return this.RedirectToPreviousPage();
                }

                this._logger.LogWarning("Result indicator creation failed due to validation errors");
                await this.PopulateViewBagDropdowns(viewModel.SuccessFactorId);
                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating result indicator");
                this.ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo chỉ số kết quả.");
                await this.PopulateViewBagDropdowns(viewModel.SuccessFactorId);
                return this.View(viewModel);
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

                this._logger.LogInformation("Preparing edit form for result indicator ID: {Id}", id);

                var resultIndicator = await this._unitOfWork.ResultIndicators.GetAll()
                    .Include(ri => ri.SuccessFactor)
                    .FirstOrDefaultAsync(ri => ri.Id == id);

                if (resultIndicator == null)
                {
                    this._logger.LogWarning("Result indicator with ID {Id} not found", id);
                    return this.NotFound();
                }

                var viewModel = this._mapper.Map<ResultIndicatorEditViewModel>(resultIndicator);

                await this.PopulateDropdownsForEdit(viewModel);

                this.ViewBag.IsKeyResultIndicator = resultIndicator.IsKey;
                this.ViewBag.TypeDisplay = resultIndicator.IsKey ? "KRI" : "RI";
                this.ViewBag.TypeName = resultIndicator.IsKey ? "Key Result Indicator" : "Result Indicator";

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing edit form for result indicator ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi chuẩn bị biểu mẫu chỉnh sửa." });
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
                    this._logger.LogWarning("Result indicator ID mismatch. URL ID: {UrlId}, Model ID: {ModelId}", id, viewModel.Id);
                    return this.BadRequest();
                }

                if (!this.ModelState.IsValid)
                {
                    await this.PopulateDropdownsForEdit(viewModel);
                    this.ViewBag.IsKeyResultIndicator = viewModel.IsKey;
                    this.ViewBag.TypeDisplay = viewModel.IsKey ? "KRI" : "RI";
                    this.ViewBag.TypeName = viewModel.IsKey ? "Key Result Indicator" : "Result Indicator";
                    return this.View(viewModel);
                }

                this._logger.LogInformation("Updating {Type} with ID: {Id}",
                    viewModel.IsKey ? "KRI" : "RI", viewModel.Id);

                // Get existing entity
                var existingResultIndicator = await this._unitOfWork.ResultIndicators.GetByIdAsync(id);
                if (existingResultIndicator == null)
                {
                    this._logger.LogWarning("Result indicator with ID {Id} not found during edit", id);
                    return this.NotFound();
                }

                // Map view model to entity, preserving original values that shouldn't change
                this._mapper.Map(viewModel, existingResultIndicator);

                // Set last updated date
                existingResultIndicator.LastUpdated = DateTime.UtcNow;

                // Update in database
                this._unitOfWork.ResultIndicators.Update(existingResultIndicator);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("{Type} updated successfully with ID: {Id}",
                    viewModel.IsKey ? "KRI" : "RI", viewModel.Id);

                this.AddSuccessAlert("Chỉ số kết quả đã được cập nhật thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while updating result indicator");
                this.ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật chỉ số kết quả.");
                await this.PopulateDropdownsForEdit(viewModel);
                return this.View(viewModel);
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

                this._logger.LogInformation("Preparing delete confirmation for result indicator ID: {Id}", id);

                var resultIndicator = await this._unitOfWork.ResultIndicators.GetAll()
                    .Include(ri => ri.SuccessFactor)
                    .Include(ri => ri.Department)
                    .FirstOrDefaultAsync(ri => ri.Id == id);

                if (resultIndicator == null)
                {
                    this._logger.LogWarning("Result indicator with ID {Id} not found", id);
                    return this.NotFound();
                }

                var viewModel = this._mapper.Map<ResultIndicatorDetailsViewModel>(resultIndicator);

                this.ViewBag.IsKeyResultIndicator = resultIndicator.IsKey;
                this.ViewBag.TypeDisplay = resultIndicator.IsKey ? "KRI" : "RI";
                this.ViewBag.TypeName = resultIndicator.IsKey ? "Key Result Indicator" : "Result Indicator";

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing delete confirmation for result indicator ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi chuẩn bị xác nhận xóa." });
            }
        }

        // POST: ResultIndicator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                this._logger.LogInformation("Deleting result indicator with ID: {Id}", id);

                var resultIndicator = await this._unitOfWork.ResultIndicators.GetByIdAsync(id);
                if (resultIndicator == null)
                {
                    this._logger.LogWarning("Result indicator with ID {Id} not found during delete", id);
                    return this.NotFound();
                }

                var isKey = resultIndicator.IsKey;

                // Check for dependencies
                var hasPerformanceIndicators = await this._unitOfWork.PerformanceIndicators.GetAll()
                    .AnyAsync(pi => pi.ResultIndicatorId == id);

                if (hasPerformanceIndicators)
                {
                    this._logger.LogWarning("Cannot delete result indicator ID {Id} because it has related performance indicators", id);
                    return this.View("Error", new ErrorViewModel
                    {
                        Message = "Không thể xóa chỉ số kết quả này vì nó có các chỉ số hiệu suất liên quan."
                    });
                }

                // Delete from database
                this._unitOfWork.ResultIndicators.Delete(resultIndicator);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("{Type} deleted successfully with ID: {Id}",
                    isKey ? "KRI" : "RI", id);

                this.AddSuccessAlert("Chỉ số kết quả đã được xóa thành công.");
                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while deleting result indicator ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi xóa chỉ số kết quả." });
            }
        }

        // GET: ResultIndicator/BySuccessFactor/5
        [HttpGet]
        public async Task<IActionResult> BySuccessFactor(Guid successFactorId, string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                this._logger.LogInformation("Retrieving result indicators for success factor ID: {SuccessFactorId}", successFactorId);

                // Get success factor to display information
                var successFactor = await this._unitOfWork.SuccessFactors.GetByIdAsync(successFactorId);
                if (successFactor == null)
                {
                    return this.NotFound("Success factor not found");
                }

                // Filter by success factor ID
                var resultIndicatorsQuery = this._unitOfWork.ResultIndicators.GetAll()
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
                var resultIndicatorListItems = this._mapper?.Map<List<ResultIndicatorListItemViewModel>>(resultIndicators)
                    ?? new List<ResultIndicatorListItemViewModel>();

                // Create view model
                this.ViewBag.CurrentPage = page;
                this.ViewBag.PageSize = pageSize;
                this.ViewBag.TotalCount = totalItems;
                this.ViewBag.SearchTerm = searchTerm;

                this.ViewBag.SuccessFactorName = successFactor.Name;
                this.ViewBag.SuccessFactorId = successFactorId;
                this.ViewBag.IsCritical = successFactor.IsCritical;

                return this.View("Index", resultIndicatorListItems);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving result indicators for success factor");
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi truy xuất chỉ số kết quả cho yếu tố thành công này." });
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
                    return this.Json(false);
                }

                var exists = await this._unitOfWork.ResultIndicators.GetAll()
                    .AnyAsync(ri => ri.Code == code);

                return this.Json(exists);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while checking code existence: {Code}", code);
                return this.Json(false);
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

        // Helper methods
        private async Task PopulateViewBagDropdowns(Guid? selectedSuccessFactorId = null)
        {
            // Get all success factors
            var successFactors = await this._unitOfWork.SuccessFactors.GetAll()
                .OrderBy(sf => sf.Name)
                .ToListAsync();

            this.ViewBag.SuccessFactors = new SelectList(successFactors, "Id", "Name", selectedSuccessFactorId);

            // Get all departments
            var departments = await this._unitOfWork.Departments.GetAll()
                .OrderBy(d => d.Name)
                .ToListAsync();

            this.ViewBag.Departments = new SelectList(departments, "Id", "Name");

            // Get active users for responsible team members dropdown
            var users = this._userManager.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    Id = u.Id,
                    FullName = $"{u.LastName} {u.FirstName}"
                })
                .ToList();

            this.ViewBag.ResponsibleUsers = new SelectList(users, "Id", "FullName");
            this.ViewBag.UserCount = users.Count;

            if (users.Count == 0)
            {
                this._logger.LogWarning("No active users found for responsible user dropdown");
            }
        }

        private async Task PopulateDropdownsForEdit(ResultIndicatorEditViewModel viewModel)
        {
            // Get all success factors
            var successFactors = await this._unitOfWork.SuccessFactors.GetAll()
                .OrderBy(sf => sf.Name)
                .ToListAsync();

            viewModel.SuccessFactorOptions = new SelectList(successFactors, "Id", "Name", viewModel.SuccessFactorId);

            // Get all departments
            var departments = await this._unitOfWork.Departments.GetAll()
                .OrderBy(d => d.Name)
                .ToListAsync();

            // Get active users for responsible user dropdown
            var users = this._userManager.Users
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
            viewModel.UnitOptions = this.EnumToSelectList<MeasurementUnit>(viewModel.Unit);
            viewModel.FrequencyOptions = this.EnumToSelectList<MeasurementFrequency>(viewModel.Frequency);

            if (viewModel.MeasurementScope.HasValue)
                viewModel.MeasurementScopeOptions = this.EnumToSelectList<MeasurementScope>(viewModel.MeasurementScope.Value);
            else
                viewModel.MeasurementScopeOptions = this.EnumToSelectList<MeasurementScope>();

            if (viewModel.ProcessArea.HasValue)
                viewModel.ProcessAreaOptions = this.EnumToSelectList<ProcessArea>(viewModel.ProcessArea.Value);
            else
                viewModel.ProcessAreaOptions = this.EnumToSelectList<ProcessArea>();

            if (viewModel.TimeFrame.HasValue)
                viewModel.TimeFrameOptions = this.EnumToSelectList<TimeFrame>(viewModel.TimeFrame.Value);
            else
                viewModel.TimeFrameOptions = this.EnumToSelectList<TimeFrame>();

            if (viewModel.DataSource.HasValue)
                viewModel.DataSourceOptions = this.EnumToSelectList<DataSource>(viewModel.DataSource.Value);
            else
                viewModel.DataSourceOptions = this.EnumToSelectList<DataSource>();

            if (viewModel.ResultType.HasValue)
                viewModel.ResultTypeOptions = this.EnumToSelectList<ResultType>(viewModel.ResultType.Value);
            else
                viewModel.ResultTypeOptions = this.EnumToSelectList<ResultType>();
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