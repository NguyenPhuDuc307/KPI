using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for managing indicator measurements
    /// </summary>
    [Authorize]
    public class MeasurementController : Controller
    {
        private readonly ILogger<MeasurementController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="unitOfWork">Unit of Work for data access</param>
        /// <param name="mapper">Mapper for object-object mapping</param>
        public MeasurementController(
            ILogger<MeasurementController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Displays the list of all measurements with filtering and pagination
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        [HttpGet]
        public async Task<IActionResult> Index(MeasurementSearchViewModel? searchModel = null)
        {
            try
            {
                this._logger.LogInformation("Getting measurements");

                searchModel ??= new MeasurementSearchViewModel();

                // Tạo MeasurementListViewModel thay vì MeasurementIndexViewModel
                var model = new MeasurementListViewModel
                {
                    // Ánh xạ từ MeasurementSearchViewModel sang IndicatorMeasurementFilterViewModel
                    Filter = new IndicatorMeasurementFilterViewModel
                    {
                        // Cần ánh xạ các thuộc tính phù hợp từ searchModel
                        SearchTerm = searchModel.IndicatorName,
                        DepartmentId = searchModel.DepartmentId
                        // Ánh xạ thêm các thuộc tính khác nếu cần
                    },
                    CurrentPage = searchModel.CurrentPage,
                    PageSize = searchModel.PageSize
                };

                // Populate dropdown lists
                model.Departments = await this.GetDepartmentsForDropdown();
                model.IndicatorTypes = this.GetIndicatorTypesForDropdown();
                model.MeasurementFrequencies = this.GetMeasurementFrequenciesForDropdown();

                // Get measurements with filters
                var query = this._unitOfWork.Measurements.GetAll();

                if (searchModel.IndicatorId.HasValue)
                {
                    // Look for measurements where PerformanceIndicatorId or ResultIndicatorId matches
                    query = query.Where(m =>
                        m.PerformanceIndicatorId == searchModel.IndicatorId ||
                        m.ResultIndicatorId == searchModel.IndicatorId);
                }

                if (!string.IsNullOrEmpty(searchModel.IndicatorName))
                {
                    // Fetch the indicators with matching names
                    var piIndicators = this._unitOfWork.PerformanceIndicators
                        .GetAll()
                        .Where(i => i.Name.Contains(searchModel.IndicatorName))
                        .Select(i => i.Id);

                    var riIndicators = this._unitOfWork.ResultIndicators
                        .GetAll()
                        .Where(i => i.Name.Contains(searchModel.IndicatorName))
                        .Select(i => i.Id);

                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && piIndicators.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && riIndicators.Contains(m.ResultIndicatorId.Value)));
                }

                if (searchModel.DepartmentId.HasValue)
                {
                    // Find all indicators belonging to the department
                    var departmentPIs = this._unitOfWork.PerformanceIndicators
                        .GetAll()
                        .Where(i => i.DepartmentId == searchModel.DepartmentId)
                        .Select(i => i.Id);

                    var departmentRIs = this._unitOfWork.ResultIndicators
                        .GetAll()
                        .Where(i => i.DepartmentId == searchModel.DepartmentId)
                        .Select(i => i.Id);

                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && departmentPIs.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && departmentRIs.Contains(m.ResultIndicatorId.Value)));
                }

                if (searchModel.FromDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate >= searchModel.FromDate.Value);
                }

                if (searchModel.ToDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate <= searchModel.ToDate.Value);
                }

                // Apply sorting
                query = query.OrderByDescending(m => m.MeasurementDate);

                // Get total count for pagination
                model.TotalCount = await query.CountAsync();

                // Apply pagination
                query = query.Skip((searchModel.CurrentPage - 1) * searchModel.PageSize)
                             .Take(searchModel.PageSize);

                // Execute query and load related data
                var measurements = await query.ToListAsync();

                // Map measurements to IndicatorValueViewModel instead of MeasurementViewModel
                model.Items = await this.MapToIndicatorValueViewModels(measurements);

                return View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting measurements");
                return View("Error");
            }
        }

        /// <summary>
        /// Maps measurement entities to IndicatorValueViewModel for display
        /// </summary>
        private async Task<List<IndicatorValueViewModel>> MapToIndicatorValueViewModels(List<Measurement> measurements)
        {
            var result = new List<IndicatorValueViewModel>();

            foreach (var measurement in measurements)
            {
                var item = new IndicatorValueViewModel
                {
                    Id = measurement.Id,
                    MeasurementDate = measurement.MeasurementDate,
                    ActualValue = measurement.Value,
                    Status = this.GetStatusDisplayName(measurement.Status),
                    Notes = measurement.Notes,
                    CreatedBy = measurement.CreatedBy ?? "System"
                };

                // Set indicator information based on type
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(measurement.PerformanceIndicatorId.Value);
                    if (pi != null)
                    {
                        item.IndicatorId = pi.Id;
                        item.IndicatorCode = pi.Code;
                        item.IndicatorName = pi.Name;
                        item.TargetValue = pi.TargetValue;
                        item.AchievementPercentage = this.CalculateAchievementPercentage(measurement.Value, pi.TargetValue);
                        item.StatusCssClass = this.GetStatusCssClass(measurement.Status);
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(measurement.ResultIndicatorId.Value);
                    if (ri != null)
                    {
                        item.IndicatorId = ri.Id;
                        item.IndicatorCode = ri.Code;
                        item.IndicatorName = ri.Name;
                        item.TargetValue = ri.TargetValue;
                        item.AchievementPercentage = this.CalculateAchievementPercentage(measurement.Value, ri.TargetValue);
                        item.StatusCssClass = this.GetStatusCssClass(measurement.Status);
                    }
                }

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Calculate achievement percentage
        /// </summary>
        private decimal CalculateAchievementPercentage(decimal actual, decimal? target)
        {
            if (!target.HasValue || target.Value == 0)
                return 0;

            return Math.Round((actual / target.Value) * 100, 2);
        }

        /// <summary>
        /// Get CSS class for status display
        /// </summary>
        private string GetStatusCssClass(MeasurementStatus status)
        {
            return status switch
            {
                MeasurementStatus.Target => "badge bg-success",
                MeasurementStatus.Expected => "badge bg-warning",
                MeasurementStatus.Actual => "badge bg-primary",
                MeasurementStatus.NotSet => "badge bg-secondary",
                MeasurementStatus.Threshold => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }

        /// <summary>
        /// Get friendly display name for MeasurementStatus
        /// </summary>
        private string GetStatusDisplayName(MeasurementStatus status)
        {
            return status switch
            {
                MeasurementStatus.Target => "Mục tiêu",
                MeasurementStatus.Expected => "Dự kiến",
                MeasurementStatus.Actual => "Thực tế",
                MeasurementStatus.NotSet => "Chưa thiết lập",
                MeasurementStatus.Threshold => "Ngưỡng tối thiểu",
                _ => status.ToString()
            };
        }

        /// <summary>
        /// Get departments for dropdown
        /// </summary>
        private async Task<List<SelectListItem>> GetDepartmentsForDropdown()
        {
            var departments = await this._unitOfWork.Departments.GetAll().OrderBy(d => d.Name).ToListAsync();
            return departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
        }

        /// <summary>
        /// Get indicator types for dropdown
        /// </summary>
        private List<SelectListItem> GetIndicatorTypesForDropdown()
        {
            return
            [
                new SelectListItem { Value = "PI", Text = "Chỉ số thực hiện (PI)" },
                new SelectListItem { Value = "RI", Text = "Chỉ số kết quả (RI)" },
                new SelectListItem { Value = "KPI", Text = "Chỉ số KPI" }
            ];
        }

        /// <summary>
        /// Get measurement frequencies for dropdown
        /// </summary>
        private List<SelectListItem> GetMeasurementFrequenciesForDropdown()
        {
            return
            [
                new SelectListItem { Value = "Daily", Text = "Hàng ngày" },
                new SelectListItem { Value = "Weekly", Text = "Hàng tuần" },
                new SelectListItem { Value = "Monthly", Text = "Hàng tháng" },
                new SelectListItem { Value = "Quarterly", Text = "Hàng quý" },
                new SelectListItem { Value = "Yearly", Text = "Hàng năm" }
            ];
        }

        /// <summary>
        /// Creates a measurement form for a specific indicator
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        public async Task<IActionResult> AddMeasurement(Guid indicatorId, IndicatorType type)
        {
            try
            {
                this._logger.LogInformation("Displaying create measurement form for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);

                // Default to current month
                var currentDate = DateTime.Now;
                var viewModel = new MeasurementCreateViewModel
                {
                    MeasurementDate = currentDate,
                    Type = this.GetIndicatorMeasurementType(type)
                };

                // Set details based on indicator type
                switch (type)
                {
                    case IndicatorType.KPI:
                    case IndicatorType.PI:
                        var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(indicatorId);
                        if (pi != null)
                        {
                            viewModel.PerformanceIndicatorId = indicatorId;
                            viewModel.IndicatorName = pi.Name;
                            viewModel.IndicatorCode = pi.Code;
                            viewModel.Unit = pi.Unit;
                            viewModel.TargetValue = pi.TargetValue;
                            viewModel.Frequency = pi.MeasurementFrequency;
                        }
                        break;

                    case IndicatorType.RI:
                        var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(indicatorId);
                        if (ri != null)
                        {
                            viewModel.ResultIndicatorId = indicatorId;
                            viewModel.IndicatorName = ri.Name;
                            viewModel.IndicatorCode = ri.Code;
                            viewModel.Unit = ri.Unit;
                            viewModel.TargetValue = ri.TargetValue;
                            viewModel.Frequency = ri.MeasurementFrequency;
                        }
                        break;

                    case IndicatorType.SF:
                    case IndicatorType.CSF:
                        var sf = await this._unitOfWork.SuccessFactors.GetByIdAsync(indicatorId);
                        if (sf != null)
                        {
                            viewModel.SuccessFactorId = indicatorId;
                            viewModel.IndicatorName = sf.Name;
                            viewModel.IndicatorCode = sf.Code;
                            viewModel.Unit = sf.Unit;
                            viewModel.TargetValue = sf.TargetValue;
                            viewModel.Frequency = sf.MeasurementFrequency;
                        }
                        break;
                }

                if (string.IsNullOrEmpty(viewModel.IndicatorName))
                {
                    this._logger.LogWarning("Indicator with ID {Id} of type {Type} not found", indicatorId, type);
                    return this.NotFound();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying measurement creation form for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);
                return View("Error", new ErrorViewModel { Message = "An error occurred while preparing the measurement form." });
            }
        }

        /// <summary>
        /// Processes the creation of a new measurement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        public async Task<IActionResult> AddMeasurement(MeasurementCreateViewModel viewModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    this._logger.LogWarning("Invalid model state when creating measurement");
                    return View(viewModel);
                }

                this._logger.LogInformation("Creating new measurement for indicator: {Name}", viewModel.IndicatorName);

                // Tạo đối tượng Measurement từ ViewModel
                var measurement = new Measurement
                {
                    Value = viewModel.ActualValue,
                    MeasurementDate = viewModel.MeasurementDate,
                    Notes = viewModel.Notes,
                    Status = viewModel.Status
                };

                // Thiết lập ID chỉ số tương ứng dựa vào loại
                switch (viewModel.Type)
                {
                    case IndicatorMeasurementType.PerformanceIndicator:
                        measurement.PerformanceIndicatorId = viewModel.PerformanceIndicatorId;
                        break;
                    case IndicatorMeasurementType.ResultIndicator:
                        measurement.ResultIndicatorId = viewModel.ResultIndicatorId;
                        break;
                    case IndicatorMeasurementType.SuccessFactor:
                        measurement.SuccessFactorId = viewModel.SuccessFactorId;
                        break;
                    default:
                        this._logger.LogError("Unknown indicator type: {Type}", viewModel.Type);
                        this.ModelState.AddModelError("", "Unknown indicator type.");
                        return View(viewModel);
                }

                // Lưu đo lường mới vào cơ sở dữ liệu
                await this._unitOfWork.Measurements.AddAsync(measurement);
                await this._unitOfWork.SaveChangesAsync();

                // Cập nhật trạng thái của chỉ số tương ứng
                await this.UpdateIndicatorStatusAsync(measurement);

                this._logger.LogInformation("Successfully created measurement with ID: {Id}", measurement.Id);

                this.TempData["SuccessMessage"] = "Đã thêm giá trị đo lường mới thành công.";

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating measurement");
                this.ModelState.AddModelError("", "An unexpected error occurred while saving the measurement.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Displays the measurement history for a specific indicator
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> History(Guid indicatorId, IndicatorType type)
        {
            try
            {
                this._logger.LogInformation("Viewing measurement history for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);

                // Chuẩn bị ViewModel để hiển thị lịch sử
                var viewModel = new MeasurementHistoryViewModel
                {
                    IndicatorId = indicatorId,
                    IndicatorType = type
                };

                IQueryable<Measurement> query = this._unitOfWork.Measurements.GetAll();

                // Lọc các phép đo phù hợp với loại chỉ số
                switch (type)
                {
                    case IndicatorType.KPI:
                    case IndicatorType.PI:
                        query = query.Where(m => m.PerformanceIndicatorId == indicatorId);
                        var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(indicatorId);
                        if (pi != null)
                        {
                            viewModel.IndicatorName = pi.Name;
                            viewModel.IndicatorCode = pi.Code;
                            viewModel.Unit = pi.Unit;
                            viewModel.TargetValue = pi.TargetValue;
                            if (pi.Department != null)
                            {
                                viewModel.Department = pi.Department.Name;
                            }
                        }
                        break;

                    case IndicatorType.RI:
                        query = query.Where(m => m.ResultIndicatorId == indicatorId);
                        var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(indicatorId);
                        if (ri != null)
                        {
                            viewModel.IndicatorName = ri.Name;
                            viewModel.IndicatorCode = ri.Code;
                            viewModel.Unit = ri.Unit;
                            viewModel.TargetValue = ri.TargetValue;
                            if (ri.Department != null)
                            {
                                viewModel.Department = ri.Department.Name;
                            }
                        }
                        break;

                    case IndicatorType.SF:
                    case IndicatorType.CSF:
                        query = query.Where(m => m.SuccessFactorId == indicatorId);
                        var sf = await this._unitOfWork.SuccessFactors.GetByIdAsync(indicatorId);
                        if (sf != null)
                        {
                            viewModel.IndicatorName = sf.Name;
                            viewModel.IndicatorCode = sf.Code;
                            viewModel.Unit = sf.Unit;
                            viewModel.TargetValue = sf.TargetValue;
                            if (sf.Department != null)
                            {
                                viewModel.Department = sf.Department.Name;
                            }
                        }
                        break;

                    default:
                        this._logger.LogWarning("Unsupported indicator type: {Type}", type);
                        return this.NotFound();
                }

                // Kiểm tra chỉ số tồn tại
                if (string.IsNullOrEmpty(viewModel.IndicatorName))
                {
                    this._logger.LogWarning("Indicator with ID {Id} of type {Type} not found", indicatorId, type);
                    return this.NotFound();
                }

                // Lấy dữ liệu đo lường và sắp xếp theo thời gian
                var measurements = await query
                    .OrderByDescending(m => m.MeasurementDate)
                    .ToListAsync();

                // Chuyển đổi sang MeasurementItemViewModel
                var items = measurements.Select(m => new MeasurementItemViewModel
                {
                    Id = m.Id,
                    Value = m.Value,
                    MeasurementDate = m.MeasurementDate,
                    Status = m.Status,
                    Notes = m.Notes,
                    CreatedDate = m.CreatedAt,
                    Period = m.MeasurementDate.ToString("MMM yyyy")
                }).ToList();

                // Tính toán thêm thông tin chi tiết cho từng mục
                if (viewModel.TargetValue.HasValue)
                {
                    foreach (var item in items)
                    {
                        item.AchievementPercentage = Math.Round((item.Value / viewModel.TargetValue.Value) * 100, 2);
                        item.Variance = item.Value - viewModel.TargetValue.Value;
                    }
                }

                viewModel.Measurements = items;

                this._logger.LogInformation("Retrieved {0} measurements for indicator {1}", items.Count, viewModel.IndicatorName);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving measurement history for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);
                return View("Error", new ErrorViewModel { Message = "An error occurred while retrieving measurement history." });
            }
        }

        /// <summary>
        /// Builds a query with applied filters
        /// </summary>
        private IQueryable<Measurement> BuildFilteredQuery(IndicatorMeasurementFilterViewModel filter)
        {
            this._logger.LogInformation("Building filtered query for measurements");
            var query = this._unitOfWork.Measurements.GetAll();

            if (filter != null)
            {
                // Filter by search term (Indicator Code or Notes)
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.Trim().ToLower();
                    // Lấy ID của các Indicator có Code khớp
                    var piIds = this._unitOfWork.PerformanceIndicators.GetAll()
                        .Where(i => i.Code.ToLower().Contains(searchTerm))
                        .Select(i => i.Id)
                        .ToList();
                    var riIds = this._unitOfWork.ResultIndicators.GetAll()
                        .Where(i => i.Code.ToLower().Contains(searchTerm))
                        .Select(i => i.Id)
                        .ToList();
                    // Hoặc ghi chú chứa searchTerm
                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && piIds.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && riIds.Contains(m.ResultIndicatorId.Value)) ||
                        (m.Notes != null && m.Notes.ToLower().Contains(searchTerm)));
                }

                // Filter by department
                if (filter.DepartmentId.HasValue)
                {
                    // Requires joining with indicators to filter by department
                    var departmentId = filter.DepartmentId.Value;
                    var piIds = this._unitOfWork.PerformanceIndicators.GetAll()
                        .Where(i => i.DepartmentId == departmentId)
                        .Select(i => i.Id)
                        .ToList();

                    var riIds = this._unitOfWork.ResultIndicators.GetAll()
                        .Where(i => i.DepartmentId == departmentId)
                        .Select(i => i.Id)
                        .ToList();

                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && piIds.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && riIds.Contains(m.ResultIndicatorId.Value)));
                }

                // Filter by indicator type (Sửa lại logic này)
                if (filter.IndicatorType.HasValue)
                {
                    var indicatorType = filter.IndicatorType.Value;
                    switch (indicatorType)
                    {
                        case IndicatorType.KPI:
                            // KPI = PerformanceIndicator where IsKey = true
                            var kpiIds = this._unitOfWork.PerformanceIndicators.GetAll()
                                .Where(pi => pi.IsKey)
                                .Select(pi => pi.Id)
                                .ToList();
                            query = query.Where(m => m.PerformanceIndicatorId.HasValue && kpiIds.Contains(m.PerformanceIndicatorId.Value));
                            break;
                        case IndicatorType.PI:
                            // PI = PerformanceIndicator where IsKey = false
                            var piIds = this._unitOfWork.PerformanceIndicators.GetAll()
                                .Where(pi => !pi.IsKey)
                                .Select(pi => pi.Id)
                                .ToList();
                            query = query.Where(m => m.PerformanceIndicatorId.HasValue && piIds.Contains(m.PerformanceIndicatorId.Value));
                            break;
                        case IndicatorType.KRI:
                            // KRI = ResultIndicator where IsKey = true
                            var kriIds = this._unitOfWork.ResultIndicators.GetAll()
                                .Where(ri => ri.IsKey)
                                .Select(ri => ri.Id)
                                .ToList();
                            query = query.Where(m => m.ResultIndicatorId.HasValue && kriIds.Contains(m.ResultIndicatorId.Value));
                            break;
                        case IndicatorType.RI:
                            // RI = ResultIndicator where IsKey = false
                            var riIds = this._unitOfWork.ResultIndicators.GetAll()
                                .Where(ri => !ri.IsKey)
                                .Select(ri => ri.Id)
                                .ToList();
                            query = query.Where(m => m.ResultIndicatorId.HasValue && riIds.Contains(m.ResultIndicatorId.Value));
                            break;
                            // Thêm trường hợp cho SuccessFactor/CSF nếu cần thiết
                            // case IndicatorType.SF:
                            // case IndicatorType.CSF:
                            //     var sfIds = _unitOfWork.SuccessFactors.GetAll().Select(sf => sf.Id).ToList();
                            //     query = query.Where(m => m.SuccessFactorId.HasValue && sfIds.Contains(m.SuccessFactorId.Value));
                            //     break;
                    }
                }

                // Filter by date range (support both old and new property names)
                if (filter.StartDate.HasValue || filter.FromDate.HasValue)
                {
                    var startDate = filter.StartDate ?? filter.FromDate;
                    if (startDate.HasValue)
                    {
                        query = query.Where(m => m.MeasurementDate >= startDate.Value);
                    }
                }

                if (filter.EndDate.HasValue || filter.ToDate.HasValue)
                {
                    var endDate = filter.EndDate ?? filter.ToDate;
                    if (endDate.HasValue)
                    {
                        query = query.Where(m => m.MeasurementDate <= endDate.Value);
                    }
                }
            }

            return query;
        }

        /// <summary>
        /// Updates the status of an indicator based on its measurements
        /// </summary>
        private async Task UpdateIndicatorStatusAsync(Measurement measurement)
        {
            try
            {
                this._logger.LogInformation("Updating indicator status based on new measurement");

                // Cập nhật trạng thái của chỉ số dựa trên phép đo mới nhất
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(measurement.PerformanceIndicatorId.Value);
                    if (pi != null && pi.TargetValue.HasValue)
                    {
                        var achievementPercentage = (measurement.Value / pi.TargetValue.Value) * 100;

                        // Cập nhật trạng thái dựa trên phần trăm đạt được
                        pi.CurrentValue = measurement.Value;
                        pi.LastUpdated = DateTime.UtcNow;

                        // TODO: Cập nhật IndicatorStatus khi xác định được enum phù hợp
                        /*
                        if (achievementPercentage >= 100)
                        {
                            pi.Status = IndicatorStatus.OnTarget;
                        }
                        else if (achievementPercentage >= 80)
                        {
                            pi.Status = IndicatorStatus.AtRisk;
                }
                else
                {
                            pi.Status = IndicatorStatus.BelowTarget;
                        }
                        */

                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(measurement.ResultIndicatorId.Value);
                    if (ri != null && ri.TargetValue.HasValue)
                    {
                        var achievementPercentage = (measurement.Value / ri.TargetValue.Value) * 100;

                        // Cập nhật trạng thái
                        ri.CurrentValue = measurement.Value;
                        ri.LastUpdated = DateTime.UtcNow;

                        // TODO: Cập nhật IndicatorStatus khi xác định được enum phù hợp

                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
                else if (measurement.SuccessFactorId.HasValue)
                {
                    var sf = await this._unitOfWork.SuccessFactors.GetByIdAsync(measurement.SuccessFactorId.Value);
                    if (sf != null && sf.TargetValue.HasValue)
                    {
                        var achievementPercentage = (measurement.Value / sf.TargetValue.Value) * 100;

                        // Cập nhật trạng thái
                        sf.CurrentValue = measurement.Value;
                        sf.LastUpdated = DateTime.UtcNow;

                        // TODO: Cập nhật SuccessFactorStatus khi xác định được enum phù hợp

                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error updating indicator status");
                throw;
            }
        }

        /// <summary>
        /// Maps IndicatorType to IndicatorMeasurementType
        /// </summary>
        private IndicatorMeasurementType GetIndicatorMeasurementType(IndicatorType type)
        {
            return type switch
            {
                IndicatorType.KPI or IndicatorType.PI => IndicatorMeasurementType.PerformanceIndicator,
                IndicatorType.RI => IndicatorMeasurementType.ResultIndicator,
                IndicatorType.SF or IndicatorType.CSF => IndicatorMeasurementType.SuccessFactor,
                _ => throw new ArgumentException($"Unsupported indicator type: {type}")
            };
        }

        /// <summary>
        /// Gets cached department select list
        /// </summary>
        private async Task<List<SelectListItem>> GetDepartmentSelectListCached()
        {
            var departments = await this._unitOfWork.Departments.GetAllAsync();
            return departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
        }

        /// <summary>
        /// Gets cached enum select list
        /// </summary>
        private List<SelectListItem> GetEnumSelectListCached<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = e.GetDisplayName()
                }).ToList();
        }

        private async Task<List<MeasurementViewModel>> EnrichMeasurements(List<Measurement> measurements)
        {
            var result = new List<MeasurementViewModel>();

            foreach (var measurement in measurements)
            {
                var viewModel = new MeasurementViewModel
                {
                    Id = measurement.Id,
                    MeasurementDate = measurement.MeasurementDate,
                    Value = measurement.Value,
                    Status = measurement.Status,
                    Notes = measurement.Notes
                };

                // Check if status should be NotSet
                if (!measurement.Status.Equals(MeasurementStatus.NotSet) && !viewModel.TargetValue.HasValue)
                {
                    viewModel.Status = MeasurementStatus.NotSet;
                }

                // Determine indicator type and get details
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var performanceIndicator = await this._unitOfWork.PerformanceIndicators
                        .GetByIdAsync(measurement.PerformanceIndicatorId.Value);

                    if (performanceIndicator != null)
                    {
                        viewModel.IndicatorId = performanceIndicator.Id;
                        viewModel.IndicatorName = performanceIndicator.Name;
                        viewModel.IndicatorType = performanceIndicator.IsKey ? "KPI" : "PI";
                        viewModel.DepartmentName = performanceIndicator.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = performanceIndicator.Unit;
                        viewModel.TargetValue = performanceIndicator.TargetValue;

                        // Set status to NotSet if no target value is available
                        if (!viewModel.TargetValue.HasValue && viewModel.Status != MeasurementStatus.NotSet)
                        {
                            viewModel.Status = MeasurementStatus.NotSet;
                        }

                        result.Add(viewModel);
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var resultIndicator = await this._unitOfWork.ResultIndicators
                        .GetByIdAsync(measurement.ResultIndicatorId.Value);

                    if (resultIndicator != null)
                    {
                        viewModel.IndicatorId = resultIndicator.Id;
                        viewModel.IndicatorName = resultIndicator.Name;
                        viewModel.IndicatorType = resultIndicator.IsKey ? "KRI" : "RI";
                        viewModel.DepartmentName = resultIndicator.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = resultIndicator.Unit;
                        viewModel.TargetValue = resultIndicator.TargetValue;

                        // Set status to NotSet if no target value is available
                        if (!viewModel.TargetValue.HasValue && viewModel.Status != MeasurementStatus.NotSet)
                        {
                            viewModel.Status = MeasurementStatus.NotSet;
                        }

                        result.Add(viewModel);
                    }
                }
                else if (measurement.SuccessFactorId.HasValue)
                {
                    var successFactor = await this._unitOfWork.SuccessFactors
                        .GetByIdAsync(measurement.SuccessFactorId.Value);

                    if (successFactor != null)
                    {
                        viewModel.IndicatorId = successFactor.Id;
                        viewModel.IndicatorName = successFactor.Name;
                        viewModel.IndicatorType = "SF";
                        viewModel.DepartmentName = successFactor.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = successFactor.Unit ?? string.Empty;
                        viewModel.TargetValue = successFactor.TargetValue;
                        result.Add(viewModel);
                    }
                }
            }

            return result;
        }
    }
}
